using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

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
            ojosPesados();
        }
    }

    private void ojosPesados()
    {
        Debug.Log("Tienes los ojos pesados...");
    }
}
