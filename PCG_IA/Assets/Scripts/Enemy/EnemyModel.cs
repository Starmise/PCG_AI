using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// En MVC, Model solo es lógica para los datos básicos, por eso no se hace MonoBehaviour.
/// Una forma fácil de verlo es como un header, contiene las variables, su inicialización
/// y los métodos principales, pero no la lógica de ejecución.
/// </summary>
public class EnemyModel
{
    public float HP;
    public float AttackPower;
    public float AttackRate; // Velocidad de ataque
    public float Speed;
    public string SpecialEffect; // Efectos especiales

    // Valores normalizados
    float normHP;
    float normAttackPower;
    float normAttackRate;
    float normSpeed;
    float normDetectionRange;
    float normSpeedDiff;


    // Propiedades binarizadas de efectos especiales
    public int HasPoison => SpecialEffect == "Poison" ? 1 : 0;
    public int HasBurn => SpecialEffect == "Burn" ? 1 : 0;
    public int HasShock => SpecialEffect == "Shock" ? 1 : 0;
    public int HasEffect => SpecialEffect != "None" ? 1 : 0; // General

    public float DetectionRange { get; private set; }

    // Se declara al array como static para ahorrar memoria porque no se necesita
    // Crear una copia para cada enemigo. Si no quisieramos que se ediara en el código,
    // se le puede añadir un readonly como propiedad al array.
    private static string[] PossibleEffects = { "Poison", "Burn", "Shock", "None" };

    /// <summary>
    /// Almacenamos las variables para las estadísticas de los enemigos
    /// </summary>
    public EnemyModel(float hp, float attackPower, float attackRate, float speed, string specialEffect, float detectionRange)
    {
        HP = hp;
        AttackPower = attackPower;
        AttackRate = attackRate;
        Speed = speed;
        SpecialEffect = specialEffect;
        DetectionRange = detectionRange;
    }

    private void UpdateNormalizedStats()
    {
        normHP = Normalize(HP, 50f, 200f);
        normAttackPower = Normalize(AttackPower, 5f, 20f);
        normAttackRate = Normalize(AttackRate, 0.5f, 2f);
        normSpeed = Normalize(Speed, 1f, 8f);
        normDetectionRange = Normalize(DetectionRange, 5f, 15f);

        normSpeedDiff = (Speed > 4f) ? 0 : 1;
    }

    /// <summary>
    /// Método encargado de normalizar valores, para no tener que ir haciendo
    /// opración por operación.
    /// </summary>
    private float Normalize(float value, float min, float max)
    {
        return Mathf.Clamp01((value - min) / (max - min));
    }

    /// <summary>
    /// Las Fitness Functions normalizadas, cada una dentro de un switch para poder gestionar ya después
    /// cuál fórmula será la utilizada para evaluar la dificultad de los enemigos.
    /// </summary>
    public float CalculateDifficulty(int functionVersion)
    {
        UpdateNormalizedStats();

        switch (functionVersion)
        {
            case 1:
                return normHP + normAttackPower * (1.0f / Mathf.Max(normAttackRate, 0.01f)) + GetEffectBinary();
            case 2:
                return (normHP + normSpeed) + (normAttackPower / Mathf.Max(normAttackRate, 0.01f)) + GetEffectBinary();
            case 3:
                return normHP + (normAttackPower / Mathf.Max(normAttackRate, 0.01f)) + normSpeedDiff + GetEffectBinary();
            case 4:
                return normHP * 0.5f + (normAttackPower * normAttackRate) + GetEffectBinary()
                       + normSpeedDiff + normDetectionRange * 0.5f;
            case 5:
                return normHP * 0.33f + (normAttackPower * normAttackRate * 0.75f) + GetEffectBinary()
                       + normSpeedDiff + normDetectionRange * 0.33f;
            default:
                Debug.LogError("El valor asignado a la Fitness Function no es válido");
                return 0;
        }
    }

    public float CalculateBalance(int functionVersion)
    {
        UpdateNormalizedStats();

        // Calculo del Balance
        float sumStats = normHP + normAttackPower + normAttackRate + normSpeedDiff + GetEffectBinary() + normDetectionRange;
        float balanceScore = 1f - Mathf.Clamp01(Mathf.Abs(sumStats / 6)); //Entre 6 por la cantidad de stats

        return balanceScore;
    }

    public float CalculateTotalScore(int functionVersion, float difficultyWeight, float balanceWeight)
    {
        float rawDifficulty = CalculateDifficulty(functionVersion);
        float maxDifficulty = 1f;

        // Acá rezamos para que los calculos sean correctos
        switch (functionVersion)
        {
            case 1:
                maxDifficulty = 10f;
                break;
            case 2:
                maxDifficulty = 12f;
                break;
            case 3:
                maxDifficulty = 10f;
                break;
            case 4:
                maxDifficulty = 6f;
                break;
            case 5:
                maxDifficulty = 5f;
                break;
            default:
                Debug.LogError("Fitness Function no válida");
                return 0;
        }
        float difficultyScore = Normalize(rawDifficulty, 0, maxDifficulty);

        float balanceScore = CalculateBalance(functionVersion);
        return difficultyScore * difficultyWeight + balanceScore * balanceWeight;
    }


    ///// <summary>
    ///// Los valores especiales, de momento solo son valores numéricos.
    ///// </summary>
    //private float GetEffectValue()
    //{
    //    if (SpecialEffect == "Poison") return 7.5f;
    //    if (SpecialEffect == "Burn") return 5f;
    //    if (SpecialEffect == "Shock") return 3f;
    //    return 0;
    //}

    // Nuevo método binario
    private float GetEffectBinary()
    {
        return HasEffect;
    }

    /// <summary>
    /// Acá establecemos los rangos para obtener un valor aleatorio de cada estadística
    /// que tienen los enemigos.
    /// </summary>
    public static EnemyModel GenerateRandomEnemy()
    {
        return new EnemyModel(
            Random.Range(50f, 200f), // HP
            Random.Range(5f, 20f), // Ataque
            Random.Range(0.5f, 2f), // Vel. de Ataque
            Random.Range(1f, 8f), // Velocidad
            PossibleEffects[Random.Range(0, PossibleEffects.Length)],// Efecto aleatorio
            Random.Range(5f, 15f) // Rango de detección
        );
    }

    /// <summary>
    /// No sabía, pero se puede crear un método ToString que contenga la información a mostrar, 
    /// sin necesidad de ir escribiendolo nuevamente cada que sea necesario. También,
    /// se puede usar $ para que en la cadena de strings no sea necesario usar + al poner variables.
    /// Se usa override para que el mensaje personalizado funcione correctamente.
    /// </summary>
    public override string ToString()
    {
        return $"Enemy Stats:\n" +
               $"- HP: {Mathf.RoundToInt(HP)}\n" +
               $"- Attack: {Mathf.RoundToInt(AttackPower)}\n" +
               $"- Attack Rate: {Mathf.RoundToInt(AttackRate)}\n" +
               $"- Speed: {Mathf.RoundToInt(Speed)}\n" +
               $"- Special Effect: {SpecialEffect} {HasEffect}\n";
    }
}
