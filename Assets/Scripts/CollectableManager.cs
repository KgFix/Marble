using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    private void Start()
    {
        // Count all child collectables in the hierarchy
        int total = GetComponentsInChildren<Collectable>().Length;

        // Reset GameState collectables
        GameState.ResetCollectables(total);
    }
}
