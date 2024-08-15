using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TXRButton))]
public class TXRButtonEditor : Editor
{
    SerializedProperty stateProperty;
    SerializedProperty pressedEventProperty;
    SerializedProperty releasedEventProperty;
    SerializedProperty hoverEnterEventProperty;
    SerializedProperty hoverExitEventProperty;
    SerializedProperty shouldPlaySoundsProperty;
    SerializedProperty responseHoverEnterProperty;
    SerializedProperty responseHoverExitProperty;
    SerializedProperty responsePressProperty;
    SerializedProperty responseReleaseProperty;

    bool showAdvancedConfiguration = false;

    private void OnEnable()
    {
        stateProperty = serializedObject.FindProperty("_state");
        pressedEventProperty = serializedObject.FindProperty("Pressed");
        releasedEventProperty = serializedObject.FindProperty("Released");
        hoverEnterEventProperty = serializedObject.FindProperty("HoverEnter");
        hoverExitEventProperty = serializedObject.FindProperty("HoverExit");
        shouldPlaySoundsProperty = serializedObject.FindProperty("ShouldPlaySounds");
        responseHoverEnterProperty = serializedObject.FindProperty("ResponseHoverEnter");
        responseHoverExitProperty = serializedObject.FindProperty("ResponseHoverExit");
        responsePressProperty = serializedObject.FindProperty("ResponsePress");
        responseReleaseProperty = serializedObject.FindProperty("ResponseRelease");
    }

    public override void OnInspectorGUI()
    {
        // Get the target script
        TXRButton txrButton = (TXRButton)target;

        // Ensure that TXRButtonReferences, Backface, and Stroke are not null
        if (txrButton.References != null && txrButton.References.Backface != null && txrButton.References.Stroke != null && txrButton.References.ButtonInput.transform != null)
        {
            // Begin another change check for the other properties
            EditorGUI.BeginChangeCheck();

            // Draw float fields for the width, height, and stroke offset properties
            float newWidth = EditorGUILayout.FloatField("Width", txrButton.References.Backface.Width);
            float newHeight = EditorGUILayout.FloatField("Height", txrButton.References.Backface.Height);
            string buttonText = EditorGUILayout.TextField("Text", txrButton.References.Text.text);
            float textSize = EditorGUILayout.FloatField("Text Size", txrButton.References.Text.fontSize);
            Color textColor = EditorGUILayout.ColorField("Text Color", txrButton.References.Text.color);

            // Draw color fields for the four color properties
            Color activeColor = EditorGUILayout.ColorField("Active Color", txrButton.References.Backface.FillColorEnd);
            Color pressedColor = EditorGUILayout.ColorField("Pressed Color", txrButton.References.PressedColor);
            Color disabledColor = EditorGUILayout.ColorField("Disabled Color", txrButton.References.DisabledColor);
            Color hoverGradientColor = EditorGUILayout.ColorField("Hover Gradient Color", txrButton.References.HoverGradientColor);

            // If there are any changes
            if (EditorGUI.EndChangeCheck())
            {
                // Record the changes for undo functionality
                Undo.RecordObject(txrButton.References.Backface, "Change Backface Dimensions");
                Undo.RecordObject(txrButton.References.Stroke, "Change Stroke Dimensions");
                Undo.RecordObject(txrButton.References.ButtonInput.transform, "Change Input Dimensions");
                Undo.RecordObject(txrButton.References.Text, "Change Text");
                Undo.RecordObject(txrButton.References, "Change Button Colors");

                // Apply the new width and height values to Backface
                txrButton.References.Backface.Width = newWidth;
                txrButton.References.Backface.Height = newHeight;

                // Apply the new width and height values to Stroke, including the stroke offset
                txrButton.References.Stroke.Width = newWidth + 0.005f;
                txrButton.References.Stroke.Height = newHeight + 0.005f;

                // Apply input collider size change
                txrButton.References.ButtonInput.transform.localScale = new Vector3(newWidth + 0.025f, newHeight + 0.025f, 0.08f);

                // Apply button text
                txrButton.References.Text.text = buttonText;
                txrButton.References.Text.fontSize = textSize;
                txrButton.References.Text.color = textColor;
                txrButton.References.Text.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);

                // Apply the new colors to the references
                txrButton.References.ActiveColor = activeColor;
                txrButton.References.PressedColor = pressedColor;
                txrButton.References.DisabledColor = disabledColor;
                txrButton.References.HoverGradientColor = hoverGradientColor;
                if (Application.isPlaying)
                    txrButton.References.ButtonVisuals.UpdateColorsFromReferences();

                // Apply the active color to backface fill color end
                txrButton.References.Backface.FillColorEnd = activeColor;

                EditorUtility.SetDirty(txrButton.References.Backface);
                EditorUtility.SetDirty(txrButton.References.Stroke);
                EditorUtility.SetDirty(txrButton.References);
            }

            EditorGUILayout.PropertyField(shouldPlaySoundsProperty, new GUIContent("Should Play Sounds"));

            // Draw the Pressed event
            EditorGUILayout.PropertyField(pressedEventProperty, new GUIContent("Pressed Event"));

            // Begin a change check for the state property
            EditorGUI.BeginChangeCheck();

            // Draw the state property field
            EditorGUILayout.PropertyField(stateProperty, new GUIContent("State"));

            // Check if the state property was changed
            bool stateChanged = EditorGUI.EndChangeCheck();

            // If the state property was changed
            if (stateChanged)
            {
                serializedObject.ApplyModifiedProperties();
            }

            // Add a button to call the SetState method manually
            if (GUILayout.Button("Set State"))
            {
                if (Application.isPlaying) txrButton.SetState();
            }

            // Advanced Configuration toggle
            showAdvancedConfiguration = EditorGUILayout.Foldout(showAdvancedConfiguration, "Advanced Configuration");
            if (showAdvancedConfiguration)
            {
                EditorGUILayout.PropertyField(releasedEventProperty, new GUIContent("Released Event"));
                EditorGUILayout.PropertyField(hoverEnterEventProperty, new GUIContent("Hover Enter Event"));
                EditorGUILayout.PropertyField(hoverExitEventProperty, new GUIContent("Hover Exit Event"));

                EditorGUILayout.PropertyField(responseHoverEnterProperty, new GUIContent("Response Hover Enter"));
                EditorGUILayout.PropertyField(responseHoverExitProperty, new GUIContent("Response Hover Exit"));
                EditorGUILayout.PropertyField(responsePressProperty, new GUIContent("Response Press"));
                EditorGUILayout.PropertyField(responseReleaseProperty, new GUIContent("Response Release"));
            }

            // Apply changes to the serialized object
            serializedObject.ApplyModifiedProperties();
        }
        else
        {
            EditorGUILayout.HelpBox("References, Backface, or Stroke is not assigned.", MessageType.Warning);
        }
    }
}
