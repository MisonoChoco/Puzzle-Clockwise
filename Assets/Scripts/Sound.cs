using System;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sound : MonoBehaviour
{
    public string LoseSoundName = "LoseSound";
    public string WinSoundName = "WinSound";

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Lose")
        {
            PlayLoseSound();
        }
        else if (SceneManager.GetActiveScene().name == "Win")
        {
            PlayWinSound();
        }
    }

    private void PlayLoseSound()
    {
        AudioClip loseClip = Resources.Load<AudioClip>(LoseSoundName);

        AudioSource.PlayClipAtPoint(loseClip, Vector3.zero, 1.5f);
    }

    private void PlayWinSound()
    {
        AudioClip winClip = Resources.Load<AudioClip>(WinSoundName);
        AudioSource.PlayClipAtPoint(winClip, Vector3.zero, 1.5f);
    }
}