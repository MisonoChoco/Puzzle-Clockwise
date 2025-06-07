using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundScaler : MonoBehaviour
{
    private void Start()
    {
        ScaleToScreen();
    }

    private void ScaleToScreen()
    {
        Camera cam = Camera.main;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (cam == null || sr.sprite == null)
        {
            Debug.LogWarning("Camera or SpriteRenderer.sprite missing.");
            return;
        }

        float screenHeight = 2f * cam.orthographicSize;
        float screenWidth = screenHeight * cam.aspect;

        Vector2 spriteSize = sr.sprite.bounds.size;

        // Calculate scale to fit the screen exactly
        float scaleX = screenWidth / spriteSize.x;
        float scaleY = screenHeight / spriteSize.y;

        transform.localScale = new Vector3(scaleX, scaleY, 1);

        //set z position behind gameplay
        if (transform.position.z >= 0)
            transform.position = new Vector3(transform.position.x, transform.position.y, 10);
    }
}