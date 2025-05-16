using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPositionMarker : MonoBehaviour
{
    public readonly static SortedList<int, HashSet<PlayerPositionMarker>> ActiveMarkers = new();
    public static Vector2 CurrentPosition => ActiveMarkers.Count == 0 ? default : (ActiveMarkers.Values[^1].FirstOrDefault()?.transform?.position ?? default)._xz();
    [Tooltip("Higher values are better")][SerializeField] private int Priority;
    private void OnEnable() => AddSelf();
    private void OnDisable() => RemoveSelf();
    private void RemoveSelf()
    {
        if (!ActiveMarkers.ContainsKey(Priority)) return;
        ActiveMarkers[Priority].Remove(this);
        if (ActiveMarkers[Priority].Count == 0) ActiveMarkers.Remove(Priority);
    }
    private void AddSelf()
    {
        if (ActiveMarkers.ContainsKey(Priority))
            ActiveMarkers[Priority].Add(this);
        else ActiveMarkers[Priority] = new() { this };
    }
}
