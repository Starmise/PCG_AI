using UnityEngine;

public class TailMovement : MonoBehaviour
{
    private Transform[] tailBones;

    public string tailRootName; // Nombre del hueso raíz
    public float waveSpeed = 2f;
    public float waveAmplitude = 10f;
    public Color gizmoColor = Color.cyan; // Color para los Gizmos para visualizar los huesos

    private void Awake()
    {
        // Encuentra todos los huesos de la cola dinámicamente
        Transform tailRoot = FindBone(transform, tailRootName);
        if (tailRoot != null)
        {
            // Obtiene todos los hijos del objeto que encuentre con el nombre, que es el hueso raíz
            tailBones = tailRoot.GetComponentsInChildren<Transform>();
            Debug.Log($"Huesos de la cola encontrados: {tailBones.Length}");
        }
        else
        {
            Debug.LogError("No se encontró el hueso raíz de la cola.");
        }
    }

    void Start()
    {
    }

    void Update()
    {
        if (tailBones == null || tailBones.Length < 2) return;

        for (int i = 1; i < tailBones.Length; i++) // Empieza desde 1 para evitar afectar la raíz
        {
            float waveOffset = i * 0.2f; // Hace que el movimiento se propague de forma escalonada
            float angle = Mathf.Sin(Time.time * waveSpeed - waveOffset) * waveAmplitude;

            // Aplica la rotación acumulativa
            tailBones[i].localRotation = Quaternion.Euler(angle, 0, 0) * tailBones[i].localRotation;
        }
    }

    // Función recursiva para encontrar el hueso dentro del personaje
    Transform FindBone(Transform parent, string boneName)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.name == boneName)
                return child;
        }
        return null;
    }

    // Dibujar Gizmos para visualizar los huesos en la escena
    void OnDrawGizmos()
    {
        if (tailBones == null) return;

        Gizmos.color = gizmoColor;
        foreach (Transform bone in tailBones)
        {
            if (bone != null)
            {
                Gizmos.DrawSphere(bone.position, 0.02f); // Esfera en cada hueso
            }
        }
    }
}
