using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        // save current scene before switching
        SceneTracker.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadPreviousScene()
    {
        if (!string.IsNullOrEmpty(SceneTracker.previousScene))
            SceneManager.LoadScene(SceneTracker.previousScene);
        else
            Debug.LogWarning("No previous scene stored!");
    }
}
