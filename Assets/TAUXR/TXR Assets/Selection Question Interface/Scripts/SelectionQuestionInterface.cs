using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class SelectionQuestionInterface : MonoBehaviour
{
    public Action<SelectionAnswerData> AnswerSubmitted;

    [SerializeField] private SelectionQuestionInterfaceReferences _references;

    // remove scriptableObject. use time as const and color as serialized fields.
    [SerializeField] private SelectionAnswerButtonConfiguration _correctAnswerButtonConfiguration;
    [SerializeField] private SelectionAnswerButtonConfiguration _wrongAnswerButtonConfiguration;
    [SerializeField] private bool _startHidden;


    private bool _finishedAnswering = false;
    private SelectionQuestionData _questionData;

    private TXRRadioButtonGroup _selectionAnswersRadioButtonGroup;

    private int _numberOfTriesInCurrentQuestion;

    private void Awake()
    {
        _selectionAnswersRadioButtonGroup = GetComponent<TXRRadioButtonGroup>();
    }

    private void OnEnable()
    {
        _references.SubmitButton.AnswerSubmitted += OnAnswerSubmitted;
    }

    private void OnDisable()
    {
        _references.SubmitButton.AnswerSubmitted -= OnAnswerSubmitted;
    }


    public void Show()
    {
        // _references.SubmitButton.AnswerSubmitted += OnAnswerSubmitted;
        _references.Graphics.SetActive(true);
    }

    public void Hide()
    {
        if (_references.AnswerInfo != null)
        {
            _references.AnswerInfo.Hide();
        }

        // _references.SubmitButton.AnswerSubmitted -= OnAnswerSubmitted;
        _references.Graphics.SetActive(false);
    }

    private void Start()
    {
        if (_startHidden)
        {
            Hide();
        }
    }

    public async UniTask ShowQuestionAndWaitForFinalSubmission(SelectionQuestionData selectionQuestionData,
        CancellationToken cancellationToken = default)
    {
        Show();
        InitializeWithNewQuestion(selectionQuestionData);

        await UniTask.WaitUntil(() => _finishedAnswering, cancellationToken: cancellationToken);
    }

    private void InitializeWithNewQuestion(SelectionQuestionData selectionQuestionData)
    {
        _numberOfTriesInCurrentQuestion = 0;
        _references.AnswerInfo.Hide();
        _questionData = selectionQuestionData;
        _finishedAnswering = false;
        _references.QuestionTextDisplay.SetText(_questionData.Text);
        InitializeNewAnswers(selectionQuestionData.Answers);
    }

    private void InitializeNewAnswers(SelectionAnswerData[] selectionAnswersData)
    {
        _selectionAnswersRadioButtonGroup.Reset();
        int numberOfAnswersInInterface = _selectionAnswersRadioButtonGroup.Buttons.Length;
        for (int answerIndex = 0; answerIndex < numberOfAnswersInInterface; answerIndex++)
        {
            SelectionAnswerButton selectionAnswerButton =
                _selectionAnswersRadioButtonGroup.Buttons[answerIndex].GetComponent<SelectionAnswerButton>();
            bool answerIsActive = answerIndex < selectionAnswersData.Length;
            selectionAnswerButton.gameObject.SetActive(answerIsActive);
            if (!answerIsActive) continue;
            bool isCorrectAnswer = selectionAnswersData[answerIndex].IsCorrect;
            SelectionAnswerButtonConfiguration buttonConfiguration =
                isCorrectAnswer ? _correctAnswerButtonConfiguration : _wrongAnswerButtonConfiguration;
            selectionAnswerButton.SetNewAnswer(selectionAnswersData[answerIndex], buttonConfiguration);
        }
    }


    /*  private void OnAnswerSubmitted()
      {
          if (_selectionAnswersRadioButtonGroup.SelectedButton == null)
          {
              return;
          }
  
          // cr: get answer on event
          SelectionAnswerButton selectedAnswer = _selectionAnswersRadioButtonGroup.SelectedButton.GetComponent<SelectionAnswerButton>();
          _finishedAnswering = selectedAnswer.SelectionAnswerData.IsCorrect;
  
  
          bool noAnswersLeft = _numberOfTriesInCurrentQuestion == _questionData.Answers.Length - 2;
          bool outOfTries = _numberOfTriesInCurrentQuestion - 1 == _questionData.MaxNumberOfTries;
          bool shouldSubmitCorrectAnswer = (outOfTries || noAnswersLeft) && !_finishedAnswering;
  
          SubmitSelectedAnswer(showAnswerInfo: !shouldSubmitCorrectAnswer);
  
          if (!shouldSubmitCorrectAnswer) return;
          _finishedAnswering = true;
          SubmitCorrectAnswer().Forget(); // cr: confusing name, would implement this inside this method.
      }
  
      private async UniTask SubmitCorrectAnswer()
      {
          await UniTask.Delay(TimeSpan.FromSeconds(_wrongAnswerButtonConfiguration.TimeFromSubmitToDisable));
          _selectionAnswersRadioButtonGroup.SelectButton(GetCorrectAnswer().GetComponent<TXRButton_Toggle>());
          SubmitSelectedAnswer();
      }
  
      private void SubmitSelectedAnswer(bool showAnswerInfo = true)
      {
          // cr: get answer on method parameter
          SelectionAnswerButton selectedAnswer = _selectionAnswersRadioButtonGroup.SelectedButton.GetComponent<SelectionAnswerButton>();
          AnswerSubmitted?.Invoke(selectedAnswer.SelectionAnswerData);
          _numberOfTriesInCurrentQuestion++;
          if (showAnswerInfo)
          {
              ShowAnswerInfo(selectedAnswer.SelectionAnswerData.AnswerInfo);
          }
  
          _selectionAnswersRadioButtonGroup.Reset();
  
          selectedAnswer.OnAnswerSubmitted().Forget();
  
          if (_finishedAnswering)
          {
              DisableWrongAnswers();
          }
      }*/

    private void OnAnswerSubmitted()
    {
        if (_selectionAnswersRadioButtonGroup.SelectedButton == null) return;

        SelectionAnswerButton selectedAnswer = _selectionAnswersRadioButtonGroup.SelectedButton.GetComponent<SelectionAnswerButton>();
        SubmitAnswer(selectedAnswer).Forget();
    }

    private async UniTask SubmitAnswer(SelectionAnswerButton selectedAnswer)
    {
        if (_selectionAnswersRadioButtonGroup.SelectedButton == null) return;

        _selectionAnswersRadioButtonGroup.Reset();
        _numberOfTriesInCurrentQuestion++;
        selectedAnswer.OnAnswerSubmitted().Forget();
        AnswerSubmitted?.Invoke(selectedAnswer.SelectionAnswerData);

        if (selectedAnswer.SelectionAnswerData.IsCorrect)
        {
            OnCorrectAnswerSubmitted(selectedAnswer);
        }
        else
        {
            await OnWrongAnswerSubmitted(selectedAnswer);
        }

    }

    private void OnCorrectAnswerSubmitted(SelectionAnswerButton selectedAnswer)
    {
        ShowAnswerInfo(selectedAnswer.SelectionAnswerData.AnswerInfo);
        DisableWrongAnswers();
        _finishedAnswering = true;
    }

    private void DisableWrongAnswers()
    {
        foreach (TXRButton_Toggle button in _selectionAnswersRadioButtonGroup.Buttons)
        {
            if (button.GetComponent<SelectionAnswerButton>().SelectionAnswerData.IsCorrect)
            {
                continue;
            }

            button.SetState(TXRButtonState.Disabled);
        }
    }

    private async UniTask OnWrongAnswerSubmitted(SelectionAnswerButton selectedAnswer)
    {
        bool noAnswersLeft = _numberOfTriesInCurrentQuestion == _questionData.Answers.Length - 1;
        bool outOfTries = _numberOfTriesInCurrentQuestion == _questionData.MaxNumberOfTries;
        Debug.Log(_numberOfTriesInCurrentQuestion);
        if (outOfTries || noAnswersLeft)
        {
            await ShowCorrectAnswer();
        }
        else
        {
            ShowAnswerInfo(selectedAnswer.SelectionAnswerData.AnswerInfo);
        }
    }

    private async UniTask ShowCorrectAnswer()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_wrongAnswerButtonConfiguration.TimeFromSubmitToDisable));
        DisableWrongAnswers();
        SelectionAnswerButton correctAnswer = GetCorrectAnswer();
        correctAnswer.OnAnswerSubmitted().Forget();
        ShowAnswerInfo(correctAnswer.SelectionAnswerData.AnswerInfo);
        _finishedAnswering = true;
    }


    private void ShowAnswerInfo(string selectedAnswerInfo)
    {
        _references.AnswerInfo.Show();
        _references.AnswerInfo.SetText(selectedAnswerInfo);
    }

    private SelectionAnswerButton GetCorrectAnswer()
    {
        return _references.SelectionAnswers.ToList()
            .Find((selectionAnswer) => selectionAnswer.SelectionAnswerData.IsCorrect);
    }
}