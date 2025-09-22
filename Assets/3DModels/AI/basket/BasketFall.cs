using UnityEngine;

public class BasketFall : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private Vector3 moveDirection = Vector3.forward;
    [SerializeField] private GameObject playerObject; // Assign your player here

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool shouldMove = false;
    private float moveProgress = 0f;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition + moveDirection.normalized * moveDistance;
    }

    void Update()
    {
        if (shouldMove && moveProgress < 1f)
        {
            moveProgress += Time.deltaTime * moveSpeed;
            moveProgress = Mathf.Clamp01(moveProgress);
            transform.position = Vector3.Lerp(initialPosition, targetPosition, moveProgress);
        }
    }

    // Call this to trigger the basket fall
    public void TriggerFall()
    {
        if (!shouldMove)
        {
            Debug.Log("Basket triggered by player!");
            shouldMove = true;
        }
    }
}

