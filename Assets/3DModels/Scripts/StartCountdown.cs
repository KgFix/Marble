using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("Timing")]
    [Tooltip("Delay between numbers (in seconds)")]
    [SerializeField] private float delayBetweenNumbers = 1f;

    // optional event if other scripts want to subscribe
    public static System.Action OnCountdownComplete;

    private void Start()
    {
        // Ensure text visible at start if assigned
        if (countdownText != null) countdownText.gameObject.SetActive(true);
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        int count = 3;

        while (count > 0)
        {
            if (countdownText != null) countdownText.text = count.ToString();
            yield return new WaitForSeconds(delayBetweenNumbers);
            count--;
        }

        if (countdownText != null) countdownText.text = "GO!";

        // start the timer automatically if present
        var timer = FindObjectOfType<LevelTimer>();
        if (timer != null)
        {
            timer.ResetTimer();
            timer.StartTimer();
        }

        // small pause so player sees GO!
        yield return new WaitForSeconds(0.5f);

        if (countdownText != null) countdownText.gameObject.SetActive(false);

        // notify listeners
        OnCountdownComplete?.Invoke();
    }
}
