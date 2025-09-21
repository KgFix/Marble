using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Whatever your collect logic is
            GameState.CollectItem();

            // Disable visuals and collider immediately
            Renderer rend = GetComponent<Renderer>();
            if (rend != null) rend.enabled = false;

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Destroy object right away (no need to wait for sound)
            Destroy(gameObject);
        }
    }
}
