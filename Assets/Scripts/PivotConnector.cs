using System.Collections;
using UnityEngine;

public class PivotConnector : MonoBehaviour
{
    public HourHandController hourHand;

    private void OnTriggerEnter2D(Collider2D other)

    {
        if (other.CompareTag("HandPivot"))
        {
            Debug.Log("Pivot connected");
            StartCoroutine(HandlePivotConnection());
        }
    }

    private IEnumerator HandlePivotConnection()
    {
        yield return new WaitForSeconds(0.1f);

        hourHand.SetAnchor(transform); // Update anchor
        hourHand.StopRotation();       // Stop spin
    }

    private void Update()
    {
        if (hourHand == null)
        {
            hourHand = Object.FindFirstObjectByType<HourHandController>();
            if (hourHand == null) return;
        }
    }
}