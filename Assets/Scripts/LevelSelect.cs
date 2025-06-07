using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectSprite : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    [SerializeField] private string gameplaySceneName = "GameplayScene";

    private void OnMouseDown()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData not assigned on: " + gameObject.name);
            return;
        }

        LevelLoader.SelectedLevelData = levelData;
        SceneManager.LoadScene(gameplaySceneName);
    }
}