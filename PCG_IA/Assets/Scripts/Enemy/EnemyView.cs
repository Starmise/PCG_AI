using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem.XR;

/// <summary>
/// En MVC, View no es como nos habían dicho de que era solo para UI, View es ya tal cual
/// la lógica de lo que el elemnto hará en Unity, como crearse, moverse, sus usos,
/// sus eventos, etc.
/// </summary>
public class EnemyView : MonoBehaviour
{
    private EnemyController controller;
    public TMP_Text enemyStatsTxt;

    /// <summary>
    /// Acá ya se crea (al menos en teoría lógica) el enemigo,con valores que ya aleatorios.
    /// De momento nadamás indicamos que se devuelve como dificultad 1 en predeterminado, 
    /// que en este caso sería la Fitness Function 1, ya en el futuro lo hago controlable
    /// desde el inspector como se pide.
    /// </summary>
    void Start()
    {
        controller = EnemyController.CreateRandomEnemy();

        // Obtener las estadísticas y mostrarlas en consola
        string stats = controller.GetEnemyStats().ToString();
        Debug.Log(stats);

        Debug.Log("Dificultad del enemigo: " + controller.GetDifficulty(1));

        // Mostrar en la UI
        if (enemyStatsTxt != null)
        {
            enemyStatsTxt.text = stats;
        }
    }
}
