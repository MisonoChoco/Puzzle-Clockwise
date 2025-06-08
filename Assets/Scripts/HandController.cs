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
    private Color originalColor;

    public AudioSource rotatingaudioSource;
    public AudioSource anchoraudioSource;
    public AudioClip rotatingClip;
    public AudioClip anchorClip;

    [Header("Afterimage Settings")]
    public GameObject afterimagePrefab;

    public float afterimageSpawnInterval = 0.05f;
    public float afterimageLifetime = 0.4f;

    private Coroutine afterimageRoutine;

    private void Start()
    {
        currentAnchor = pivotStart;
        currentPivot = pivotStart;

        pivotASprite = pivotStart.GetComponentInChildren<SpriteRenderer>();
        pivotBSprite = pivotEnd.GetComponentInChildren<SpriteRenderer>();
        if (pivotASprite != null)
            originalColor = pivotASprite.color;
        if (pivotBSprite != null)
            originalColor = pivotBSprite.color;
    }

    private void OnDestroy()
    {
        if (rotationTween != null && rotationTween.IsActive())
            rotationTween.Kill();
    }

    public void RotateAround(Vector3 pivotPosition)
    {
        StopRotation(); // Stop any existing rotation

        rotationTween = DOVirtual.Float(0f, 360f, 360f / rotationSpeed, value =>
        {
            transform.RotateAround(pivotPosition, Vector3.forward, rotationSpeed * Time.deltaTime);
        })
        .SetEase(Ease.Linear)
        .SetLoops(-1);

        rotatingaudioSource.clip = rotatingClip;
        if (!rotatingaudioSource.isPlaying)
        {
            rotatingaudioSource.Play();
        }

        if (afterimageRoutine == null)
            afterimageRoutine = StartCoroutine(SpawnAfterimages());
    }

    private IEnumerator SpawnAfterimages()
    {
        while (true)
        {
            GameObject afterimage = Instantiate(afterimagePrefab, transform.position, transform.rotation);
            Destroy(afterimage, afterimageLifetime);
            yield return new WaitForSeconds(afterimageSpawnInterval);
        }
    }

    public void StopRotation()
    {
        if (rotationTween != null && rotationTween.IsActive())
            rotationTween.Kill();

        StartCoroutine(WaitForAudio());

        if (afterimageRoutine != null)
        {
            StopCoroutine(afterimageRoutine);
            afterimageRoutine = null;
        }
    }

    public void SetAnchor(Transform newAnchor)
    {
        currentAnchor = newAnchor;
        currentPivot = GetCorrespondingPivot(newAnchor);
        ReanchorTo(newAnchor);

        if (pivotASprite != null)
        {
            pivotASprite.DOColor(Color.cyan, 0.3f).OnComplete(() =>
            {
                pivotASprite.DOColor(originalColor, 0.3f);
            });
        }

        if (pivotBSprite != null)
        {
            pivotBSprite.DOColor(Color.cyan, 0.3f).OnComplete(() =>
            {
                pivotBSprite.DOColor(originalColor, 0.3f);
            });
        }

        //anchoraudioSource.clip = anchorClip;
        //if (!anchoraudioSource.isPlaying)
        //{
        //    anchoraudioSource.Play();
        //    StartCoroutine(WaitForAudio());
        //}
    }

    private IEnumerator WaitForAudio()
    {
        if (rotatingaudioSource.isPlaying)
        {
            yield return new WaitWhile(() => rotatingaudioSource.isPlaying);
            rotatingaudioSource.Stop(); // Stop rotating sound when rotation ends
            anchoraudioSource.PlayOneShot(anchoraudioSource.clip); // Play anchor sound after rotation stops
        }
        if (anchoraudioSource.isPlaying)
        {
            yield return new WaitWhile(() => anchoraudioSource.isPlaying);
            anchoraudioSource.Stop(); // Stop anchor sound after it finishes playing
        }
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
            float distStart = Vector3.Distance(anchor.position, pivotStart.position);
            float distEnd = Vector3.Distance(anchor.position, pivotEnd.position);
            return distStart < distEnd ? pivotStart : pivotEnd;
        }
    }

    public void ReanchorTo(Transform newAnchor)
    {
        // Move the entire hand so that the hand's currentPivot aligns exactly with newAnchor

        Vector3 handPivotWorldPos = currentPivot.position;
        Vector3 anchorPos = newAnchor.position;

        Vector3 offset = anchorPos - handPivotWorldPos;
        transform.DOMove(transform.position + offset, 0.15f).SetEase(Ease.OutBack);
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