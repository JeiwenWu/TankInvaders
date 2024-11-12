using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Text elapsedTimeText; // Reference to the UI Text for elapsed time
    public Text scoreText; // Reference to the UI Text for score
    public Button pauseButton; // Reference to the pause button
    public float gameDuration = 300f; // Game duration in seconds (default is 5 minutes)

    private float elapsedTime;
    private float timePointInterval;
    private int score;
    private bool isPaused = false;
    private bool gameEnded = false; // Flag to indicate if the game has ended
    private Player player;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep this GameObject when loading new scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        ResetGame();
    }

    void Update()
    {
        if (isPaused || gameEnded)
            return;

        // Update the elapsed time
        elapsedTime += Time.deltaTime;
        timePointInterval += Time.deltaTime;

        // Gain 5 points every second
        if (timePointInterval >= 1f)
        {
            AddTimePoints();
            timePointInterval = 0f;
        }

        UpdateUI();

        // Check for the pause key (Escape key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        // Check for win condition
        if (elapsedTime >= gameDuration)
        {
            gameEnded = true; // Stop the timer and score updates
            StoreScore(score);
            WinGame();
        }

        // Check for loss condition (0 HP)
        if (player != null && player.GetCurrentHP() <= 0)
        {
            gameEnded = true; // Stop the timer and score updates
            StoreScore(score);
            LoseGame();
        }
    }

    public void AddEnemyPoints(int points)
    {
        // Gain 1000 points for destroying an enemy
        score += points;
        UpdateUI();
    }

    public void AddCoinPoints()
    {
        // Gain 500 points for picking up a coin
        score += 500;
        UpdateUI();
    }

    public void AddTimePoints()
    {
        // Gain 5 points every second
        score += 5;
        UpdateUI();
    }

    public int GetScore()
    {
        // Return the current score
        return score;
    }

    private void UpdateUI()
    {
        // Update the UI elements
        elapsedTimeText.text = "Time: " + elapsedTime.ToString("F2") + "s";
        scoreText.text = "Score: " + score.ToString();
    }

    public void Resume()
    {
        SceneManager.UnloadSceneAsync("PauseMenu");
        Time.timeScale = 1f; // Resume game time
        isPaused = false;
        pauseButton.interactable = true; // Enable the pause button
    }

    public void Pause()
    {
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        Time.timeScale = 0f; // Pause game time
        isPaused = true;
        pauseButton.interactable = false; // Disable the pause button
    }

    private void WinGame()
    {
        pauseButton.gameObject.SetActive(false); // Hide the pause button
        // Change to the victory scene
        SceneManager.LoadScene("VictoryScene");
    }

    private void LoseGame()
    {
        pauseButton.gameObject.SetActive(false); // Hide the pause button
        // Change to the losing scene
        SceneManager.LoadScene("LoseScene");
    }

    private void StoreScore(int newScore)
    {
        List<int> topScores = new List<int>();

        // Load existing top scores
        for (int i = 0; i < 5; i++)
        {
            topScores.Add(PlayerPrefs.GetInt("TopScore" + i, 0));
        }

        // Add the new score and sort the list
        topScores.Add(newScore);
        topScores.Sort((a, b) => b.CompareTo(a)); // Sort in descending order

        // Keep only the top 5 scores
        if (topScores.Count > 5)
        {
            topScores.RemoveAt(5);
        }

        // Save the top 5 scores
        for (int i = 0; i < topScores.Count; i++)
        {
            PlayerPrefs.SetInt("TopScore" + i, topScores[i]);
        }

        PlayerPrefs.Save();
    }

    public List<int> GetTopScores()
    {
        List<int> topScores = new List<int>();

        // Load existing top scores
        for (int i = 0; i < 5; i++)
        {
            topScores.Add(PlayerPrefs.GetInt("TopScore" + i, 0));
        }

        return topScores;
    }

    private void ResetGame()
    {
        elapsedTime = 0f;
        timePointInterval = 0f;
        score = 0;
        gameEnded = false;
        player = FindObjectOfType<Player>(); // Find the player in the scene
        UpdateUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene") // Replace "GameScene" with the actual name of your game scene
        {
            pauseButton.gameObject.SetActive(true); // Re-enable the pause button
            ResetGame();
        }
    }
}
