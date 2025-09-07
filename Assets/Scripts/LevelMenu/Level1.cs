using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PanelSceneLoader : MonoBehaviour, IPointerClickHandler
{
    [Header("Scene to Load on Click")]
    [SerializeField] private string sceneName;

    // This is called when the panel (or its children) is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("No scene name set on " + gameObject.name);
        }
    }
}
