using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables numéricas
    public float moveSpeed = 5f; // Velocidad del personaje
    private Vector3 moveDirection; // Dirección a la que se mueve el personaje
    public float attackDamage = 10f;

    private float nextFireTime = 0f;
    public static int numClicks = 0;
    private float lastClickedTime = 0f;
    private float maxComboDelay = 1;

    // Booleanos
    public bool isAttacking;

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

        // Había olvidado desactivar el estado de ataque xd
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime > 0.2f && (stateInfo.IsName("Idle") || stateInfo.IsName("Standard Run")))
        {
            isAttacking = false;
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

    /// <summary>
    /// Controla la lógica de los ataques y la gestión del combo.
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
    /// Gestiona la ejecución del combo basado en la cantidad de clicks y el estado de la animación.
    /// </summary>
    private void ProcessCombo()
    {
        lastClickedTime = Time.time;
        numClicks++;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        isAttacking = true;

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

    /// <summary>
    /// Detecta colisiones con los enemigos para golpearlos a puño limpio. 
    /// A diferencia de Enemy, el jugador detecta la colisión en cuanto 
    /// se entre en contacto, NO mientras esté en contacto con el otro objeto.
    /// Se llama al método de TakeDamage para dañar el enemigo.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && isAttacking)
        {
            EnemyView enemyView = other.GetComponent<EnemyView>();
            if (enemyView != null)
            {
                enemyView.TakeDamage(attackDamage);
            }
        }
    }
}