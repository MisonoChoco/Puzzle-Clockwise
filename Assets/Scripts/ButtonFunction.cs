using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SpriteButton : MonoBehaviour
{
    public UnityEvent onClick;

    private void OnMouseUpAsButton()
    {
        onClick?.Invoke();
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void LoadLevelSelectTimed()
    {
        SceneManager.LoadScene("LevelSelectTimed");
    }

    public void LoadTutorialScene()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void RestartCurrentLevel()
    {
        GameFlowManager.Instance.RestartCurrentLevel();
    }

    public void LoadGuideLevel()
    {
        SceneManager.LoadScene("Guide");
    }
}