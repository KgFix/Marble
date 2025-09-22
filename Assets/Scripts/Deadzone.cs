using UnityEngine;
using TMPro;

public class Deadzone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private MonoBehaviour[] cameraScripts;
    [SerializeField] private TextMeshProUGUI completionText;
    [SerializeField] private EndScreenUI endScreenUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Deadzone triggered.");

            GameState.CompleteLevel();

            // Freeze camera
            Vector3 camPos = mainCamera.transform.position;
            Quaternion camRot = mainCamera.transform.rotation;

            foreach (var script in cameraScripts)
                if (script != null) script.enabled = false;

            mainCamera.transform.SetPositionAndRotation(camPos, camRot);

            // Show completed text
            if (completionText != null)
            {
                completionText.text = "You Died!";
                completionText.gameObject.SetActive(true);
            }

            if (endScreenUI != null)
                endScreenUI.ShowEndScreen();
        }
    }
}
