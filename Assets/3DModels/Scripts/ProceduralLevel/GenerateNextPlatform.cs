using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GenerateNextPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public GameObject[] platformPrefabs; // Assign in Inspector
    public Transform platformParent;     // Assign in Inspector
    public float platformLength = 100f;  // Length along Z
    public int numberOfPlatforms = 5;    // Set this in Inspector

    [Header("Special Pieces")]
    public GameObject startPiece;        // Assign Start piece in Inspector
    public GameObject endPiece;          // Assign End piece in Inspector

    private Transform lastPlatform;
    private Queue<GameObject> spawnedPlatforms = new Queue<GameObject>();

    void Start()
    {
        lastPlatform = transform;
        GeneratePlatforms();
    }

    void GeneratePlatforms()
    {
        if (startPiece == null || endPiece == null)
        {
            Debug.LogWarning("Start or End piece not assigned.");
            return;
        }

        // Place Start piece
        Vector3 startPos = transform.position;
        GameObject firstPlatform = Instantiate(startPiece, startPos, lastPlatform.rotation, platformParent);
        spawnedPlatforms.Enqueue(firstPlatform);
        lastPlatform = firstPlatform.transform;

        // Get Start's end letter (second segment)
        string[] startParts = startPiece.name.Split('.');
        if (startParts.Length < 3)
        {
            Debug.LogWarning($"Start piece name '{startPiece.name}' does not match expected format (A.B.Variant).");
            return;
        }
        string currentEnd = startParts[1];

        // Place middle pieces
        for (int i = 0; i < numberOfPlatforms - 2; i++)
        {
            GameObject chosenPrefab = PickPrefabWithStart(currentEnd);
            if (chosenPrefab == null)
            {
                Debug.LogWarning("No suitable platform prefab found. Skipping this platform.");
                continue;
            }

            string[] parts = chosenPrefab.name.Split('.');
            if (parts.Length < 3)
            {
                Debug.LogWarning($"Prefab name '{chosenPrefab.name}' does not match expected format (A.B.Variant). Skipping this platform.");
                continue;
            }

            currentEnd = parts[1]; // Use the second segment as the new end

            Vector3 spawnPos = lastPlatform.position + new Vector3(0, 0, platformLength);
            GameObject newPlatform = Instantiate(chosenPrefab, spawnPos, lastPlatform.rotation, platformParent);

            spawnedPlatforms.Enqueue(newPlatform);
            lastPlatform = newPlatform.transform;

            if (spawnedPlatforms.Count > 8)
            {
                GameObject oldPlatform = spawnedPlatforms.Dequeue();
                Destroy(oldPlatform);
            }
        }

        // Place End piece
        Vector3 endSpawnPos = lastPlatform.position + new Vector3(0, 0, platformLength);
        GameObject lastPlatformObj = Instantiate(endPiece, endSpawnPos, lastPlatform.rotation, platformParent);
        spawnedPlatforms.Enqueue(lastPlatformObj);
        lastPlatform = lastPlatformObj.transform;

        if (spawnedPlatforms.Count > 8)
        {
            GameObject oldPlatform = spawnedPlatforms.Dequeue();
            Destroy(oldPlatform);
        }
    }


    GameObject PickPrefabWithStart(string start)
    {
        var candidates = platformPrefabs.Where(p =>
        {
            var parts = p.name.Split('.');
            // Match only if there are at least three segments and the first segment matches 'start'
            return parts.Length >= 3 && parts[0] == start;
        }).ToArray();

        // If no candidates, pick any prefab as fallback
        if (candidates.Length == 0)
        {
            Debug.LogWarning($"No prefab starts with '{start}'. Picking any prefab as fallback.");
            return PickAnyPrefab();
        }
        return candidates[Random.Range(0, candidates.Length)];
    }

    GameObject PickAnyPrefab()
    {
        if (platformPrefabs.Length == 0) return null;
        return platformPrefabs[Random.Range(0, platformPrefabs.Length)];
    }
}
