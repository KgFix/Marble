using UnityEngine;

public class SpotlightFollower : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player; // Assign your marble GameObject here

    [Header("Spotlight Offset")]
    public float xOffset = 5f; // Distance from player's X position

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 newPosition = transform.position;
        newPosition.x = player.position.x + xOffset;
        newPosition.y = player.position.y;
        newPosition.z = player.position.z;
        transform.position = newPosition;
    }
}
