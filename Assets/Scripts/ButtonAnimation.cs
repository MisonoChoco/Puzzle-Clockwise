using UnityEngine;
using DG.Tweening;

public class SpriteButtonAnimator : MonoBehaviour
{
    private Vector3 originalScale;
    private Tween currentTween;
    private bool isPointerOver = false;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#elif UNITY_IOS || UNITY_ANDROID
        HandleTouchInput();
#endif
    }

    private void HandleMouseInput()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
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
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);
    }

    private void AnimateHoverOut()
    {
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
    }

    private void AnimateClick()
    {
        currentTween?.Kill();
        transform.DOScale(originalScale * 0.9f, 0.1f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack));
    }
}