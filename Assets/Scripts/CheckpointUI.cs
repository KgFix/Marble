using UnityEngine;
using TMPro;

public class CheckpointUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI CheckpointText;

    private void Start()
    {
        // set initial text
        UpdateCheckpointText();
    }

    private void OnEnable()
    {
        // subscribe to coin updates if you add events later
    }

    private void Update()
    {
        // continuously update text (simple approach)
        UpdateCheckpointText();
    }

    void UpdateCheckpointText()
    {
            int count = GameState.CurrentCheckpointIndex;
            int total = GameState.TotalCheckpoints;
            CheckpointText.text = $"{count}/{total}";
    }
}
