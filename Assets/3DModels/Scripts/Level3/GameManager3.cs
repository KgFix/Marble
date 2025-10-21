using UnityEngine;

public class GameManager3 : MonoBehaviour
{
    private bool hasHandledLoss = false;

    void Update()
    {
        // Detect loss: Level is completed and not a victory
        if (!hasHandledLoss && GameState.IsCompleted && !GameState.IsVictory)
        {
            hasHandledLoss = true;
            OnPlayerLost();
        }
    }

    private void OnPlayerLost()
    {
        Debug.Log("GameManager3: Player lost (detected from GameState).");
        // Add your custom loss handling logic here (UI, analytics, restart, etc.)
    }
}
