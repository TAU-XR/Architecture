using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectionQuestion", menuName = "ScriptableObjects/SelectionQuestion")]
public class SelectionQuestionData : ScriptableObject
{
    [TextArea]
    public string Text;
    public SelectionAnswerData[] Answers;
    public int MaxNumberOfTries = 4;

    //TODO: add this in a future improvement
    public bool MultipleSelection;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Answers == null || Answers.Length is < 2 or > 4)
        {
            Debug.LogWarning("Selection answers must be between 2 and 4");
        }
    }
#endif
}