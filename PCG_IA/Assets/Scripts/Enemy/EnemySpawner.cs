using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs; // Los diferentes tipos de enemigos
    [Header("Spawn Settings")]
    public float spawnInterval = 3f; // Intervalo entre cada aparición
    public float spawnRangeX = 10f; // Rango para spawn en el eje X
    public float spawnRangeZ = 10f; // Rango para spawn en el eje Z

    private float timer = 0f; // Controla el tiempo entre los spawn

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f; // Reinicia el temporizador
        }
    }

    void SpawnEnemy()
    {
        // Escoge un enemigo aleatorio
        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);

        // Genera una posición aleatoria dentro de los rangos definidos
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX),
            0.5f, // Altura de spawn, ajusta según sea necesario
            Random.Range(-spawnRangeZ, spawnRangeZ)
        );

        // Instancia el enemigo en la escena
        Instantiate(enemyPrefabs[randomEnemyIndex], spawnPosition, Quaternion.identity);
    }
}
