using UnityEngine;
using TMPro;

public class FinishLine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private MonoBehaviour[] cameraScripts;
    [SerializeField] private TextMeshProUGUI completionText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"FinishLine triggered. Checkpoints: {GameState.CurrentCheckpointIndex}/{GameState.TotalCheckpoints}");

            // Only complete level if all checkpoints passed
            if (GameState.CurrentCheckpointIndex >= GameState.TotalCheckpoints)
            {
                GameState.CompleteLevel();

                // Freeze camera
                Vector3 camPos = mainCamera.transform.position;
                Quaternion camRot = mainCamera.transform.rotation;

                foreach (var script in cameraScripts)
                    if (script != null) script.enabled = false;

                mainCamera.transform.SetPositionAndRotation(camPos, camRot);

                // Show completed text
                if (completionText != null)
                    completionText.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("Cannot finish yet: not all checkpoints passed.");
            }
        }
    }
}
