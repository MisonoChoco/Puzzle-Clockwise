using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float totalTime = 60f;

    public static LevelTimer Instance;

    private float currentTime;
    public bool isRunning = false;

    public UnityEvent OnTimeOut;

    private void Start()
    {
        StartTimer(totalTime);
    }

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

    private void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            OnTimeOut?.Invoke();
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    public void StartTimer(float time)
    {
        totalTime = time;
        currentTime = totalTime;
        isRunning = true;
        UpdateTimerText();
    }

    public void ResetTimer()
    {
        StartTimer(totalTime);
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetTimeLeft() => currentTime;
}