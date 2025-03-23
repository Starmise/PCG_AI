using UnityEngine;

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

    /// <summary>
    /// Las Fitness Functions, cada una dentro de un switch para poder gestionar ya después
    /// cuál fórmula será la utilizada para evaluar la dificultad de los enemigos.
    /// </summary>
    public float CalculateDifficulty(int functionVersion)
    {
        switch (functionVersion)
        {
            case 1:
                return HP + AttackPower * (1.0f / AttackRate) + GetEffectValue();
            case 2:
                return (HP + Speed) + (AttackPower / AttackRate) + GetEffectValue();
            case 3:
                return HP + (AttackPower / AttackRate) + (Speed > 5f ? 25 : 10) + GetEffectValue();
            case 4:
                return HP * 0.5f + (AttackPower * AttackRate) + GetEffectValue() 
                       + (Speed > 4f ? 20 : 10) + DetectionRange * 0.5f;
            case 5:
                return HP * 0.33f + (AttackPower * AttackRate * 0.75f) + GetEffectValue() 
                       + (Speed > 4f ? 15 : 5) + (DetectionRange * 0.33f);
            default:
                Debug.LogError("El valor asignado a la Fitness Function no es válido");
                return 0;
        }
    }

    /// <summary>
    /// Los valores especiales, de momento solo son valores numéricos.
    /// </summary>
    private float GetEffectValue()
    {
        if (SpecialEffect == "Poison") return 7.5f;
        if (SpecialEffect == "Burn") return 5f;
        if (SpecialEffect == "Shock") return 3f;
        return 0;
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
               $"- Special Effect: {SpecialEffect}\n";
    }
}
