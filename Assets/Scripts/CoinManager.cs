using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI References")]
    public Text coinCountText; // For legacy UI
    public TMPro.TextMeshProUGUI coinCountTMP; // For TextMeshPro (recommended)
    public Image coinIcon; // NEW: Coin icon for enhanced UI

    [Header("Coin Data")]
    public int totalCoins = 0;
    public int coinsCollected = 0;

    [Header("UI Animation Settings")]
    public float iconScaleMultiplier = 1.3f;
    public float animationDuration = 0.2f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Count total coins in the scene
        CountTotalCoins();
        UpdateUI();
    }

    public void CollectCoin(int value = 1)
    {
        coinsCollected += value;
        UpdateUI();

        // Animate the coin icon when collected
        if (coinIcon != null)
        {
            StartCoroutine(AnimateCoinCollection());
        }

        // Optional: Check if all coins collected
        if (coinsCollected >= totalCoins)
        {
            OnAllCoinsCollected();
        }
    }

    IEnumerator AnimateCoinCollection()
    {
        if (coinIcon == null) yield break;

        // Mario-style bounce animation
        Vector3 originalScale = coinIcon.transform.localScale;
        Vector3 bounceScale = originalScale * 1.4f;

        // Quick bounce up
        float bounceTime = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < bounceTime)
        {
            float progress = elapsedTime / bounceTime;
            coinIcon.transform.localScale = Vector3.Lerp(originalScale, bounceScale, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Quick bounce down
        elapsedTime = 0f;
        while (elapsedTime < bounceTime)
        {
            float progress = elapsedTime / bounceTime;
            coinIcon.transform.localScale = Vector3.Lerp(bounceScale, originalScale, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        coinIcon.transform.localScale = originalScale;
    }

    void UpdateUI()
    {
        string coinText = coinsCollected + " / " + totalCoins;

        if (coinCountText != null)
            coinCountText.text = coinText;

        if (coinCountTMP != null)
            coinCountTMP.text = coinText;
    }

    IEnumerator AnimateCoinIcon()
    {
        if (coinIcon == null) yield break;

        Vector3 originalScale = coinIcon.transform.localScale;
        Vector3 targetScale = originalScale * iconScaleMultiplier;

        float elapsedTime = 0f;

        // Scale up then down
        while (elapsedTime < animationDuration)
        {
            float progress = elapsedTime / animationDuration;
            float scaleValue = scaleCurve.Evaluate(progress);

            coinIcon.transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it returns to original scale
        coinIcon.transform.localScale = originalScale;
    }

    void CountTotalCoins()
    {
        Coin[] allCoins = FindObjectsOfType<Coin>();
        totalCoins = 0;

        foreach (Coin coin in allCoins)
        {
            totalCoins += coin.coinValue;
        }
    }

    void OnAllCoinsCollected()
    {
        Debug.Log("All coins collected!");

        // Optional: Flash the UI when all coins collected
        if (coinIcon != null)
        {
            StartCoroutine(FlashCompleteUI());
        }
    }

    IEnumerator FlashCompleteUI()
    {
        Color originalColor = coinIcon.color;
        Color flashColor = Color.yellow;

        for (int i = 0; i < 3; i++)
        {
            coinIcon.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            coinIcon.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Public method to reset coins (useful for restarting levels)
    public void ResetCoins()
    {
        coinsCollected = 0;
        CountTotalCoins();
        UpdateUI();
    }
}