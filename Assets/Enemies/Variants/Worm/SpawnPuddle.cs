using UnityEngine;

public class SpawnPuddle : MonoBehaviour
{
    Vector2 lastPosition;
    [Tooltip("How far the puddles should be apart from each other")]
    [SerializeField] float minDistance = 1f;
    [SerializeField] private AcidPuddleDespawnAfterTimeOrOwnerDeath PuddlePrefab;

    void FixedUpdate()
    {
        var selfPos = transform.position._xz();
        var delta = selfPos - lastPosition;
        if (delta.sqrMagnitude <= minDistance * minDistance) return;
        lastPosition = selfPos;
        var instance = Instantiate(PuddlePrefab, transform.position, Quaternion.identity);
        instance.Owner = gameObject;

    }
}
