using UnityEngine;
using DG.Tweening;

public class AfterimageFade : MonoBehaviour
{
    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, 0.6f); // initial faded alpha
            sr.DOFade(0f, 0.4f); // fade to 0 over lifetime
        }
    }
}