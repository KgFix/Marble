using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Finish : MonoBehaviour
{
    private bool levelEnded = false;
    [SerializeField] private EndGameScreenforlevel2 level2EndScreen; // optional reference

    private void Awake()
    {
        if (level2EndScreen == null)
            level2EndScreen = SceneUtil.FindInScene<EndGameScreenforlevel2>(includeInactive: true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (levelEnded || GameState.IsCompleted)
            return;

        if (other.CompareTag("Player"))
        {
            levelEnded = true;
            // Mark victory and show Level 2 end screen (time-only stars)
            GameState.SetVictory(true);
            GameState.CompleteLevel();

            if (level2EndScreen == null)
                level2EndScreen = SceneUtil.FindInScene<EndGameScreenforlevel2>(includeInactive: true);

            if (level2EndScreen != null)
            {
                level2EndScreen.ShowEndScreen(GameState.LevelTime, true);
            }
            else if (EndLevelUIManager.Instance != null)
            {
                // Fallback to legacy manager
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
