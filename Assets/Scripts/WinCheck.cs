using System.Collections;
using UnityEngine;
using DG.Tweening;

public class WinPivotController : MonoBehaviour
{
    public HourHandController hourHand;
    public Transform shadowHand;
    public float matchTolerance = 5f;
    private bool isChecking = false;

    public GameObject winScreen;

    public delegate void OnWinDelegate();

    public event OnWinDelegate OnWin;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HandPivot"))
        {
            Debug.Log("One more click to win.");
            StartCoroutine(HandlePivotConnection());
            isChecking = true;
        }
    }

    private IEnumerator HandlePivotConnection()
    {
        yield return new WaitForSeconds(0.1f);

        if (hourHand != null)
        {
            hourHand.SetAnchor(transform);
            hourHand.StopRotation();
        }
        else
        {
            Debug.LogWarning("HourHand reference is missing!");
        }
    }

    private void Update()
    {
        if (hourHand == null)
        {
            hourHand = Object.FindFirstObjectByType<HourHandController>();
            if (hourHand == null) return;
        }

        if (shadowHand == null)
        {
            GameObject shadowHandObj = GameObject.FindWithTag("ShadowHand");
            if (shadowHandObj != null)
            {
                shadowHand = shadowHandObj.transform;
            }
            else
            {
                //safety check if shadow hand is not found
                return;
            }
        }

        if (isChecking && shadowHand != null && hourHand != null)
        {
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(
                hourHand.transform.eulerAngles.z,
                shadowHand.eulerAngles.z
            ));

            if ((angleDifference <= matchTolerance && BellManager.Instance != null && BellManager.Instance.AreAllBellsActivated())
    || angleDifference <= matchTolerance) //win condition
            {
                Debug.Log("Winner");

                ScoreManager.Instance.AddScore(10);

                if (GameSettings.CurrentMode == GameMode.Timed)
                {
                    float timeLeft = Object.FindFirstObjectByType<LevelTimer>().GetTimeLeft();

                    int bonusScore = 0;

                    if (timeLeft > 40) bonusScore = 25;
                    else if (timeLeft > 30) bonusScore = 20;
                    else if (timeLeft > 20) bonusScore = 15;
                    else if (timeLeft > 10) bonusScore = 10;
                    else if (timeLeft > 5) bonusScore = 3;

                    ScoreManager.Instance.AddScore(bonusScore);
                }

                Instantiate(winScreen, Vector3.zero, Quaternion.identity);

                isChecking = false;

                // Clean up DOTween
                DOTween.Kill(hourHand.transform);
                DOTween.KillAll(false);

                OnWin?.Invoke();
            }
        }
    }
}