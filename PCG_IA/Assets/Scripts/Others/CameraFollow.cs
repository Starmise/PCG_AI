using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(4.5f, 4.5f, -10f);
    public float smoothSpeed = 5f;
    public float cameraDistance = 5f;

    [Header("Collision Settings")]
    public float cameraRadius = 0.3f;
    public float minDistance = 0.8f;
    public float maxDistance = 6f;
    public LayerMask collisionLayer;

    private float currentDistance;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        currentDistance = maxDistance;
    }

    void LateUpdate()
    {
        if (!player) return;

        float height = Mathf.Lerp(1.5f, 2.5f, currentDistance / maxDistance);
        Vector3 targetOrigin = player.position + Vector3.up * height;

        Vector3 direction = (transform.forward * -1f).normalized;

        float targetDistance = maxDistance;

        RaycastHit hit;
        if (Physics.SphereCast(
            targetOrigin,
            cameraRadius,
            direction,
            out hit,
            maxDistance,
            collisionLayer))
        {
            targetDistance = Mathf.Clamp(hit.distance - 0.2f, minDistance, maxDistance);
        }

        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * smoothSpeed);

        Vector3 desiredPosition = targetOrigin + direction * currentDistance;
        transform.position = desiredPosition;

        transform.LookAt(targetOrigin);
    }
}
