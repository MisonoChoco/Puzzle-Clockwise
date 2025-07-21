using UnityEngine;
using DG.Tweening;
using System.Collections;

public class HourHandController : MonoBehaviour
{
    public Transform pivotStart;
    public Transform pivotEnd;
    public float rotationSpeed = 90f;

    private Transform currentAnchor;
    private Transform currentPivot;
    private Tween rotationTween;

    public SpriteRenderer pivotASprite;
    public SpriteRenderer pivotBSprite;
    private Color originalColorA;
    private Color originalColorB;

    public AudioSource rotatingaudioSource;
    public AudioSource anchoraudioSource;
    public AudioClip rotatingClip;
    public AudioClip anchorClip;

    [Header("Afterimage Settings")]
    public GameObject afterimagePrefab;

    public float afterimageSpawnInterval = 0.05f;
    public float afterimageLifetime = 0.4f;

    private Coroutine afterimageRoutine;
    private Coroutine audioWaitRoutine;

    private void Start()
    {
        currentAnchor = pivotStart;
        currentPivot = pivotStart;

        // Get sprite renderers if not assigned
        if (pivotASprite == null && pivotStart != null)
            pivotASprite = pivotStart.GetComponentInChildren<SpriteRenderer>();
        if (pivotBSprite == null && pivotEnd != null)
            pivotBSprite = pivotEnd.GetComponentInChildren<SpriteRenderer>();

        // Store original colors separately
        if (pivotASprite != null)
            originalColorA = pivotASprite.color;
        if (pivotBSprite != null)
            originalColorB = pivotBSprite.color;
    }

    private void OnDestroy()
    {
        // Clean up all tweens and coroutines
        if (rotationTween != null && rotationTween.IsActive())
        {
            rotationTween.Kill();
            rotationTween = null;
        }

        // Stop coroutines safely
        if (afterimageRoutine != null)
        {
            StopCoroutine(afterimageRoutine);
            afterimageRoutine = null;
        }

        if (audioWaitRoutine != null)
        {
            StopCoroutine(audioWaitRoutine);
            audioWaitRoutine = null;
        }

        // Kill any remaining DOTween tweens on this gameobject
        transform.DOKill();
        if (pivotASprite != null)
            pivotASprite.DOKill();
        if (pivotBSprite != null)
            pivotBSprite.DOKill();
    }

    public void RotateAround(Vector3 pivotPosition)
    {
        StopRotation(); // Stop any existing rotation

        // Create rotation tween with proper cleanup
        rotationTween = DOVirtual.Float(0f, 360f, 360f / rotationSpeed, value =>
        {
            if (this != null && transform != null) // Null check for safety
            {
                transform.RotateAround(pivotPosition, Vector3.forward, rotationSpeed * Time.deltaTime);
            }
        })
        .SetEase(Ease.Linear)
        .SetLoops(-1)
        .SetTarget(this); // Set target for proper cleanup

        // Handle rotating audio - play one shot instead of looping
        if (rotatingaudioSource != null && rotatingClip != null)
        {
            if (!rotatingaudioSource.isPlaying)
            {
                rotatingaudioSource.PlayOneShot(rotatingClip);
            }
        }

        // Start afterimage spawning
        if (afterimagePrefab != null && afterimageRoutine == null)
            afterimageRoutine = StartCoroutine(SpawnAfterimages());
    }

    private IEnumerator SpawnAfterimages()
    {
        while (afterimagePrefab != null)
        {
            if (this != null && transform != null)
            {
                GameObject afterimage = Instantiate(afterimagePrefab, transform.position, transform.rotation);
                if (afterimage != null)
                    Destroy(afterimage, afterimageLifetime);
            }
            yield return new WaitForSeconds(afterimageSpawnInterval);
        }
    }

    public void StopRotation()
    {
        // Kill rotation tween safely
        if (rotationTween != null && rotationTween.IsActive())
        {
            rotationTween.Kill();
            rotationTween = null;
        }

        // Stop audio wait routine if running
        if (audioWaitRoutine != null)
        {
            StopCoroutine(audioWaitRoutine);
        }
        audioWaitRoutine = StartCoroutine(WaitForAudio());

        // Stop afterimage spawning
        if (afterimageRoutine != null)
        {
            StopCoroutine(afterimageRoutine);
            afterimageRoutine = null;
        }
    }

    public void SetAnchor(Transform newAnchor)
    {
        if (newAnchor == null) return; // Safety check

        currentAnchor = newAnchor;
        currentPivot = GetCorrespondingPivot(newAnchor);
        ReanchorTo(newAnchor);

        // Kill any existing color tweens before starting new ones
        if (pivotASprite != null)
        {
            pivotASprite.DOKill();
            pivotASprite.DOColor(Color.cyan, 0.3f).OnComplete(() =>
            {
                if (pivotASprite != null) // Null check in callback
                    pivotASprite.DOColor(originalColorA, 0.3f);
            }).SetTarget(pivotASprite);
        }

        if (pivotBSprite != null)
        {
            pivotBSprite.DOKill();
            pivotBSprite.DOColor(Color.cyan, 0.3f).OnComplete(() =>
            {
                if (pivotBSprite != null) // Null check in callback
                    pivotBSprite.DOColor(originalColorB, 0.3f);
            }).SetTarget(pivotBSprite);
        }
    }

    private IEnumerator WaitForAudio()
    {
        // Wait for rotating audio to finish
        if (rotatingaudioSource != null && rotatingaudioSource.isPlaying)
        {
            yield return new WaitWhile(() => rotatingaudioSource != null && rotatingaudioSource.isPlaying);

            if (rotatingaudioSource != null)
                rotatingaudioSource.Stop();

            // Play anchor sound after rotation stops
            if (anchoraudioSource != null && anchorClip != null)
                anchoraudioSource.PlayOneShot(anchorClip);
        }

        // Wait for anchor audio to finish
        if (anchoraudioSource != null && anchoraudioSource.isPlaying)
        {
            yield return new WaitWhile(() => anchoraudioSource != null && anchoraudioSource.isPlaying);

            if (anchoraudioSource != null)
                anchoraudioSource.Stop();
        }

        audioWaitRoutine = null;
    }

    public Transform GetFreePivot()
    {
        return currentPivot == pivotStart ? pivotEnd : pivotStart;
    }

    private Transform GetCorrespondingPivot(Transform anchor)
    {
        if (anchor == pivotStart)
            return pivotStart;
        else if (anchor == pivotEnd)
            return pivotEnd;
        else
        {
            // fallback: pick the closest pivot to anchor
            if (pivotStart != null && pivotEnd != null && anchor != null)
            {
                float distStart = Vector3.Distance(anchor.position, pivotStart.position);
                float distEnd = Vector3.Distance(anchor.position, pivotEnd.position);
                return distStart < distEnd ? pivotStart : pivotEnd;
            }
            return pivotStart; // Fallback to start if null checks fail
        }
    }

    public void ReanchorTo(Transform newAnchor)
    {
        if (newAnchor == null || currentPivot == null) return; // Safety checks

        // Kill any existing move tween
        transform.DOKill();

        // Move the entire hand so that the hand's currentPivot aligns exactly with newAnchor
        Vector3 handPivotWorldPos = currentPivot.position;
        Vector3 anchorPos = newAnchor.position;
        Vector3 offset = anchorPos - handPivotWorldPos;

        transform.DOMove(transform.position + offset, 0.15f)
            .SetEase(Ease.OutBack)
            .SetTarget(this); // Set target for proper cleanup
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (pivotStart != null && pivotEnd != null)
        {
            DrawArcPath(pivotStart.position, pivotEnd.position, Color.red);
            DrawArcPath(pivotEnd.position, pivotStart.position, Color.blue);
        }
    }

    private void DrawArcPath(Vector3 anchor, Vector3 orbitingPoint, Color color)
    {
        const int segments = 64;
        float radius = Vector3.Distance(anchor, orbitingPoint);

        Vector3 offset = orbitingPoint - anchor;
        float startAngle = Mathf.Atan2(offset.y, offset.x);

        Gizmos.color = color;
        Vector3 prevPoint = anchor + new Vector3(Mathf.Cos(startAngle), Mathf.Sin(startAngle), 0f) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = startAngle + (i * Mathf.PI * 2f / segments);
            Vector3 nextPoint = anchor + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        // Optional markers
        Gizmos.DrawSphere(anchor, 0.02f);
        Gizmos.DrawSphere(orbitingPoint, 0.02f);
    }

#endif
}