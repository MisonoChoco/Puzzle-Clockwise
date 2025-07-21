using System.Collections;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;

    [Header("Level Configuration")]
    public LevelData[] levels;

    private int currentLevelIndex = 0;

    [Header("References")]
    public LevelTimer Timer; // Note: This seems unused, consider removing if not needed

    [SerializeField] private LevelManager levelManager;

    private bool isTransitioning = false; // Prevent multiple simultaneous transitions

    private void Awake()
    {
        // Singleton pattern with proper cleanup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; // Exit early to prevent further execution
        }
    }

    private void Start()
    {
        // Validate references before starting
        if (ValidateReferences())
        {
            LoadLevel(currentLevelIndex);
        }
    }

    private bool ValidateReferences()
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("GameFlowManager: No levels assigned!");
            return false;
        }

        if (levelManager == null)
        {
            Debug.LogError("GameFlowManager: LevelManager reference is missing!");
            return false;
        }

        // Check for null levels in the array
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] == null)
            {
                Debug.LogWarning($"GameFlowManager: Level at index {i} is null!");
            }
        }

        return true;
    }

    public void LoadLevel(int index)
    {
        // Prevent multiple simultaneous transitions
        if (isTransitioning)
        {
            Debug.LogWarning("GameFlowManager: Already transitioning to another level!");
            return;
        }

        // Validate index bounds
        if (index < 0 || index >= levels.Length)
        {
            Debug.LogWarning($"GameFlowManager: Invalid level index {index}. Available levels: 0-{levels.Length - 1}");
            return;
        }

        // Validate level data
        if (levels[index] == null)
        {
            Debug.LogError($"GameFlowManager: Level data at index {index} is null!");
            return;
        }

        // Validate levelManager reference
        if (levelManager == null)
        {
            Debug.LogError("GameFlowManager: LevelManager reference is missing!");
            return;
        }

        currentLevelIndex = index;
        isTransitioning = true;

        StartCoroutine(LoadLevelCoroutine(index));
    }

    private IEnumerator LoadLevelCoroutine(int index)
    {
        // Use a coroutine wrapper to handle the callback properly
        bool transitionComplete = false;

        StartCoroutine(levelManager.TransitionOutOldLevel(() =>
        {
            try
            {
                LevelLoader.SelectedLevelData = levels[index];
                levelManager.LoadLevel(levels[index]);
                transitionComplete = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"GameFlowManager: Error loading level {index}: {e.Message}");
                transitionComplete = true;
            }
        }));

        // Wait for transition to complete
        yield return new WaitUntil(() => transitionComplete);

        isTransitioning = false;
    }

    public void LoadNextLevel()
    {
        int nextIndex = currentLevelIndex + 1;

        // Check if there are more levels
        if (nextIndex >= levels.Length)
        {
            Debug.Log("GameFlowManager: No more levels! Reached the end.");
            // Optionally handle end of game scenario here
            return;
        }

        LoadLevel(nextIndex);
        StartTimerIfNeeded();
    }

    public void RestartCurrentLevel()
    {
        LoadLevel(currentLevelIndex);
        StartTimerIfNeeded();
    }

    private void StartTimerIfNeeded()
    {
        // Check if we're in timed mode and timer instance exists
        if (GameSettings.CurrentMode == GameMode.Timed)
        {
            if (LevelTimer.Instance != null)
            {
                LevelTimer.Instance.StartTimer(60f);
            }
            else
            {
                Debug.LogWarning("GameFlowManager: LevelTimer.Instance is null but timed mode is active!");
            }
        }
    }

    public void ReturnToLobby()
    {
        // Prevent lobby loading during transition
        if (isTransitioning)
        {
            Debug.LogWarning("GameFlowManager: Cannot return to lobby during level transition!");
            return;
        }

        isTransitioning = true;

        try
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GameFlowManager: Error loading lobby scene: {e.Message}");
            isTransitioning = false;
        }
    }

    // Utility methods for external access
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    public LevelData GetCurrentLevel()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < levels.Length)
            return levels[currentLevelIndex];
        return null;
    }

    public int GetTotalLevelCount()
    {
        return levels?.Length ?? 0;
    }

    public bool IsTransitioning()
    {
        return isTransitioning;
    }

    // Called when the object is destroyed
    private void OnDestroy()
    {
        // Clean up singleton reference if this is the instance
        if (Instance == this)
        {
            Instance = null;
        }
    }
}