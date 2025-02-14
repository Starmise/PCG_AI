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

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f
           && animator.GetCurrentAnimatorStateInfo(0).IsName("1stPunch"))
        {
            animator.SetBool("Attack1", false);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f
           && animator.GetCurrentAnimatorStateInfo(0).IsName("2ndPunch"))
        {
            animator.SetBool("Attack2", false);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f
           && animator.GetCurrentAnimatorStateInfo(0).IsName("Kick"))
        {
            animator.SetBool("Attack3", false);
            numClicks = 0;
        }

        // Comprobar que se pueda realizar el combo y que el personaje pueda atacar
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            numClicks = 0;
        }
        if (Time.time > nextFireTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnClick();
            }
        }
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
    /// Al dar click el jugador har� la animaci�n de atacar, donde depndiendo del n�mero
    /// de clicks, har� una animaci�n diferente para un combo.
    /// </summary>
    void OnClick()
    {
        lastClickedTime = Time.time;
        numClicks++;

        // Un click = primer ataque
        if (numClicks == 1)
        {
            animator.SetBool("Attack1", true);
        }

        // Dos clicks y la animaci�n se encuentra en el primer ataque = segundo ataque
        if (numClicks >= 2 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f
           && animator.GetCurrentAnimatorStateInfo(0).IsName("1stPunch"))
        {
            animator.SetBool("Attack1", false);
            animator.SetBool("Attack2", true);
        }

        // Tres clicks y la animaci�n se encuentra en el segundo ataque = tercer ataque
        if (numClicks >= 3 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f
           && animator.GetCurrentAnimatorStateInfo(0).IsName("2ndPunch"))
        {
            animator.SetBool("Attack2", false);
            animator.SetBool("Attack3", true);
        }
    }
}