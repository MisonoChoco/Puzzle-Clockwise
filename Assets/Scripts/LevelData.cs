using UnityEngine;

[CreateAssetMenu(menuName = "Clockwise/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Player Hand Settings")]
    public Vector3 handStartPosition;

    public float handStartRotation;

    [Header("Map Pivots")]
    public GameObject[] mapPivotPrefabs;

    public Vector3[] mapPivotPositions;

    [Header("Win Pivot")]
    public GameObject winPivotPrefab;

    public Vector3 winPivotPosition;

    [Header("Shadow Hand")]
    public GameObject shadowHandPrefab;

    public Vector3 shadowHandPosition;
    public float shadowHandRotation;

    [Header("Bells")]
    public GameObject[] bellPrefabs;

    public Vector3[] bellPositions;
}