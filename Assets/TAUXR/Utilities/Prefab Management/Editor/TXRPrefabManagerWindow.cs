using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.SceneManagement;

public class TXRPrefabManagerWindow : EditorWindow
{
    private TXRPrefabManager prefabManager;
    private SerializedObject serializedPrefabManager;
    private SerializedProperty prefabsProperty;

    private const string prefabManagerPath = "Assets/TAUXR/Utilities/Prefab Management/TXRPrefabManager.asset";

    [MenuItem("Tools/TXR Prefab Manager")]
    public static void ShowWindow()
    {
        GetWindow<TXRPrefabManagerWindow>("TXR Prefab Manager");
    }

    private void OnEnable()
    {
        LoadPrefabManager();
    }

    private void LoadPrefabManager()
    {
        prefabManager = AssetDatabase.LoadAssetAtPath<TXRPrefabManager>(prefabManagerPath);

        if (prefabManager != null)
        {
            serializedPrefabManager = new SerializedObject(prefabManager);
            prefabsProperty = serializedPrefabManager.FindProperty("Prefabs");
        }
    }

    private void OnGUI()
    {
        if (prefabManager == null)
        {
            EditorGUILayout.HelpBox($"TXRPrefabManager not found at path: {prefabManagerPath}. Please create one.", MessageType.Warning);
            if (GUILayout.Button("Create TXRPrefabManager"))
            {
                CreatePrefabManager();
            }
            return;
        }

        serializedPrefabManager.Update();

        EditorGUILayout.LabelField("TXR Prefab Manager", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (prefabsProperty != null && prefabsProperty.isArray)
        {
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < prefabsProperty.arraySize; i++)
            {
                if (i > 0 && i % 2 == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(20); // Add space between each line of prefabs
                    EditorGUILayout.BeginHorizontal();
                }

                if (i % 2 == 0)
                {
                    EditorGUILayout.BeginHorizontal(); // Start a new horizontal group for every two prefabs
                }

                SerializedProperty prefabProperty = prefabsProperty.GetArrayElementAtIndex(i);
                SerializedProperty prefabObjectProperty = prefabProperty.FindPropertyRelative("Prefab");
                SerializedProperty prefabNameProperty = prefabProperty.FindPropertyRelative("Name");
                SerializedProperty prefabImageProperty = prefabProperty.FindPropertyRelative("Image");

                EditorGUILayout.BeginVertical(GUILayout.Width(200));

                // Clickable title
                if (GUILayout.Button(new GUIContent(prefabNameProperty.stringValue), new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleLeft }))
                {
                    PingObject((GameObject)prefabObjectProperty.objectReferenceValue);
                }

                EditorGUILayout.Space();

                // Clickable image
                Texture2D prefabImage = (Texture2D)prefabImageProperty.objectReferenceValue;
                if (prefabImage != null)
                {
                    if (GUILayout.Button(prefabImage, GUILayout.Width(100), GUILayout.Height(100)))
                    {
                        SpawnPrefab((GameObject)prefabObjectProperty.objectReferenceValue);
                    }
                }
                else
                {
                    if (GUILayout.Button("Spawn", GUILayout.Width(100)))
                    {
                        SpawnPrefab((GameObject)prefabObjectProperty.objectReferenceValue);
                    }
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5); // Add space between prefabs

                if (i % 2 == 1)
                {
                    EditorGUILayout.EndHorizontal(); // End the horizontal group for every two prefabs
                }
            }
            if (prefabsProperty.arraySize % 2 != 0)
            {
                EditorGUILayout.EndHorizontal(); // Ensure the last row ends properly if it's an odd number of prefabs
            }
            EditorGUILayout.EndVertical();
        }

        serializedPrefabManager.ApplyModifiedProperties();
    }

    private void PingObject(Object obj)
    {
        if (obj != null)
        {
            EditorGUIUtility.PingObject(obj);
        }
    }

    private void CreatePrefabManager()
    {
        TXRPrefabManager newPrefabManager = ScriptableObject.CreateInstance<TXRPrefabManager>();

        // Ensure the directory exists
        string directoryPath = Path.GetDirectoryName(prefabManagerPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        AssetDatabase.CreateAsset(newPrefabManager, prefabManagerPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        LoadPrefabManager();
    }

    private void SpawnPrefab(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Cannot spawn prefab. Prefab reference is null.");
            return;
        }

        Scene baseScene = SceneManager.GetSceneByName("Base Scene");

        if (baseScene.isLoaded && SceneManager.sceneCount == 1)
        {
            EditorUtility.DisplayDialog("Info", "Base Scene should not be modified :)", "OK");
            return;
        }

        // Find a scene other than the base scene
        Scene targetScene = default;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded && scene.name != "Base Scene")
            {
                targetScene = scene;
                break;
            }
        }

        if (!targetScene.IsValid())
        {
            EditorUtility.DisplayDialog("Info", "No suitable scene found to spawn the prefab.", "OK");
            return;
        }

        // Set the target scene as the active scene
        SceneManager.SetActiveScene(targetScene);

        // Spawn the prefab in the target scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, targetScene);
        Undo.RegisterCreatedObjectUndo(instance, "Spawn Prefab");
        Selection.activeObject = instance;
    }
}
