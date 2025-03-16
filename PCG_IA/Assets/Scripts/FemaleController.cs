using UnityEngine;

public class FemaleController : MonoBehaviour
{
    // Variables numéricas
    public float moveSpeed = 5f; // Velocidad del personaje
    private Vector3 moveDirection; // Dirección a la que se mueve el personaje

    // Booleanos

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

    /// <summary>
    /// Se usa el movimiento del personale para que la velocidad sea en la dirección correcta.
    /// </summary>
    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    /// <summary>
    /// Se ejecutan los métodos en cada frame
    /// </summary>
    void Update()
    {
        HandleMovementInput();
    }

    /// <summary>
    /// Maneja la entrada del jugador para moverse y actualizar la animación.
    /// </summary>
    private void HandleMovementInput()
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
}
