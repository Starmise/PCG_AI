using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public bool immortal = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("El jugador tomó daño. Salud actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            if (immortal)
            {
                //currentHealth = 0; // No muere, pero la salud se queda en 0
                Debug.Log("El jugador es inmortal y no puede morir.");
            }
            else
            {
                ojosPesados();
            }
        }
    }

    private void ojosPesados()
    {
        Debug.Log("Tienes los ojos pesados...");
        CanvasManager canvasManager = FindFirstObjectByType<CanvasManager>();
        if (canvasManager != null)
        {
            canvasManager.deathScreen();
        }
    }
}
