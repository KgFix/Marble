using UnityEngine;

public class GameManager3 : MonoBehaviour
{
    private bool hasHandledEnd = false;

    public void Win()
    {
        float levelTime = GameState.LevelTime;
        if (EndScreenUI3.Instance != null)
            EndScreenUI3.Instance.ShowEndScreen(true, levelTime);

        Debug.Log("GameManager3: Player won.");
        // Add any additional logic for winning here
    }

    public void Lose()
    {
        float levelTime = GameState.LevelTime;
        if (EndScreenUI3.Instance != null)
            EndScreenUI3.Instance.ShowEndScreen(false, levelTime);

        Debug.Log("GameManager3: Player lost.");
        // Add any additional logic for losing here
    }
}
