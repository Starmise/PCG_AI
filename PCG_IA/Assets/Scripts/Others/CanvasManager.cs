using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject UIStats;
    [SerializeField] private GameObject deathUI;

    private void Start()
    {
        UIStats.SetActive(true);
        deathUI.SetActive(false);
    }

    public void restartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToggleUI()
    {
        if (UIStats.activeSelf == false)
        {
            UIStats.SetActive(true);
        }
        else UIStats.SetActive(false);
    }

    public void deathScreen()
    {
        deathUI.SetActive(true);
    }
}
