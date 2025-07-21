using UnityEngine;
using DG.Tweening;

public class SpriteButtonAnimator : MonoBehaviour
{
    private Vector3 originalScale;
    private Tween currentTween;
    private bool isPointerOver = false;
    private Camera mainCamera;

    private void Start()
    {
        originalScale = transform.localScale;

        // Cache camera reference to avoid repeated Camera.main calls
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("No main camera found! SpriteButtonAnimator may not work correctly.");
        }
    }

    private void OnDestroy()
    {
        // Clean up tweens to prevent warnings
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
            currentTween = null;
        }

        // Kill any remaining tweens on this transform
        transform.DOKill();
    }

    private void Update()
    {
        if (mainCamera == null) return; // Early exit if no camera

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#elif UNITY_IOS || UNITY_ANDROID
        HandleTouchInput();
#endif
    }

    private void HandleMouseInput()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            if (!isPointerOver)
            {
                isPointerOver = true;
                AnimateHoverIn();
            }

            if (Input.GetMouseButtonDown(0))
                AnimateClick();
        }
        else if (isPointerOver)
        {
            isPointerOver = false;
            AnimateHoverOut();
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            if (isPointerOver)
            {
                isPointerOver = false;
                AnimateHoverOut();
            }
            return;
        }

        Touch touch = Input.GetTouch(0);
        Vector3 touchPos = mainCamera.ScreenToWorldPoint(touch.position);
        Vector2 touchPos2D = new Vector2(touchPos.x, touchPos.y);
        RaycastHit2D hit = Physics2D.Raycast(touchPos2D, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            if (!isPointerOver)
            {
                isPointerOver = true;
                AnimateHoverIn();
            }

            if (touch.phase == TouchPhase.Began)
                AnimateClick();
        }
        else if (isPointerOver)
        {
            isPointerOver = false;
            AnimateHoverOut();
        }
    }

    private void AnimateHoverIn()
    {
        // Kill current tween safely
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        currentTween = transform.DOScale(originalScale * 1.1f, 0.2f)
            .SetEase(Ease.OutBack)
            .SetTarget(this); // Set target for proper cleanup
    }

    private void AnimateHoverOut()
    {
        // Kill current tween safely
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        currentTween = transform.DOScale(originalScale, 0.2f)
            .SetEase(Ease.OutBack)
            .SetTarget(this); // Set target for proper cleanup
    }

    private void AnimateClick()
    {
        // Kill current tween safely
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        // First scale down, then scale back up
        currentTween = transform.DOScale(originalScale * 0.9f, 0.1f)
            .SetEase(Ease.InOutQuad)
            .SetTarget(this) // Set target for proper cleanup
            .OnComplete(() =>
            {
                // Null check in callback to prevent errors if object is destroyed
                if (this != null && transform != null)
                {
                    currentTween = transform.DOScale(originalScale, 0.2f)
                        .SetEase(Ease.OutBack)
                        .SetTarget(this);
                }
            });
    }

    // Optional: Method to reset button state (useful for external control)
    public void ResetToOriginalScale()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        isPointerOver = false;
        transform.localScale = originalScale;
    }

    // Optional: Method to disable/enable button animations
    public void SetInteractable(bool interactable)
    {
        enabled = interactable;
        if (!interactable)
        {
            ResetToOriginalScale();
        }
    }
}