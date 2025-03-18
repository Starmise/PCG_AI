using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // Variables numericas
    public float TimeBetweenShots = 0.3333f;
    private float m_timeStamp = 0f; // Marca de tiempo para los disparos

    // Booleanos

    // Referencias
    public GameObject BulletPrefab;
    public Transform BulletSpawn;

    /// <summary>
    /// En cada frame constante se verifica que haya pasado suficiente tiempo 
    /// desde el último disparo y si el jugador mantiene presionado el botón de disparo.
    /// </summary>
    void FixedUpdate()
    {
        if ((Time.time >= m_timeStamp) && (Input.GetKey(KeyCode.Mouse0)))
        {
            Fire();
            m_timeStamp = Time.time + TimeBetweenShots;
        }
    }

    /// <summary>
    /// Instancia una bala desde la posición del jugador para ser disparada.
    /// </summary>
    void Fire()
    {
        var bullet = (GameObject)Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation);

        // La velocidad de la bala
        bullet.GetComponent<Rigidbody>().linearVelocity = bullet.transform.forward * 50;

        // Para evitar problemas de memoria, se destruye la bala tras 2 segundos
        Destroy(bullet, 2.0f);
    }
}
