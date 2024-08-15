using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SelectionQuestionInterface))]
public class QuestionsManager : MonoBehaviour
{
    private CancellationTokenSource _cts;

    public int NumberOfSelectionQuestions => _selectionQuestions.Length;

    [SerializeField] private SelectionQuestionData[] _selectionQuestions;
    [SerializeField] private float _timeBetweenQuestions = 5;
    [SerializeField] [HideInInspector] private int _startingQuestionIndex = 0;
    [SerializeField] [ReadOnly] private int _currentQuestionIndex;
    private SelectionQuestionInterface _selectionQuestionInterface;

    private void Start()
    {
        _currentQuestionIndex = _startingQuestionIndex;
        _selectionQuestionInterface = GetComponent<SelectionQuestionInterface>();
        RunExamFromCurrentQuestion().Forget();
    }

    private async UniTask RunExamFromCurrentQuestion()
    {
        _cts = new CancellationTokenSource();
        while (_currentQuestionIndex < _selectionQuestions.Length)
        {
            await _selectionQuestionInterface.ShowQuestionAndWaitForFinalSubmission(_selectionQuestions[_currentQuestionIndex], _cts.Token);
            _currentQuestionIndex++;
            await UniTask.Delay(TimeSpan.FromSeconds(_timeBetweenQuestions), cancellationToken: _cts.Token);
        }
    }

    public void NextQuestion()
    {
        RunExamFromQuestionIndex(_currentQuestionIndex + 1);
    }

    public void PreviousQuestion()
    {
        RunExamFromQuestionIndex(_currentQuestionIndex - 1);
    }

    public void RunExamFromQuestionIndex(int questionIndex)
    {
        if (questionIndex < 0 || questionIndex > _selectionQuestions.Length)
        {
            return;
        }

        _currentQuestionIndex = questionIndex;
        _cts.Cancel();
        RunExamFromCurrentQuestion().Forget();
    }
}