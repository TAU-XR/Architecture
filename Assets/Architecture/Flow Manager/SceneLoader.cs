using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad = "Scene2";  // The additional scene to load

    void Start()
    {
        // Prevent the scene from loading when running in the Unity Editor
        if (!Application.isEditor)
        {
            // Load the second scene additively without touching the current scene
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        }
    }
}
