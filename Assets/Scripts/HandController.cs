using UnityEngine;
using DG.Tweening;

public class HourHandController : MonoBehaviour
{
    public Transform pivotStart;
    public Transform pivotEnd;
    public float rotationSpeed = 90f; // Degrees per second

    private Transform currentAnchor;  // The map pivot we're anchored to
    private Transform currentPivot;   // The hand's pivot corresponding to currentAnchor
    private Tween rotationTween;

    private void Start()
    {
        currentAnchor = pivotStart;
        currentPivot = pivotStart; // start anchored to pivotStart
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
    }

    public void StopRotation()
    {
        if (rotationTween != null && rotationTween.IsActive())
            rotationTween.Kill();
    }

    public void SetAnchor(Transform newAnchor)
    {
        currentAnchor = newAnchor;
        currentPivot = GetCorrespondingPivot(newAnchor);
        ReanchorTo(newAnchor);
    }

    public Transform GetFreePivot()
    {
        return currentPivot == pivotStart ? pivotEnd : pivotStart;
    }

    private Transform GetCorrespondingPivot(Transform anchor)
    {
        // Assuming the map pivot transform matches pivotStart or pivotEnd directly
        // Or you can add your own matching logic here if needed

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
}