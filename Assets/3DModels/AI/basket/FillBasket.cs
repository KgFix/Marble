using UnityEngine;
using System.Collections.Generic;

public class FillBasket : MonoBehaviour
{
    [Header("Ball Prefabs")]
    public GameObject bowlingBallPrefab;
    public GameObject golfBallPrefab;
    public GameObject soccerBallPrefab;
    public GameObject tennisBallPrefab;
    public GameObject volleyballPrefab;
    public GameObject baseballPrefab;

    public int minBalls = 5;
    public int maxBalls = 20;
    public float ballScale = 1f;

    [Header("Spawn Area")]
    public Vector3 spawnAreaSize = new Vector3(1.5f, 1.5f, 1.5f); // Increased area

    [Tooltip("Minimum distance between balls")]
    public float minDistance = 0.3f;

    void Start()
    {
        Fill();
    }

    void Fill()
    {
        GameObject[] ballPrefabs = new GameObject[]
        {
            bowlingBallPrefab,
            golfBallPrefab,
            soccerBallPrefab,
            tennisBallPrefab,
            volleyballPrefab,
            baseballPrefab
        };

        int numberOfBalls = Random.Range(minBalls, maxBalls + 1);
        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < numberOfBalls; i++)
        {
            GameObject selectedPrefab = ballPrefabs[Random.Range(0, ballPrefabs.Length)];
            if (selectedPrefab == null)
                continue;

            Vector3 spawnPos = Vector3.zero;
            bool found = false;
            int attempts = 0;

            // Try up to 20 times to find a non-overlapping position
            while (!found && attempts < 20)
            {
                spawnPos = GetRandomPositionInBasket();
                found = true;
                foreach (var pos in usedPositions)
                {
                    if (Vector3.Distance(spawnPos, pos) < minDistance)
                    {
                        found = false;
                        break;
                    }
                }
                attempts++;
            }

            usedPositions.Add(spawnPos);
            GameObject ball = Instantiate(selectedPrefab, spawnPos, Quaternion.identity, transform);
            ball.transform.localScale = Vector3.one * ballScale;
        }
    }

    Vector3 GetRandomPositionInBasket()
    {
        Vector3 center = transform.position;
        Vector3 halfSize = spawnAreaSize * 0.5f;
        float x = Random.Range(center.x - halfSize.x, center.x + halfSize.x);
        float y = Random.Range(center.y, center.y + spawnAreaSize.y);
        float z = Random.Range(center.z - halfSize.z, center.z + halfSize.z);
        return new Vector3(x, y, z);
    }
}
