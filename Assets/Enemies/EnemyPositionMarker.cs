using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPositionMarker : MonoBehaviour
{
    public readonly static HashSet<EnemyPositionMarker> ActiveMarkers = new();
    public static Vector2 ClosestFrom(Vector2 from) => ActiveMarkers.OrderBy(p => (p.Position - from).sqrMagnitude).FirstOrDefault()?.Position ?? from;
    public static int EnemyCount => ActiveMarkers.Count;
    public Vector2 Position => transform.position._xz();
    private void OnEnable() => AddSelf();
    private void OnDisable() => RemoveSelf();
    private void RemoveSelf() => ActiveMarkers.Remove(this);
    private void AddSelf() => ActiveMarkers.Add(this);
}
