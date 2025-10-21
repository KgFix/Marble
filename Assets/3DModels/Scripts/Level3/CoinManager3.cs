using UnityEngine;
using System.Collections.Generic;

public class CoinManager3 : MonoBehaviour
{
    public static CoinManager3 Instance { get; private set; }

    private List<Collectable> allCollectables = new List<Collectable>();
    private int collectedCount = 0;
    private int collectedValue = 0;

    // BossHealth is set to the number of collectables at Start
    public int BossHealth { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        allCollectables.Clear();
        allCollectables.AddRange(GetComponentsInChildren<Collectable>());
        collectedCount = 0;
        collectedValue = 0;
        BossHealth = allCollectables.Count;
    }

    public void Collect(Collectable collectable)
    {
        if (allCollectables.Contains(collectable))
        {
            collectedCount++;
            collectedValue += collectable.collectableValue;
        }
    }

    public int TotalCollectables => allCollectables.Count;
    public int CollectedCount => collectedCount;
    public int CollectedValue => collectedValue;
}
