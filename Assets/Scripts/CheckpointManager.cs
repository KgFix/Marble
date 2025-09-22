using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [Header("Assign checkpoints in order")]
    [SerializeField] private Checkpoint[] checkpoints;

    private void Awake()
    {
        // Automatically find all Checkpoint components in children, in hierarchy order
        checkpoints = GetComponentsInChildren<Checkpoint>();

        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].checkpointIndex = i;
        }

        GameState.Reset(checkpoints.Length);

        Debug.Log($"CheckpointManager initialized with {checkpoints.Length} checkpoints.");
    }


}
