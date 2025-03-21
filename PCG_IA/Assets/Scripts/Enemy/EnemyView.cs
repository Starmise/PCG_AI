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
    public TMP_Text difficulty_txt;

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
        currentFunctionVersion = Random.Range(1, 6); // Selecciona una función aleatoria entre 1 y 5
        UpdateUI();
        /*
         * Codigo original
        // Obtener las estadísticas y mostrarlas en consola
        string stats = controller.GetEnemyStats().ToString();
        Debug.Log(stats);

        float difficulty = controller.GetDifficulty(1);
        Debug.Log("Dificultad del enemigo: " + difficulty);

        // Debug.Log("Dificultad del enemigo: " + controller.GetDifficulty(1));

        // Mostrar en la UI
        if (enemyStatsTxt != null)
        {
            enemyStatsTxt.text = stats;
        }

        if (difficulty_txt != null)
        {
            int difficultyInt = (int)difficulty; // Corta los decimales
            difficulty_txt.text = "Dificultad: " + difficultyInt;
        }
        */
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeDifficultyFunction(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeDifficultyFunction(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeDifficultyFunction(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeDifficultyFunction(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeDifficultyFunction(5);
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
