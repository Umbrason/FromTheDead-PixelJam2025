using UnityEngine;

public class SpawnPuddle : MonoBehaviour
{
    Vector2 lastPosition;
    [Tooltip("How far the puddles should be apart from each other")]
    [SerializeField] float minDistance = .75f;
    [SerializeField] private GameObject PuddlePrefab;

    void FixedUpdate()
    {
        var selfPos = transform.position._xz();
        var delta = selfPos - lastPosition;
        if (delta.sqrMagnitude < minDistance * minDistance) return;
        lastPosition = selfPos;
        Instantiate(PuddlePrefab, transform.position, Quaternion.identity);
    }
}
