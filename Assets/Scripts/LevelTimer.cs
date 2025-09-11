using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    void Update()
    {
        // Update timer in GameState
        GameState.UpdateTimer(Time.deltaTime);

        // Update UI
        if (timerText != null)
        {
            float t = GameState.LevelTime;
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);
            int milliseconds = Mathf.FloorToInt((t * 1000f) % 1000f);

            timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }
}
