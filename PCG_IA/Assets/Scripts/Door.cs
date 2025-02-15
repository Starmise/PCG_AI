using UnityEngine;

public class Door : MonoBehaviour
{
    private bool isUnlocked = false; // Indica si la puerta ha sido abierta

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Si el jugador choca con la puerta
        {
            if (KeyManager.instance.HasKey()) // Si el jugador tiene al menos 1 llave
            {
                KeyManager.instance.UseKey(); // Resta una llave
                Destroy(gameObject); // Destruye la puerta
                isUnlocked = true;
                Debug.Log("Puerta abierta");
            }
            else
            {
                Debug.Log("No tienes llaves suficientes para abrir esta puerta.");
            }
        }
    }
}