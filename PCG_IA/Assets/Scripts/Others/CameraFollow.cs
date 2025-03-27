using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(4.5f, 4.5f, -10f);
    public float smoothSpeed = 5f;
    public float cameraDistance = 5f;
    public LayerMask collisionLayers;

    private Vector3 desiredPosition;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = player.position + offset;

            // Raycast para detectar colisiones de la cámraa
            RaycastHit hit;
            if (Physics.Raycast(player.position, (targetPosition - player.position).normalized, out hit, cameraDistance, collisionLayers))
            {
                targetPosition = hit.point + hit.normal * 0.2f;
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            transform.LookAt(player); // La cámara siempre mira hacia el jugador, como vimos en gráficas LookAt es el que actualiza la matriz de vista
        }
    }
}
