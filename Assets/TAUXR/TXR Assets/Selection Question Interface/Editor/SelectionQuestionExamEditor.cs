using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestionsManager))]
public class SelectionQuestionExamEditor : Editor
{
    private SerializedProperty _startingQuestionIndexSerializedProperty;
    private int _questionToStartExamFromIndex = 0;

    private void OnEnable()
    {
        _startingQuestionIndexSerializedProperty = serializedObject.FindProperty("_startingQuestionIndex");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        QuestionsManager questionsManager = (QuestionsManager)target;

        _startingQuestionIndexSerializedProperty.intValue =
            EditorGUILayout.IntSlider("Starting Question Index", _startingQuestionIndexSerializedProperty.intValue, 0,
                questionsManager.NumberOfSelectionQuestions - 1);

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Previous Question"))
        {
            questionsManager.PreviousQuestion();
        }

        if (GUILayout.Button("Next Question"))
        {
            questionsManager.NextQuestion();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        _questionToStartExamFromIndex = EditorGUILayout.IntSlider("Question to start exam from", _questionToStartExamFromIndex, 0,
            questionsManager.NumberOfSelectionQuestions - 1);
        if (GUILayout.Button("Restart Exam from new question"))
        {
            questionsManager.RunExamFromQuestionIndex(_questionToStartExamFromIndex);
        }

        serializedObject.ApplyModifiedProperties();
    }
}