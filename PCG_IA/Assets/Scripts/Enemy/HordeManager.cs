using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HordeManager : MonoBehaviour
{
    public EnemyGenerator generator; // Asignar desde el inspector
    public GameObject enemyPrefab;

    [Header("Spawn Settings")]
    public float spawnRadius = 25f;
    public int maxSpawnAttempts = 8;

    private int waveNumber = 1;
    private List<GameObject> currentEnemies = new List<GameObject>();

    [Header("Score Settings")]
    public float baseScore = 4f;
    public float scalingFactor = 2f;
    public float difficultyWeight = 0.6f;

    void Start()
    {
        StartCoroutine(StartWave());
    }

    // Método principal para gestionar las oleadas
    IEnumerator StartWave()
    {
        float balanceWeight = 1f - difficultyWeight;
        int fitnessVersion = Random.Range(1, 6);
        float targetScore = baseScore + Mathf.Log(1 + waveNumber) * scalingFactor;

        EnemyModel baseEnemy = EnemyController.CreateRandomEnemy().GetEnemyStats(); // o como accedas al modelo
        EnemyModel generated = generator.GreedySearch(baseEnemy, fitnessVersion, difficultyWeight, balanceWeight, targetScore);

        // Generar enemigos para la oleada
        int enemiesInWave = waveNumber * 2;  // Generamos un número de enemigos por oleada (ajustable)

        for (int i = 0; i < enemiesInWave; i++)
        {
            Vector3 spawnPos;
            if (TryGetRandomNavMeshPosition(out spawnPos))
            {
                GameObject enemyGO = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                EnemyView view = enemyGO.GetComponent<EnemyView>();
                if (view != null)
                {
                    view.InitializeFromModel(generated, fitnessVersion, difficultyWeight);
                }

                currentEnemies.Add(enemyGO);
            }
            else
            {
                Debug.LogWarning("No se encontró posición válida en el Navmesh");
            }
        }

        // Esperamos hasta que todos los enemigos sean derrotados
        yield return StartCoroutine(WaitForAllEnemiesToDie());

        // Una vez todos los enemigos han muerto, comienza la siguiente oleada
        waveNumber++;
        Debug.Log("¡Comenzando la horda número " + waveNumber + "!");
        StartCoroutine(StartWave());

    }

    // Metodo para intentar spawnear en algun punto del Navmesh
    private bool TryGetRandomNavMeshPosition(out Vector3 result)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector3 randomPoint = transform.position +
                                  Random.insideUnitSphere * spawnRadius;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 3f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }


    // Método para esperar a que todos los enemigos sean derrotados
    IEnumerator WaitForAllEnemiesToDie()
    {
        while (currentEnemies.Count > 0)
        {
            // Remover enemigos muertos
            for (int i = currentEnemies.Count - 1; i >= 0; i--)
            {
                if (currentEnemies[i] == null) // Si el enemigo ha muerto
                {
                    currentEnemies.RemoveAt(i);
                }
            }

            yield return null; // Esperamos un frame y revisamos otra vez
        }

        Debug.Log("Todos los enemigos de la oleada han sido derrotados.");
    }
}
