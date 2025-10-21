using System.Linq;
using UnityEngine;

public static class SceneUtil
{
    /// <summary>
    /// Finds a component of type T. If not found and includeInactive is true, searches inactive objects in loaded scenes.
    /// </summary>
    public static T FindInScene<T>(bool includeInactive = false) where T : Component
    {
        var found = Object.FindObjectOfType<T>();
        if (found != null || !includeInactive)
            return found;

        // Fallback: search inactive objects but only in loaded scenes, skip assets/prefabs
        var all = Resources.FindObjectsOfTypeAll<T>();
        foreach (var candidate in all)
        {
            if (candidate == null) continue;
            var go = candidate.gameObject;
            if (go == null) continue;
            if (go.hideFlags != HideFlags.None) continue; // exclude prefabs/assets
            var scene = go.scene;
            if (!scene.IsValid() || !scene.isLoaded) continue;
            return candidate;
        }
        return null;
    }

    /// <summary>
    /// Finds the first MonoBehaviour whose type name matches exactly, optionally including inactive objects in loaded scenes.
    /// </summary>
    public static MonoBehaviour FindMonoBehaviourByTypeName(string typeName, bool includeInactive = false)
    {
        if (string.IsNullOrEmpty(typeName)) return null;

        // Fast path: active objects only
        if (!includeInactive)
        {
            var allActive = Object.FindObjectsOfType<MonoBehaviour>();
            foreach (var mb in allActive)
            {
                if (mb != null && mb.GetType().Name == typeName)
                    return mb;
            }
            return null;
        }

        // Include inactive: scan loaded scenes only
        var all = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
        foreach (var mb in all)
        {
            if (mb == null) continue;
            if (mb.GetType().Name != typeName) continue;
            var go = mb.gameObject;
            if (go == null) continue;
            if (go.hideFlags != HideFlags.None) continue; // exclude assets/prefabs
            var scene = go.scene;
            if (!scene.IsValid() || !scene.isLoaded) continue;
            return mb;
        }
        return null;
    }
}
