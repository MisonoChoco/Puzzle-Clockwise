using UnityEngine;

public class PivotPlacer : MonoBehaviour
{
    public GameObject pivotPrefab;

    private GameObject spawnedPivot;
    private bool pivotPlaced = false;
    private bool isDragging = false;

    private void Update()
    {
        if (!pivotPlaced && Input.GetKeyDown(KeyCode.G))
        {
            SpawnPivot();
        }

        if (spawnedPivot != null)
        {
            HandleDrag();
        }
    }

    private void SpawnPivot()
    {
        Vector3 spawnPos = GetMouseWorldPosition();
        spawnedPivot = Instantiate(pivotPrefab, spawnPos, Quaternion.identity);
        pivotPlaced = true;
        isDragging = true;
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPosition();
            spawnedPivot.transform.position = mousePos;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = 10f; // Distance from the camera
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}