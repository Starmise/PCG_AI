using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.AI;

/// <summary>
/// En MVC, View no es como nos habían dicho de que era solo para UI, View es ya tal cual
/// la lógica de lo que el elemnto hará en Unity, como crearse, moverse, sus usos,
/// sus eventos, etc.
/// </summary>
public class EnemyView : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text enemyStatsTxt;
    public TMP_Text difficulty_txt;

    [Header("Movement")]
    public Transform player;
    // Variables para el enemigo Podadora
    public Transform puntoA;
    public Transform puntoB;
    public enum EnemyType { Escapista, Agresivo, Podadora }
    public EnemyType enemyType;

    [Header("EffectsObjects")]
    public GameObject objectPoison;
    public GameObject objectBurn;
    public GameObject objectShock;


    private EnemyController controller;
    private NavMeshAgent agent;
    private Vector3 destinoActual;
    private float attackCooldown = 0f;
    private int currentFunctionVersion = 1; // Función inicial

    /// <summary>
    /// Acá ya se crea (al menos en teoría lógica) el enemigo,con valores que ya aleatorios.
    /// De momento nadamás indicamos que se devuelve como dificultad 1 en predeterminado, 
    /// que en este caso sería la Fitness Function 1, ya en el futuro lo hago controlable
    /// desde el inspector como se pide.
    /// </summary>
    void Start()
    {
        controller = EnemyController.CreateRandomEnemy();
        agent = GetComponent<NavMeshAgent>();

        // Solo olvidaste poner que la velocidad del agente NavMesh fuera igual a la del Model
        agent.speed = controller.GetEnemyStats().Speed;

        // Asignar un tipo de enemigo aleatorio si no ha sido definido manualmente en el Inspector
        if (!Application.isEditor || enemyType == default)
        {
            enemyType = (EnemyType)Random.Range(0, System.Enum.GetValues(typeof(EnemyType)).Length);
        }

        Debug.Log("Enemigo generado: " + enemyType);

        currentFunctionVersion = Random.Range(1, 6); // Selecciona una función aleatoria entre 1 y 5

        if (enemyType == EnemyType.Podadora)
        {
            destinoActual = puntoA.position;
            agent.SetDestination(destinoActual);
        }

        // De momento vamos a instanciar un objeto diferente para cada efecto especial,
        // por el poco tiempo disponible no podemos meter VFX, y sepa dios como se hacen.
        HandleSpecialEffect();

        UpdateUI();
    }

    void Update()
    {
        // Cambiar la dificultad con los números del teclado.
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeDifficultyFunction(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeDifficultyFunction(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeDifficultyFunction(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeDifficultyFunction(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeDifficultyFunction(5);

        float detectionRange = controller.GetEnemyStats().DetectionRange;

        // Cambiar el comportamiento del enemigo dependiendo de su tipo desde el inspector.
        switch (enemyType)
        {
            case EnemyType.Escapista:

                float distancia = Vector3.Distance(player.position, transform.position);

                if (distancia < detectionRange)
                {
                    Vector3 escapeDirection = (transform.position - player.position).normalized;
                    Vector3 escapeDestination = transform.position + escapeDirection * detectionRange;

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(escapeDestination, out hit, detectionRange, NavMesh.AllAreas))
                    {
                        agent.SetDestination(hit.position);
                    }
                }
                break;
            case EnemyType.Agresivo:
                agent.SetDestination(player.position);
                break;
            case EnemyType.Podadora:
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    destinoActual = (destinoActual == puntoA.position) ? puntoB.position : puntoA.position;
                    agent.SetDestination(destinoActual);
                }
                break;
        }
    }

    void ChangeDifficultyFunction(int newFunctionVersion)
    {
        currentFunctionVersion = newFunctionVersion;
        UpdateUI();
    }

    void UpdateUI()
    {
        string stats = controller.GetEnemyStats().ToString();
        float difficulty = controller.GetDifficulty(currentFunctionVersion);
        int binaryMovement = GetMovementTypeAsNumber(); // Mostrar el tipo de movimiento binario
        float score = controller.GetTotalScore(currentFunctionVersion);

        if (enemyStatsTxt != null)
            enemyStatsTxt.text = stats;

        if (difficulty_txt != null)
            difficulty_txt.text = "Difficulty: " + Mathf.RoundToInt(difficulty); // Sin decimales

        if (enemyStatsTxt != null)
            enemyStatsTxt.text += $"- Patrón de Movimiento: {binaryMovement}";

        // Obtener las estadisticas, dififultad y puntaje total y mostrarlos en consola
        //Debug.Log(stats);
        Debug.Log("Dificultad del enemigo: " + difficulty);
        Debug.Log("FF usada: " + currentFunctionVersion);
        Debug.Log("Patrón de movimiento: " + binaryMovement);
        Debug.Log("TotalScore: " + score);

        UpdateColor(difficulty);
    }

    /// <summary>
    /// Ok Lucio, ya se que a ti no te gusta documentarme tu código, pero yo no soy asi.
    /// De momento probaré con OnTriggerStay, es decir, mientras el enemigo esté dentro del
    /// trigger del juagdor, este aplicará daño
    /// </summary>
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Ahora verificamos si el jugador está atacando para que Enemy no pueda hacerle daño
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null && playerController.isAttacking)
            {
                return;
            }

            attackCooldown -= Time.deltaTime;

            if (attackCooldown <= 0)
            {
                // Atacamos al jugador accediendo al nuevo script de la vida del jugador.
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(controller.GetEnemyStats().AttackPower);
                }

                // Se reinicia el cooldown dependiendo del tiempo de ataque
                attackCooldown = controller.GetEnemyStats().AttackRate;
            }
        }
    }

    void UpdateColor(float difficulty)
    {
        // Antiguas, sin normalizar: { 247.5f, 255.5f, 272.5f, 175f, 123.45f };
        float[] maxDifficulties = { 10f, 12f, 10f, 6f, 5f };

        if (currentFunctionVersion < 1 || currentFunctionVersion > maxDifficulties.Length)
        {
            Debug.LogError("Número de función fuera de rango.");
            return;
        }

        float maxDifficulty = maxDifficulties[currentFunctionVersion - 1];

        // Umbrales según el porcentaje
        float threshold1 = maxDifficulty * 0.33f;
        float threshold2 = maxDifficulty * 0.66f;

        Color newColor;
        if (difficulty < threshold1)
            newColor = Color.blue;
        else if (difficulty < threshold2)
            newColor = Color.yellow;
        else
            newColor = Color.red;

        // Buscar el Renderer en el objeto o en sus hijos
        Renderer enemyRenderer = GetComponentInChildren<Renderer>();

        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = newColor;
        }
        else
        {
            Debug.LogWarning("No se encontró un Renderer en el enemigo.");
        }
    }

    private void HandleSpecialEffect()
    {
        string effect = controller.GetEnemyStats().SpecialEffect;
        GameObject effectObject; // Variable para hacer que los objetos se instancien como hijos
        Vector3 spawnPosition = new Vector3(transform.position.x, 2.3f, transform.position.z); // Como quiero que se inicie 2.3f en y, se hace esto
        Debug.Log("Instanciando objeto: " + effect);
        switch (effect)
        {
            case "Poison":
                if (objectPoison != null)
                {
                    effectObject = Instantiate(objectPoison, spawnPosition, Quaternion.identity); // transform.position serviría si no quisieramos personalizarlo.
                    effectObject.transform.SetParent(transform);
                }
                else Debug.LogWarning("Olvidaste asignar el objectPoison");
                break;

            case "Burn":
                if (objectBurn != null)
                {
                    effectObject = Instantiate(objectBurn, spawnPosition, Quaternion.identity);
                    effectObject.transform.SetParent(transform);
                }
                else Debug.LogWarning("Olvidaste asignar el objectBurn");
                break;

            case "Shock":
                if (objectShock != null)
                {
                    effectObject = Instantiate(objectShock, spawnPosition, Quaternion.identity);
                    effectObject.transform.SetParent(transform);
                }
                else Debug.LogWarning("Olvidaste asignar el objectShock");
                break;

            default:
                // Lol osea, oh my god, en plan holy shit, no hay más efectos y none pues es nada.
                break;
        }
    }

    /// <summary>
    /// Reduce la vida del enemigo cuando recibe daño.
    /// </summary>
    public void TakeDamage(float damageAmount)
    {
        controller.GetEnemyStats().HP -= damageAmount;
        //Debug.Log($"Enemy HP: {controller.GetEnemyStats().HP}");

        // Verificar si el enemigo muere
        if (controller.GetEnemyStats().HP <= 0)
        {
            EnemyDeath();
        }
    }
    // Aqui se normalizan el tipo de enemigos
    private int GetMovementTypeAsNumber()
    {
        switch (enemyType)
        {
            case EnemyType.Escapista: return 0;
            case EnemyType.Agresivo: return 1;
            case EnemyType.Podadora: return 2;
            default: return -1;
        }
    }

    /// <summary>
    /// Lógica de cuando se mata al enemigo
    /// </summary>
    private void EnemyDeath()
    {
        Debug.Log("La sombra ha sido derrotada");
        Destroy(gameObject);
    }

}
