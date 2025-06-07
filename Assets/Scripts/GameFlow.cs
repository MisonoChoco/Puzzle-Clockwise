using System.Threading;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;

    public LevelData[] levels;
    private int currentLevelIndex = 0;

    public LevelTimer Timer;

    [SerializeField] private LevelManager levelManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int index)
    {
        if (index >= 0 && index < levels.Length)
        {
            currentLevelIndex = index;
            StartCoroutine(levelManager.TransitionOutOldLevel(() =>
            {
                LevelLoader.SelectedLevelData = levels[index]; // keep this line to ensure LevelManager loads correct level
                levelManager.LoadLevel(levels[index]);
            }));
        }
        else
        {
            Debug.Log("No more levels!");
        }
    }

    public void LoadNextLevel()
    {
        LoadLevel(currentLevelIndex + 1);
        if (GameSettings.CurrentMode == GameMode.Timed)
        {
            LevelTimer.Instance.StartTimer(60f);
        }
    }

    public void RestartCurrentLevel()
    {
        LoadLevel(currentLevelIndex);
        if (GameSettings.CurrentMode == GameMode.Timed)
        {
            LevelTimer.Instance.StartTimer(60f);
        }
    }

    public void ReturnToLobby()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }
}