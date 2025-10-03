using UnityEngine;

public class ProceduralCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform player;   // Drag the marble/player here in Inspector

    private Vector3 offset;    // The initial relative position

    void Start()
    {
        // Record the initial relative offset between camera and player
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Keep the same relative offset
        transform.position = player.position + offset;
    }
}
