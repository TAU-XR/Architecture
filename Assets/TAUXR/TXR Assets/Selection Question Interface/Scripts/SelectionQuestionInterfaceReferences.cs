using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SelectionQuestionInterfaceReferences
{
    public TextDisplay QuestionTextDisplay => _questionTextDisplay;
    public TextDisplay AnswerInfo => _answerInfo;
    public SelectionQuestionSubmitButton SubmitButton => _submitButton;
    public SelectionAnswerButton[] SelectionAnswers => _selectionAnswers;
    public GameObject Graphics => _graphics;


    [SerializeField] private TextDisplay _questionTextDisplay;
    [SerializeField] private TextDisplay _answerInfo;
    [SerializeField] private SelectionQuestionSubmitButton _submitButton;
    [SerializeField] private SelectionAnswerButton[] _selectionAnswers;
    [SerializeField] private GameObject _graphics;
}