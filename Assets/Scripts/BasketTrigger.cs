using UnityEngine;

public class BasketTrigger : MonoBehaviour
{
    [SerializeField] private BasketFall basketFall; // Assign the BasketFall script in Inspector
    [SerializeField] private GameObject playerObject; // Assign your player here

    void OnCollisionEnter(Collision collision)
    {
        if (playerObject != null && collision.gameObject == playerObject)
        {
            basketFall.TriggerFall();
        }
    }
}

