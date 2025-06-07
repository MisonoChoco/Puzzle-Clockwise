using UnityEngine;

public class WinFailScreenButton : MonoBehaviour
{
    public enum ButtonType
    { Home, Restart, Next }

    public ButtonType buttonType;

    private WinFailScreenUI winorfailScreenUI;

    private void Start()
    {
        // Try to get WinFailScreenUI from the root
        winorfailScreenUI = GetComponentInParent<WinFailScreenUI>();
        winorfailScreenUI?.UpdateUI();
    }

    private void OnMouseDown()
    {
        // Called when this sprite is clicked or tapped
        switch (buttonType)
        {
            case ButtonType.Home:
                GameFlowManager.Instance.ReturnToLobby();
                break;

            case ButtonType.Restart:
                GameFlowManager.Instance.RestartCurrentLevel();
                break;

            case ButtonType.Next:
                GameFlowManager.Instance.LoadNextLevel();
                break;
        }

        Destroy(transform.root.gameObject); // Remove
    }
}