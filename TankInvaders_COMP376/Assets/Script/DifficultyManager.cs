using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class DifficultyManager : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    public GameObject[] enemyPrefabs; // Array of enemy prefabs
    public GameObject wallPrefab; // Wall prefab to instantiate
    public float initialSpawnRate = 5f; // Initial spawn rate in seconds
    public float spawnRateIncrease = 0.5f; // Amount to decrease spawn rate by
    public float obstacleDensityIncrease = 0.1f; // Amount to increase obstacle density by
    public float difficultyIncreaseInterval = 15f; // Time interval to increase difficulty
    public Vector2 spawnAreaMin = new Vector2(0f, 0f); // Minimum coordinates of the spawn area
    public Vector2 spawnAreaMax = new Vector2(150f, 150f); // Maximum coordinates of the spawn area
    public float spawnAreaHeight = 5f; // Height of the spawn area

    private float currentSpawnRate;
    private float obstacleDensity;
    private float elapsedTime;
    private LineRenderer lineRenderer;

    void Start()
    {
        // Initialize difficulty based on player's past performance
        InitializeDifficulty();

        // Start the difficulty scaling coroutine
        StartCoroutine(IncreaseDifficultyOverTime());

        // Initialize and draw the spawn area boundary
        InitializeLineRenderer();
        DrawSpawnAreaBoundary();

        // Instantiate walls around the spawn area
        InstantiateWalls();

        // Set player start position to the middle of the spawn area
        SetPlayerStartPosition();

        // Disable the LineRenderer to remove the pink line
        lineRenderer.enabled = false;
    }

    void InitializeDifficulty()
    {
        // Get the player's past performance (e.g., from PlayerPrefs or a save file)
        int pastPerformance = PlayerPrefs.GetInt("PastPerformance", 0);

        // Set initial difficulty based on past performance
        if (pastPerformance >= 6000)
        {
            SetDifficultyTier("Hard");
        }
        else if (pastPerformance >= 4000)
        {
            SetDifficultyTier("Medium");
        }
        else
        {
            SetDifficultyTier("Easy");
        }
    }

    void SetDifficultyTier(string tier)
    {
        switch (tier)
        {
            case "Hard":
                currentSpawnRate = Mathf.Max(1f, initialSpawnRate - 2f);
                obstacleDensity = 2f;
                spawnAreaMax = new Vector2(100f, 100f);
                break;
            case "Medium":
                currentSpawnRate = Mathf.Max(1f, initialSpawnRate - 1.5f);
                obstacleDensity = 1.75f;
                spawnAreaMax = new Vector2(125f, 125f);
                break;
            case "Easy":
            default:
                currentSpawnRate = initialSpawnRate;
                obstacleDensity = 1f;
                spawnAreaMax = new Vector2(150f, 150f);
                break;
        }
    }

    IEnumerator IncreaseDifficultyOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyIncreaseInterval);

            // Get the player's current score
            int playerScore = GameManager.Instance.GetScore();

            // Adjust difficulty based on the player's score
            if (playerScore >= 6000)
            {
                currentSpawnRate = Mathf.Max(1f, initialSpawnRate - 2f);
                obstacleDensity = 2f;
            }
            else if (playerScore >= 4000)
            {
                currentSpawnRate = Mathf.Max(1f, initialSpawnRate - 1.5f);
                obstacleDensity = 1.75f;
            }
            else if (playerScore >= 2000)
            {
                currentSpawnRate = Mathf.Max(1f, initialSpawnRate - 1f);
                obstacleDensity = 1.5f;
            }
            else
            {
                currentSpawnRate = Mathf.Max(1f, currentSpawnRate - spawnRateIncrease);
                obstacleDensity += obstacleDensityIncrease;
            }

            SpawnEnemies();
        }
    }

    void SpawnEnemies()
    {
        int enemyIndex = Random.Range(0, enemyPrefabs.Length);
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Quaternion spawnRotation = Quaternion.identity;

        // Check if the selected prefab is a coin
        if (enemyPrefabs[enemyIndex].tag == "Coin")
        {
            spawnRotation = Quaternion.Euler(90, 0, 0); // Rotate 90 degrees on the X axis
            spawnPosition.y = 1; // Set y position to 1
        }

        Instantiate(enemyPrefabs[enemyIndex], spawnPosition, spawnRotation);
    }

    Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float z = Random.Range(spawnAreaMin.y, spawnAreaMax.y);

        return new Vector3(x, 0, z); // Assuming y is 0 for ground level
    }

    void InitializeLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 8; // 4 corners at the bottom + 4 corners at the top
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.useWorldSpace = true;
    }

    void DrawSpawnAreaBoundary()
    {
        Vector3[] corners = new Vector3[8];
        corners[0] = new Vector3(spawnAreaMin.x, 0, spawnAreaMin.y);
        corners[1] = new Vector3(spawnAreaMax.x, 0, spawnAreaMin.y);
        corners[2] = new Vector3(spawnAreaMax.x, 0, spawnAreaMax.y);
        corners[3] = new Vector3(spawnAreaMin.x, 0, spawnAreaMax.y);
        corners[4] = new Vector3(spawnAreaMin.x, spawnAreaHeight, spawnAreaMin.y);
        corners[5] = new Vector3(spawnAreaMax.x, spawnAreaHeight, spawnAreaMin.y);
        corners[6] = new Vector3(spawnAreaMax.x, spawnAreaHeight, spawnAreaMax.y);
        corners[7] = new Vector3(spawnAreaMin.x, spawnAreaHeight, spawnAreaMax.y);

        lineRenderer.SetPositions(corners);
    }

    void InstantiateWalls()
    {
        // Bottom wall
        InstantiateWall(new Vector3((spawnAreaMin.x + spawnAreaMax.x) / 2, spawnAreaHeight / 2, spawnAreaMin.y), new Vector3(spawnAreaMax.x - spawnAreaMin.x, spawnAreaHeight, 1));

        // Top wall
        InstantiateWall(new Vector3((spawnAreaMin.x + spawnAreaMax.x) / 2, spawnAreaHeight / 2, spawnAreaMax.y), new Vector3(spawnAreaMax.x - spawnAreaMin.x, spawnAreaHeight, 1));

        // Left wall
        InstantiateWall(new Vector3(spawnAreaMin.x, spawnAreaHeight / 2, (spawnAreaMin.y + spawnAreaMax.y) / 2), new Vector3(1, spawnAreaHeight, spawnAreaMax.y - spawnAreaMin.y));

        // Right wall
        InstantiateWall(new Vector3(spawnAreaMax.x, spawnAreaHeight / 2, (spawnAreaMin.y + spawnAreaMax.y) / 2), new Vector3(1, spawnAreaHeight, spawnAreaMax.y - spawnAreaMin.y));
    }

    void InstantiateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
        wall.transform.localScale = scale;
    }

    void SetPlayerStartPosition()
    {
        Vector3 middlePosition = new Vector3((spawnAreaMin.x + spawnAreaMax.x) / 2, 0, (spawnAreaMin.y + spawnAreaMax.y) / 2);
        player.position = middlePosition;
    }
}
