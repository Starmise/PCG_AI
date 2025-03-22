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
    private EnemyController controller;
    public TMP_Text enemyStatsTxt;
    public TMP_Text difficulty_txt;

    private NavMeshAgent agent;
    public Transform player;

    // Variables para el enemigo Podadora
    public Transform puntoA;
    public Transform puntoB;
    private Vector3 destinoActual;

    public enum EnemyType { Escapista, Agresivo, Podadora }
    public EnemyType enemyType;

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

        switch (enemyType)
        {
            case EnemyType.Escapista:

                float distancia = Vector3.Distance(player.position, transform.position);

                if (distancia < controller.GetEnemyStats().DetectionRange)
                {
                    Vector3 escapeDirection = (transform.position - player.position).normalized;
                    Vector3 escapeDestination = transform.position + escapeDirection * 5f;

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(escapeDestination, out hit, 5f, NavMesh.AllAreas))
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
            difficulty_txt.text = "Dificultad: " + Mathf.RoundToInt(difficulty); // Sin decimales
    }
}
