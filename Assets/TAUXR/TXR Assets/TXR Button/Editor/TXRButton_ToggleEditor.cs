using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TXRButton_Toggle))]
public class TXRButton_ToggleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TXRButton_Toggle button = (TXRButton_Toggle)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Toggle On"))
        {
            button.TriggerToggleEvent(TXRButtonToggleState.On, ButtonColliderResponse.Both);
            // button.TriggerButtonEventFromInput(ButtonEvent.Pressed);
        }

        if (GUILayout.Button("Toggle Off"))
        {
            button.TriggerToggleEvent(TXRButtonToggleState.Off, ButtonColliderResponse.Both);
            // button.TriggerButtonEventFromInput(ButtonEvent.Released);
        }

        EditorGUILayout.EndHorizontal();
    }
}