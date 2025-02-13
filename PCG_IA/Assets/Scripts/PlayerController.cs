using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables numéricas
    public float moveSpeed = 5f; // Velocidad del personaje
    private Vector3 moveDirection; // Dirección a la que se mueve el personaje

    // Referencias
    private Rigidbody rb;
    private Animator animator;

    /// <summary>
    /// Se obtienen los componentes del personaje al iniciar la escena
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Calcular la dirección a la que se mueve
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // Cambia la animación a correr
        bool isRunning = moveDirection.magnitude > 0;
        animator.SetBool("isRunning", isRunning);

        // Rotación del personahe a donde se ande moviendo
        if (isRunning)
        {
            transform.forward = moveDirection;
        }
    }

    /// <summary>
    /// Se aplica el movimiento del personale para que la velocidad del jugador se aplique
    /// en la dirección correcta. Se hace en FixedUpdate en lugar de Update para que la
    /// actualización sea constante, ya que este tiene una tasa de actualización fija,
    /// mientras que Update depende de los frames del juego.
    /// </summary>
    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }
}
