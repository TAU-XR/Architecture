using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SelectionQuestionSubmitButton : MonoBehaviour
{
    private TXRButton _button;

    public Action AnswerSubmitted;

    private void Awake()
    {
        _button = GetComponent<TXRButton>();
    }

    private void Start()
    {
        _button.Pressed.AddListener(() => AnswerSubmitted?.Invoke());
    }
}