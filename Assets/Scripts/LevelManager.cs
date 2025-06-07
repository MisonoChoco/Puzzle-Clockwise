using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class LevelManager : MonoBehaviour
{
    public GameObject playerHandPrefab;
    public LevelData currentLevelData;

    [HideInInspector] public Transform levelParent;

    private GameObject playerHand;

    private LevelTimer timer;
    public GameObject failScreenPrefab;

    private void Start()
    {
        if (LevelLoader.SelectedLevelData != null)
        {
            LoadLevel(LevelLoader.SelectedLevelData);
        }
        else
        {
            Debug.LogError("No LevelData was set");
            SceneManager.LoadScene("Lobby");
        }

        timer = Object.FindFirstObjectByType<LevelTimer>();
        if (timer != null)
            timer.OnTimeOut.AddListener(OnTimeOut);
    }

    public void OnTimeOut()
    {
        Instantiate(failScreenPrefab, Vector3.zero, Quaternion.identity);
    }

    public void LoadLevel(LevelData levelData)
    {
        // Destroy old level parent if it exists
        if (levelParent != null)
            Destroy(levelParent.gameObject);

        // Create new parent
        levelParent = new GameObject("Level").transform;

        // Spawn player hand with animation
        playerHand = Instantiate(playerHandPrefab, levelData.handStartPosition, Quaternion.identity, levelParent);
        playerHand.transform.rotation = Quaternion.Euler(0, 0, levelData.handStartRotation);
        AnimateObjectIn(playerHand);

        Transform pivotA = playerHand.transform.Find("PivotA");
        Transform pivotB = playerHand.transform.Find("PivotB");

        if (pivotA != null)
            pivotA.localPosition = new Vector3(0f, 2f, 0f);

        if (pivotB != null)
            pivotB.localPosition = new Vector3(0f, -2f, 0f);

        //if (levelData.playerHandPrefab != null)
        //{
        //    GameObject shadow = Instantiate(levelData.playerHandPrefab, levelData.handStartPosition, Quaternion.Euler(0, 0, levelData.handStartRotation), levelParent);
        //    AnimateObjectIn(shadow);
        //}

        // Map pivots
        for (int i = 0; i < levelData.mapPivotPrefabs.Length; i++)
        {
            GameObject pivot = Instantiate(levelData.mapPivotPrefabs[i], levelData.mapPivotPositions[i], Quaternion.identity, levelParent);
            AnimateObjectIn(pivot);
        }

        // Win pivot
        if (levelData.winPivotPrefab != null)
        {
            GameObject winPivot = Instantiate(levelData.winPivotPrefab, levelData.winPivotPosition, Quaternion.identity, levelParent);
            AnimateObjectIn(winPivot);
        }

        // Shadow hand
        if (levelData.shadowHandPrefab != null)
        {
            GameObject shadow = Instantiate(levelData.shadowHandPrefab, levelData.shadowHandPosition, Quaternion.Euler(0, 0, levelData.shadowHandRotation), levelParent);
            AnimateObjectIn(shadow);
        }

        // Bells
        if (levelData.bellPrefabs != null && levelData.bellPrefabs.Length > 0)
        {
            for (int i = 0; i < levelData.bellPrefabs.Length; i++)
            {
                GameObject bell = Instantiate(levelData.bellPrefabs[i], levelData.bellPositions[i], Quaternion.identity, levelParent);
                AnimateObjectIn(bell);
            }
        }
    }

    public IEnumerator TransitionOutOldLevel(System.Action onComplete)
    {
        if (levelParent == null || levelParent.childCount == 0)
        {
            onComplete?.Invoke();
            yield break;
        }

        List<Transform> children = new List<Transform>();
        foreach (Transform child in levelParent)
        {
            children.Add(child);
        }

        float duration = 1.2f;
        float delayStep = 0.07f;
        float baseDistance = 300f;

        for (int i = 0; i < children.Count; i++)
        {
            Transform obj = children[i];
            float delay = i * delayStep;

            // Random direction for movement
            Vector3 randomDir = Random.insideUnitCircle.normalized * baseDistance;

            // Move out with stagger and easing
            obj.DOMove(obj.position + randomDir, duration)
                .SetEase(Ease.OutQuad)
                .SetDelay(delay);

            // Rotate while moving out
            obj.DORotate(new Vector3(0, 0, Random.Range(90f, 180f)), duration)
                .SetEase(Ease.OutCubic)
                .SetDelay(delay);

            // Scale down while moving out
            obj.DOScale(0.3f, duration)
                .SetEase(Ease.OutCubic)
                .SetDelay(delay);

            //fade out sprite alpha

            SpriteRenderer sr = obj.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.DOFade(0f, duration)
                  .SetDelay(delay)
                  .SetEase(Ease.OutQuad);
            }

            // Destroy after animation + delay + small buffer
            Destroy(obj.gameObject, duration + delay + 0.1f);
        }

        // Wait for all staggered animations to finish before continuing
        yield return new WaitForSeconds(duration + (children.Count - 1) * delayStep + 0.2f);

        onComplete?.Invoke();
    }

    private void AnimateObjectIn(GameObject obj)
    {
        float delay = Random.Range(0f, 0.15f);

        // Get original scale
        Vector3 originalScale = obj.transform.localScale;

        // Set initial scale to zero
        obj.transform.localScale = Vector3.zero;

        // Fade-out if sprite exists
        DOVirtual.DelayedCall(delay, () =>
        {
            // Animate scale back to original size
            obj.transform.DOScale(originalScale, 0.5f)
                .SetEase(Ease.OutBack);

            // Fade-in if sprite exists
            SpriteRenderer sr = obj.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                Color startColor = sr.color;
                startColor.a = 0f;
                sr.color = startColor;

                sr.DOFade(1f, 0.5f);
            }
        });
    }

    private void LateUpdate() //makeshift method to ensure pivots are set after all objects are loaded
    {
        if (playerHand != null)
        {
            Transform pivotA = playerHand.transform.Find("PivotA");
            Transform pivotB = playerHand.transform.Find("PivotB");

            if (pivotA != null && pivotB != null)
            {
                pivotA.localPosition = new Vector3(0f, 2f, 0f);
                pivotB.localPosition = new Vector3(0f, -2f, 0f);
            }
        }
    }
}