using UnityEngine;

public class PickupItem : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el jugador tenga el tag "Player"
        {
            KeyManager.instance.AddKey(); // Sumar la llave
            Destroy(gameObject); // Destruir la llave
        }
    }
}
