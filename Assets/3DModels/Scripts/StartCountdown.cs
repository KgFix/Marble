using UnityEngine;
using TMPro;
using System.Collections;

public class StartCountdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float countdownTime = 3f;

    private void Start()
    {
        
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        float time = countdownTime;

        while (time > 0)
        {
            // Show 3,2,1
            countdownText.text = Mathf.Ceil(time).ToString();
            yield return new WaitForSeconds(1f);
            time--;
        }

        // Show GO! for 1 second
        countdownText.text = "GO!";
        GameState.StartGame();
        yield return new WaitForSeconds(1f);

        // Hide text
        countdownText.gameObject.SetActive(false);
    }
}
