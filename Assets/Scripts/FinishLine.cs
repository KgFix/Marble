using UnityEngine;
using TMPro;

public class FinishLine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;               // Drag your actual Camera here
    [SerializeField] private MonoBehaviour[] cameraScripts;   // Drag CamX + CamY scripts here
    [SerializeField] private TextMeshProUGUI completionText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Finish Line Reached!");

            // Mark level as completed
            GameState.CompleteLevel();

            // Freeze camera in place
            Vector3 camPos = mainCamera.transform.position;
            Quaternion camRot = mainCamera.transform.rotation;

            foreach (var script in cameraScripts)
            {
                if (script != null) script.enabled = false;
            }

            mainCamera.transform.SetPositionAndRotation(camPos, camRot);

            // Show COMPLETED text
            if (completionText != null)
                completionText.gameObject.SetActive(true);
        }
    }
}
