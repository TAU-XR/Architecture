using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad = "Scene2";  // The additional scene to load

    void Start()
    {
        // Load the second scene additively without touching the current scene
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
    }
}
