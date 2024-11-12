using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class WinLoseManager : MonoBehaviour
{
    public Text leaderboardText; // Reference to the UI Text for the leaderboard

    void Start()
    {
        UpdateLeaderboard();
    }

    public void ReturnToMenu()
    {
        // Load the MainMenu scene
        SceneManager.LoadScene("MainMenu");
    }

    public void RetryGame()
    {
        // Load the GameScene scene
        SceneManager.LoadScene("GameScene");
    }

    private void UpdateLeaderboard()
    {
        List<int> topScores = GameManager.Instance.GetTopScores();
        leaderboardText.text = "Top 5 Scores:\n";
        for (int i = 0; i < topScores.Count; i++)
        {
            leaderboardText.text += (i + 1) + ". " + topScores[i] + "\n";
        }
    }

    public void ChangeLeaderboardText(string newText)
    {
        if (leaderboardText != null)
        {
            leaderboardText.text = newText;
        }
        else
        {
            Debug.LogError("Leaderboard Text is not assigned in the Inspector.");
        }
    }
}
