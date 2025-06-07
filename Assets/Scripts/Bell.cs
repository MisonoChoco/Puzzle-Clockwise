using UnityEngine;
using System.Collections;

public class Bell : MonoBehaviour
{
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;

    private bool isActive = false;
    private bool isCooldown = false;
    private float cooldownTime = 1.1f;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        BellManager.Instance?.RegisterBell(this);
        UpdateVisual();
    }

    public void SetActive(bool active)
    {
        if (isActive == active) return;

        isActive = active;
        UpdateVisual();

        StartCoroutine(CooldownRoutine());
    }

    public bool IsActive => isActive;

    private void UpdateVisual()
    {
        if (sr != null)
        {
            sr.sprite = isActive ? onSprite : offSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCooldown) return;

        if (other.CompareTag("PlayerHand"))
        {
            SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isCooldown) return;

        if (other.CompareTag("PlayerHand"))
        {
            SetActive(false);
        }
    }

    private IEnumerator CooldownRoutine()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }
}