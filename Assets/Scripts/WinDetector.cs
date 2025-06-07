using UnityEngine;

public class WinListener : MonoBehaviour
{
    private WinPivotController winPivot;

    private void Start()
    {
        winPivot = Object.FindFirstObjectByType<WinPivotController>();
        if (winPivot != null)
        {
            winPivot.OnWin += HandleWin;
        }
        else
        {
            Debug.LogWarning("WinPivotController not found");
        }
    }

    private void OnDestroy()
    {
        if (winPivot != null)
        {
            winPivot.OnWin -= HandleWin;
        }
    }

    private void HandleWin()
    {
        Debug.Log("Level complete, advancing..");
        GameFlowManager.Instance.LoadNextLevel();
    }
}