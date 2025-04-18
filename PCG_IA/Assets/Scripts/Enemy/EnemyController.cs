using UnityEngine;

/// <summary>
/// En MVC, según yo Controller solo manda a llamar información entre Model y View,
/// no es como los controller que usualmente manejamos que contienen toda la lógica
/// de funcionamiento del objeto al que corresponden
/// </summary>
public class EnemyController
{
    private EnemyModel enemyStats;
    private bool isEscapist = false; // Bandera para determinar si el enemigo es escapista

    public EnemyController(EnemyModel stats)
    {
        enemyStats = stats;
    }

    public void SwitchToEscapist(bool escapeMode)
    {
        isEscapist = escapeMode;
    }

    public bool IsEscapist()
    {
        return isEscapist;
    }

    /// <summary>
    /// Pues nada, como te comentaba Lucio, en Controller usamos las variables y lógica
    /// de Model para pasarlos a View, acá se maneja cuál fórmula evaluará la dificultad. 
    /// </summary>
    public float GetDifficulty(int fitnessFunction)
    {
        return enemyStats.CalculateDifficulty(fitnessFunction);
    }

    // <summary>
    /// Se crea un enemigo con valores aleatorios usando los rangos establecidos en Model
    /// </summary>
    public static EnemyController CreateRandomEnemy()
    {
        EnemyModel randomStats = EnemyModel.GenerateRandomEnemy();
        return new EnemyController(randomStats);
    }

    // <summary>
    /// Este método regresa las estadísticas, pues las necesitaremos para poder mostrarlas.
    /// Por ahora se muestran en consola, pero el objetivo es que se vean en consola también.
    /// </summary>
    public EnemyModel GetEnemyStats()
    {
        return enemyStats;
    }

    public float GetTotalScore(int fitnessFunction, float difficultyWeight, float balanceWeight)
    {
        return enemyStats.CalculateTotalScore(fitnessFunction, difficultyWeight, balanceWeight);
    }

    public bool ShouldEscape(Vector3 playerPosition, Vector3 enemyPosition)
    {
        // Si es un Escapista, calcula si debe escapar
        if (isEscapist)
        {
            float distance = Vector3.Distance(playerPosition, enemyPosition);
            return distance < enemyStats.DetectionRange;
        }
        return false; // En otro caso no escapa
    }
}
