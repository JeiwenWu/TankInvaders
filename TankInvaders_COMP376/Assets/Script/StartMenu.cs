using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Load the main game scene
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }
}
