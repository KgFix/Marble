using UnityEngine;

public class Lose : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // You can restrict to specific objects/tags if needed, e.g.:
        // if (!other.CompareTag("Player")) return;

        if (!GameState.IsCompleted)
        {
            Debug.Log("Lose trigger activated! End level.");
            GameState.SetVictory(false);
            GameState.CompleteLevel();

            float failTime = GameState.LevelTime;
            var endScreen = FindObjectOfType<EndScreenUI3>(includeInactive: true);
            if (endScreen != null)
                endScreen.ShowEndScreen(false, failTime);
            else if (!EndScreenHelper.TryShowEndScreen())
                Debug.LogWarning("Lose: No End Screen UI found. Ensure an end screen exists in the scene.");
        }
    }
}
