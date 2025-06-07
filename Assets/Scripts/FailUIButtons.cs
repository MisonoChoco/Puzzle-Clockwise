using UnityEngine;

public class FailScreenButton : MonoBehaviour
{
    public enum ButtonType
    { Home, Restart }

    public ButtonType buttonType;

    private void OnMouseDown()
    {
        switch (buttonType)
        {
            case ButtonType.Home:
                GameFlowManager.Instance.ReturnToLobby();
                Destroy(transform.root.gameObject);
                break;

            case ButtonType.Restart:
                GameFlowManager.Instance.RestartCurrentLevel();
                Destroy(transform.root.gameObject);
                break;
        }
    }
}