using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private CollectableManager collectableManager;

    [Header("Health Settings")]
    [SerializeField] private int bossMaxHealth = 10;
    [SerializeField] private bool deriveMaxFromCoins = true;
    [SerializeField] private bool useCollectableValue = false;

    private int currentHealth;

    private void Awake()
    {
        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        if (collectableManager == null)
            collectableManager = CollectableManager.Instance;

        ConfigureMaxHealth();
        if (deriveMaxFromCoins)
            TryRefreshMaxHealth();
        InitialiseSlider();
        SyncHealthBar();
    }

    private void Update()
    {
        // Lazy grab if manager not yet ready when we started.
        if (collectableManager == null)
        {
            collectableManager = CollectableManager.Instance;
            if (collectableManager == null)
                return;
        }

        if (deriveMaxFromCoins)
            TryRefreshMaxHealth();

        SyncHealthBar();
    }

    private void ConfigureMaxHealth()
    {
        if (!deriveMaxFromCoins || collectableManager == null)
        {
            bossMaxHealth = Mathf.Max(1, bossMaxHealth);
            return;
        }

        bossMaxHealth = useCollectableValue
            ? Mathf.Max(1, collectableManager.TotalCollectableValue)
            : Mathf.Max(1, collectableManager.TotalCollectables);
    }

    private void InitialiseSlider()
    {
        if (healthSlider == null)
            return;

        currentHealth = bossMaxHealth;
        healthSlider.minValue = 0f;
        healthSlider.maxValue = bossMaxHealth;
        healthSlider.value = currentHealth;
    }

    private void SyncHealthBar()
    {
        if (healthSlider == null || collectableManager == null)
            return;

        int coinsCollected = useCollectableValue
            ? collectableManager.CollectedValue
            : collectableManager.CollectedCount;

        int newHealth = Mathf.Clamp(bossMaxHealth - coinsCollected, 0, bossMaxHealth);

        if (newHealth == currentHealth)
            return;

        currentHealth = newHealth;
        healthSlider.value = currentHealth;
    }

    public void SetMaxHealth(int coinsRequired)
    {
        bossMaxHealth = Mathf.Max(1, coinsRequired);
        deriveMaxFromCoins = false;
        InitialiseSlider();
        SyncHealthBar();
    }

    private void TryRefreshMaxHealth()
    {
        if (healthSlider == null || collectableManager == null)
            return;

        int source = useCollectableValue
            ? collectableManager.TotalCollectableValue
            : collectableManager.TotalCollectables;

        source = Mathf.Max(1, source);

        if (source == bossMaxHealth)
            return;

        bossMaxHealth = source;
        healthSlider.maxValue = bossMaxHealth;
        healthSlider.value = Mathf.Clamp(healthSlider.value, 0f, bossMaxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, bossMaxHealth);
    }
}
