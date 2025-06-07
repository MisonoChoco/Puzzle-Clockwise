using UnityEngine;

public enum GameMode
{
    Normal,
    Timed
}

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public static GameMode CurrentMode { get; private set; } = GameMode.Normal;

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

    public void SetGameMode(GameMode mode)
    {
        CurrentMode = mode;
    }
}