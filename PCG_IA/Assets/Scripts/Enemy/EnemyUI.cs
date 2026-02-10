using TMPro;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    public TMP_Text difficultyTxt;
    public TMP_Text difficultyWeightTxt;
    public TMP_Text balanceWeightTxt;

    private Camera mainCam;
    private Transform target;

    public Vector3 offset = new Vector3(-1.2f, 1.8f, 0f);

    void Awake()
    {
        mainCam = Camera.main;
    }

    public void Initialize(Transform enemy)
    {
        target = enemy;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Se instancia a a la izquierda del enemigo
        transform.position = target.position + target.right * offset.x + Vector3.up * offset.y;

        // Billboard, es decir, que siempre mira a la camara
        transform.forward = mainCam.transform.forward;
    }

    public void UpdateValues(float difficulty, float diffWeight, float balanceWeight)
    {
        difficultyTxt.text = $"Difficulty: {Mathf.RoundToInt(difficulty)}";
        difficultyWeightTxt.text = $"Diff W: {diffWeight:F2}";
        balanceWeightTxt.text = $"Balance W: {balanceWeight:F2}";
    }

    public void SetVisible(bool value)
    {
        gameObject.SetActive(value);
    }
}
