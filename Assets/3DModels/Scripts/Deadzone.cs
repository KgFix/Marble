using UnityEngine;
using TMPro;

public class Deadzone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private MonoBehaviour[] cameraScripts;
    [SerializeField] private TextMeshProUGUI completionText;
    [SerializeField] private EndScreenUI endScreenUI;

    private void Awake()
    {
        // Auto-wire common refs if forgotten in Inspector
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (endScreenUI == null)
            endScreenUI = SceneUtil.FindInScene<EndScreenUI>(includeInactive: true);

        if (endScreenUI == null)
            Debug.LogWarning("Deadzone: EndScreenUI reference not set and not found in scene. End screen may not show.");
        if (mainCamera == null)
            Debug.LogWarning("Deadzone: Main Camera reference not set and Camera.main not found. Camera freeze will be skipped.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameState.IsCompleted) return; // already ended, ignore further triggers
        if (other.CompareTag("Player"))
        {
            Debug.Log("Deadzone triggered.");
            GameState.SetVictory(false);
            GameState.CompleteLevel();

            // Freeze camera
            if (mainCamera != null)
            {
                Vector3 camPos = mainCamera.transform.position;
                Quaternion camRot = mainCamera.transform.rotation;

                if (cameraScripts != null)
                {
                    foreach (var script in cameraScripts)
                        if (script != null) script.enabled = false;
                }

                mainCamera.transform.SetPositionAndRotation(camPos, camRot);
            }

            // Show completed text
            if (completionText != null)
            {
                completionText.text = "You Died!";
                completionText.gameObject.SetActive(true);
            }

            if (endScreenUI != null)
            {
                endScreenUI.ShowEndScreen();
            }
            else if (!EndScreenHelper.TryShowEndScreen())
            {
                Debug.LogWarning("Deadzone: Cannot show end screen because EndScreenUI/GameEndScreen is missing.");
            }
        }
    }
}
