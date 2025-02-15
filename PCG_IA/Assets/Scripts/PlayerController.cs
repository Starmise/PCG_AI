using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables num�ricas
    public float moveSpeed = 5f; // Velocidad del personaje
    private Vector3 moveDirection; // Direcci�n a la que se mueve el personaje

    private float nextFireTime = 0f;
    public static int numClicks = 0;
    private float lastClickedTime = 0f;
    private float maxComboDelay = 1;

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

    void Update()
    {
        HandleMovementInput();
        HandleAttackLogic();
    }

    /// <summary>
    /// Se aplica el movimiento del personale para que la velocidad del jugador se aplique
    /// en la direcci�n correcta. Se hace en FixedUpdate en lugar de Update para que la
    /// actualizaci�n sea constante, ya que este tiene una tasa de actualizaci�n fija,
    /// mientras que Update depende de los frames del juego.
    /// </summary>
    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    /// <summary>
    /// Maneja la entrada del jugador para moverse y actualizar la animaci�n.
    /// </summary>
    private void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Calcular la direcci�n a la que se mueve
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // Cambia la animaci�n a correr
        bool isRunning = moveDirection.magnitude > 0;
        animator.SetBool("isRunning", isRunning);

        // Rotaci�n del personahe a donde se ande moviendo
        if (isRunning)
        {
            transform.forward = moveDirection;
        }
    }

    /// <summary>
    /// Controla la l�gica de los ataques y la gesti�n del combo.
    /// </summary>
    private void HandleAttackLogic()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(0);

        if (stateInfo.normalizedTime > 0.7f || nextStateInfo.fullPathHash != 0)
        {
            if (stateInfo.IsName("1stPunch"))
                animator.SetBool("Attack1", false);
            else if (stateInfo.IsName("2ndPunch"))
                animator.SetBool("Attack2", false);
            else if (stateInfo.IsName("Kick"))
            {
                animator.SetBool("Attack3", false);
                numClicks = 0;
            }
        }

        if (Time.time - lastClickedTime > maxComboDelay)
        {
            numClicks = 0;
        }

        if (Time.time > nextFireTime && Input.GetMouseButtonDown(0))
        {
            ProcessCombo();
        }
    }

    /// <summary>
    /// Gestiona la ejecuci�n del combo basado en la cantidad de clicks y el estado de la animaci�n.
    /// </summary>
    private void ProcessCombo()
    {
        lastClickedTime = Time.time;
        numClicks++;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (numClicks == 1)
        {
            animator.SetBool("Attack1", true);
        }
        else if (numClicks >= 2 && (stateInfo.normalizedTime > 0.5f || stateInfo.IsName("1stPunch")))
        {
            animator.SetBool("Attack1", false);
            animator.SetBool("Attack2", true);
        }
        else if (numClicks >= 3 && (stateInfo.normalizedTime > 0.5f || stateInfo.IsName("2ndPunch")))
        {
            animator.SetBool("Attack2", false);
            animator.SetBool("Attack3", true);
        }
    }
}