using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    private void Start()
    {
        // set initial text
        UpdateCoinText();
    }

    private void OnEnable()
    {
        // subscribe to coin updates if you add events later
    }

    private void Update()
    {
        // continuously update text (simple approach)
        UpdateCoinText();
    }

    void UpdateCoinText()
    {
        if (CollectableManager.Instance != null)
        {
            int count = CollectableManager.Instance.CollectedCount;
            int total = CollectableManager.Instance.TotalCollectables;
            coinText.text = $"x {count}";
        }
    }
}
