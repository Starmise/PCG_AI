using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject UIStats;

    private void Start()
    {
        UIStats.SetActive(true);
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
}
