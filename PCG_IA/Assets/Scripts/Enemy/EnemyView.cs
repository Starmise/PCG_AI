using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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

        currentFunctionVersion = Random.Range(1, 6); // Selecciona una función aleatoria entre 1 y 5

        if (enemyType == EnemyType.Podadora)
        {
            destinoActual = puntoA.position;
            agent.SetDestination(destinoActual);
        }

        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeDifficultyFunction(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeDifficultyFunction(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeDifficultyFunction(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeDifficultyFunction(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeDifficultyFunction(5);

        float detectionRange = controller.GetEnemyStats().DetectionRange;

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

        if (enemyStatsTxt != null)
            enemyStatsTxt.text = stats;

        if (difficulty_txt != null)
            difficulty_txt.text = "Difficulty: " + Mathf.RoundToInt(difficulty); // Sin decimales

        // Obtener las estadisticas y mostrarlas en consola
        Debug.Log(stats);
        Debug.Log("Dificultad del enemigo: " + difficulty);
        Debug.Log("FF usada: " + currentFunctionVersion);
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
            attackCooldown -= Time.deltaTime;

            if (attackCooldown <= 0)
            {
                // Atacamos al jugador accediendo al nuevo script de la vida del jugador.
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(controller.GetEnemyStats().AttackPower);
                }

                // De momento vamos a instanciar un objeto diferente para cada efecto especial,
                // por el poco tiempo disponible no podemos meter VFX, y sepa dios como se hacen.
                HandleSpecialEffect();

                // Se reinicia el cooldown dependiendo del tiempo de ataque
                attackCooldown = controller.GetEnemyStats().AttackRate;
            }
        }
    }

    private void HandleSpecialEffect()
    {
        string effect = controller.GetEnemyStats().SpecialEffect;
        //GameObject effectObject = null; // Variable para después hacer que los objetos se instancien como hijos

        switch (effect)
        {
            case "Poison":
                if (objectPoison != null)
                {
                    Instantiate(objectPoison, transform.position, Quaternion.identity);
                }
                else Debug.LogWarning("Olvidaste asignar el objectPoison");
                break;

            case "Burn":
                if (objectBurn != null)
                {
                    Instantiate(objectBurn, transform.position, Quaternion.identity);
                }
                else Debug.LogWarning("Olvidaste asignar el objectBurn");
                break;

            case "Shock":
                if (objectShock != null)
                {
                    Instantiate(objectShock, transform.position, Quaternion.identity);
                }
                else Debug.LogWarning("Olvidaste asignar el objectShock");
                break;

            default:
                // Lol osea, oh my god, en plan holy shit, no hay más efectos y none pues es nada.
                break;
        }
    }


}
