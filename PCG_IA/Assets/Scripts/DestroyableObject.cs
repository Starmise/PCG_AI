using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    // Variables numericas
    public int maxHits = 3; // Vida antes de destruirse
    private int currentHits = 0; // Contador de golpes

    /// <summary>
    /// Verifica si el objeto que colisiona es el jugador y si está atacando.  
    /// Si el jugador está atacando, se aumenta el contador de golpes, y si 
    /// se supera el máximo permitido, el objeto se destruye.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter llamado con: " + other.name);
        // Verificar si el objeto que golpea pertenece al jugador
        GameObject rootObject = other.transform.root.gameObject;

        // Verificar si el objeto que colisiona tiene la layer "Player"
        // NOTA: Para debugging se po´dría hacer una variable string que almacene el nombre de la capa
        if (rootObject.layer == LayerMask.NameToLayer("Player"))
        {
            Animator playerAnimator = rootObject.GetComponent<Animator>();

            if (playerAnimator != null && IsPlayerAttacking(playerAnimator))
            {
                Debug.Log("Oh le has pegado a la caja");
                currentHits++;

                if (currentHits >= maxHits)
                {
                    Destroy(gameObject); // Destruye el objeto (caja) tras 3 golpes
                }
            }
        }

        // Verificar si el objeto que colisiona tiene la layer "Bullet"
        else if (rootObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            Debug.Log("Oh le has disparado a la caja");
            currentHits++;

            if (currentHits >= maxHits)
            {
                Destroy(gameObject); // Destruye el objeto (caja) tras 3 golpes
            }
        }
    }

    /// <summary>
    /// Verificams si el jugador está realizando un ataque. Esto porque mira Lucio, llevo 8 días
    /// durmiendo a las 2-4 y despertando a las 7 y estresado, ya tengo flojera de programar bien
    /// las colisones así que fuck it. Si se está ejecutando alguna animación de golpe, puede dañar la caja
    /// si no pues no verdad.
    /// </summary>
    private bool IsPlayerAttacking(Animator animator)
    {
        return animator.GetBool("Attack1") || animator.GetBool("Attack2") || animator.GetBool("Attack3");
    }
}
