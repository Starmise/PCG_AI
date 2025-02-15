using UnityEngine;
using TMPro;

public class KeyManager : MonoBehaviour
{
    public static KeyManager instance;
    public TextMeshProUGUI keyText;
    private int keyCount = 0;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public void AddKey()
    {
        keyCount++;
        keyText.text = "" + keyCount; // Actualiza la UI
    }

    public bool HasKey()
    {
        return keyCount > 0; // Devuelve true si hay llaves disponibles
    }

    public void UseKey()
    {
        if (keyCount > 0)
        {
            keyCount--; // Resta una llave
            keyText.text = "Llaves: " + keyCount; // Actualiza la UI
        }
    }
}
