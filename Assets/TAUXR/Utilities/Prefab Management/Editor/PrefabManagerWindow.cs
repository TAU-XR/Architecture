using UnityEngine;
using UnityEditor;

public class PrefabManagerWindow : EditorWindow
{
    private GameObject[] prefabs;
    private SerializedObject serializedObject;
    private SerializedProperty prefabsProperty;

    [MenuItem("Tools/Prefab Manager")]
    public static void ShowWindow()
    {
        GetWindow<PrefabManagerWindow>("Prefab Manager");
    }

    private void OnEnable()
    {
        prefabs = new GameObject[0];
        serializedObject = new SerializedObject(this);
        prefabsProperty = serializedObject.FindProperty("prefabs");
    }

    private void OnGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Prefab Manager", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(prefabsProperty, true);

        if (prefabsProperty.isExpanded)
        {
            for (int i = 0; i < prefabsProperty.arraySize; i++)
            {
                SerializedProperty prefabProp = prefabsProperty.GetArrayElementAtIndex(i);
                GameObject prefab = (GameObject)prefabProp.objectReferenceValue;

                if (prefab != null)
                {
                    Texture2D previewTexture = AssetPreview.GetAssetPreview(prefab);

                    if (previewTexture != null)
                    {
                        GUILayout.Label(previewTexture);
                    }

                    if (GUILayout.Button("Spawn " + prefab.name))
                    {
                        SpawnPrefab(prefab);
                    }
                }
            }
        }

        if (GUILayout.Button("Add Prefab Slot"))
        {
            prefabsProperty.arraySize++;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void SpawnPrefab(GameObject prefab)
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Undo.RegisterCreatedObjectUndo(instance, "Spawn Prefab");
        Selection.activeObject = instance;
    }
}
