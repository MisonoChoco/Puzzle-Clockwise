using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public float targetAspect = 9f / 16f; //portrait

    private void Start()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera cam = Camera.main;

        if (scaleHeight < 1.0f)
        {
            cam.orthographicSize = cam.orthographicSize / scaleHeight;
        }
    }
}