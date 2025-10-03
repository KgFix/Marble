using UnityEngine;

public class GenerateNextPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public GameObject[] platformPrefabs; // Drag prefabs here in Inspector
    public Transform platformParent;     // Empty parent for organization
    public float platformLength = 100f;  // Length of each platform along Z

    private Transform lastPlatform;

    void Start()
    {
        // Assume the first platform is already placed in the scene
        lastPlatform = transform;
        SpawnNextPlatform();
    }

    public void SpawnNextPlatform()
    {
        // Pick a random prefab
        int index = Random.Range(0, platformPrefabs.Length);
        GameObject chosenPrefab = platformPrefabs[index];

        // Calculate spawn position at END of last platform
        Vector3 spawnPos = lastPlatform.position + lastPlatform.forward * platformLength;

        // Spawn platform
        GameObject newPlatform = Instantiate(chosenPrefab, spawnPos, lastPlatform.rotation, platformParent);

        // Update last platform reference
        lastPlatform = newPlatform.transform;
    }
}
