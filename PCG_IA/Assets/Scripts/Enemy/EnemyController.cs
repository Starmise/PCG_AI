using UnityEngine;

/// <summary>
/// En MVC, según yo Controller solo manda a llamar información entre Model y View,
/// no es como los controller que usualmente manejamos que contienen toda la lógica
/// de funcionamiento del objeto al que corresponden
/// </summary>
public class EnemyController : MonoBehaviour
{
    private EnemyModel enemyStats;

    public EnemyController(EnemyModel stats)
    {
        enemyStats = stats;
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
}
