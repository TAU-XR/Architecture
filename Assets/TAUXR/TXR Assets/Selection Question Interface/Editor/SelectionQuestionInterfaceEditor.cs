using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SelectionQuestionInterface))]
public class SelectionQuestionInterfaceEditor : Editor
{
    private TXRButton _submitButton;

    private void OnEnable()
    {
        SelectionQuestionInterface selectionQuestionInterface = (SelectionQuestionInterface)target;
        _submitButton = selectionQuestionInterface.GetComponentInChildren<SelectionQuestionSubmitButton>(true).GetComponent<TXRButton>();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Submit and release"))
        {
            _submitButton.TriggerButtonEventFromCode(ButtonEvent.Pressed, ButtonColliderResponse.Both);
            _submitButton.TriggerButtonEventFromCode(ButtonEvent.Released, ButtonColliderResponse.Both);
        }
    }
}