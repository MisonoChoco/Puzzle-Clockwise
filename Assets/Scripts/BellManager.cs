using System.Collections.Generic;
using UnityEngine;

public class BellManager : MonoBehaviour
{
    public static BellManager Instance { get; private set; }

    private List<Bell> bells = new List<Bell>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterBell(Bell bell)
    {
        if (!bells.Contains(bell))
            bells.Add(bell);
    }

    public bool AreAllBellsActivated()
    {
        foreach (var bell in bells)
        {
            if (!bell.IsActive)
                return false;
        }
        return true;
    }

    public void ResetBells()
    {
        foreach (var bell in bells)
        {
            bell.SetActive(false);
        }
    }
}