using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Finish : MonoBehaviour
{
    private bool levelEnded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (levelEnded)
            return;

        if (other.CompareTag("Player"))
        {
            levelEnded = true;
            if (EndLevelUIManager.Instance != null)
            {
                EndLevelUIManager.Instance.ShowEndScreen(GameState.LevelTime);
            }
        }
    }

    public void OnRetryButton()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void OnExitButton()
    {
        SceneManager.LoadScene("LevelMenu");
    }
}
