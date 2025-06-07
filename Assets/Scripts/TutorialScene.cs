using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialLevelController : MonoBehaviour
{
    public WinPivotController winChecker;

    private void Start()
    {
        if (winChecker != null)
        {
            winChecker.OnWin += HandleWin;
        }
    }

    private void HandleWin()
    {
        Debug.Log("Tutorial won");

        SceneManager.LoadScene("Lobby");
    }

    private void OnDestroy()
    {
        if (winChecker != null)
        {
            winChecker.OnWin -= HandleWin;
        }
    }
}