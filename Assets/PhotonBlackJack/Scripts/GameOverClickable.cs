using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverClickable : MonoBehaviour
{
    private string sceneToLoad;

    public void SetSceneToLoad(string sceneName)
    {
        sceneToLoad = sceneName;
    }

    private void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene to load is not set for GameOverClickable.");
        }
    }
}