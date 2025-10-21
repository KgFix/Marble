using System.Reflection;
using UnityEngine;

public static class EndScreenHelper
{
    /// <summary>
    /// Attempts to show the end screen. Tries EndScreenUI first, then a component named 'GameEndScreen'.
    /// Returns true if a screen was shown.
    /// </summary>
    public static bool TryShowEndScreen()
    {
        // 1) Try typed EndScreenUI
        var ui = SceneUtil.FindInScene<EndScreenUI>(includeInactive: true);
        if (ui != null)
        {
            ui.ShowEndScreen();
            return true;
        }

        // 2) Try by type name: GameEndScreen
        var mb = SceneUtil.FindMonoBehaviourByTypeName("GameEndScreen", includeInactive: true);
        if (mb != null)
        {
            // Prefer direct reflection
            var method = mb.GetType().GetMethod("ShowEndScreen", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(mb, null);
                return true;
            }

            // Fallback: SendMessage (won't error with DontRequireReceiver)
            mb.gameObject.SendMessage("ShowEndScreen", SendMessageOptions.DontRequireReceiver);
            return true; // assume handled by the component
        }

        return false;
    }
}
