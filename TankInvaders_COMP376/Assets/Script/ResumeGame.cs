using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResumeGame : MonoBehaviour
{
    public Text topScoresText; // Reference to the UI Text for top scores

    void Start()
    {
        DisplayTopScores();
    }

    void Update()
    {
        // Check for the Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Call the Resume method from the GameManager
            GameManager.Instance.Resume();
        }
    }

    public void OnResumeButtonClick()
    {
        // Call the Resume method from the GameManager
        GameManager.Instance.Resume();
    }

    private void DisplayTopScores()
    {
        List<int> topScores = GameManager.Instance.GetTopScores();
        topScoresText.text = "Top Scores:\n";
        for (int i = 0; i < topScores.Count; i++)
        {
            topScoresText.text += (i + 1) + ". " + topScores[i] + "\n";
        }
    }
}
