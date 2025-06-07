using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    public Camera mainCamera;
    public HourHandController hourHand;

    public LayerMask clickableLayers;

    private void Start()
    {
    }

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // If not assigned in inspector, assign to Default layer only
        if (clickableLayers == 0)
        {
            clickableLayers = LayerMask.GetMask("Default");
        }
    }

    private void Update()
    {
        if (hourHand == null)
        {
            hourHand = Object.FindFirstObjectByType<HourHandController>();
            if (hourHand == null) return;
        }

        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            Vector2 inputPosition = Input.mousePosition;
            if (Input.touchCount > 0)
                inputPosition = Input.GetTouch(0).position;

            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(inputPosition);

            // Raycast only on clickableLayers (which excludes PivotPoint)
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, clickableLayers);

            if (hit.collider != null)
            {
                Debug.Log("2D Raycast hit: " + hit.collider.name);

                if (hit.collider.CompareTag("HandPivot"))
                {
                    Debug.Log("Clicked on a HandPivot!");
                    hourHand.RotateAround(hit.collider.transform.position);
                }
            }
            else
            {
                Debug.Log("2D Raycast hit nothing.");
            }
        }
    }
}