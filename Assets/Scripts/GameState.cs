using UnityEngine;

public class GameState : MonoBehaviour
{
    // Static so all scripts can check this
    public static bool IsCompleted { get; private set; } = false;

    public static void CompleteLevel()
    {
        IsCompleted = true;
        Debug.Log("Game Completed!");
    }

    // Optional: reset between play sessions
    public static void ResetGame()
    {
        IsCompleted = false;
    }
}
