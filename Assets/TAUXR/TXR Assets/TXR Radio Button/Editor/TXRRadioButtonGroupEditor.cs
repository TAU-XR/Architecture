using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TXRRadioButtonGroup))]
public class TXRRadioButtonGroupEditor : Editor
{
    [Range(0, 3)] private static int _buttonIndex = 0;
    private static bool _isEditorUtilitiesFoldoutGroupOpen = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TXRRadioButtonGroup radioButtonGroup = (TXRRadioButtonGroup)target;

        EditorGUILayout.Space(10);
        _isEditorUtilitiesFoldoutGroupOpen =
            EditorGUILayout.Foldout(_isEditorUtilitiesFoldoutGroupOpen, "Editor Utilities", EditorStyles.foldoutHeader);

        if (!_isEditorUtilitiesFoldoutGroupOpen) return;

        GUILayout.Label("Button index:");
        int numberOfButtons = radioButtonGroup.Buttons.Count(button => button.gameObject.activeSelf);
        _buttonIndex = EditorGUILayout.IntSlider(_buttonIndex, 0, numberOfButtons - 1);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Select Button"))
        {
            radioButtonGroup.Buttons[_buttonIndex].gameObject.GetComponent<TXRButton_Toggle>()
                .TriggerToggleEvent(TXRButtonToggleState.On, ButtonColliderResponse.Both);
        }

        if (GUILayout.Button("Deselect Button"))
        {
            radioButtonGroup.Buttons[_buttonIndex].gameObject.GetComponent<TXRButton_Toggle>()
                .TriggerToggleEvent(TXRButtonToggleState.Off, ButtonColliderResponse.Both);
        }

        EditorGUILayout.EndHorizontal();
    }
}