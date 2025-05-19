using UnityEngine;

public class BrainEnemySpikeAttack : MonoBehaviour
{
    Vector2 noiseOffset;
    [SerializeField] float speed = 8f;
    //Movetowards player in a zigzag pattern

    [Tooltip("How far the attacks should be apart from each other")]
    [SerializeField] float minDistance = 1f;
    [SerializeField] private ContactDamage AttackPrefab;

    Vector2 lastPosition;
    void TryTriggerHit()
    {
        var selfPos = transform.position._xz();
        var delta = selfPos - lastPosition;
        if (delta.sqrMagnitude <= minDistance * minDistance) return;
        lastPosition = selfPos;
        var contactDamage = Instantiate(AttackPrefab, transform.position, Quaternion.identity);
        if (contactDamage) contactDamage.OnTriggered += () => { if (gameObject != null) Destroy(gameObject); };
    }

    void Awake() => noiseOffset = Random.insideUnitCircle * 1000;
    void FixedUpdate()
    {
        var noisePos = transform.position._xz() / 3f + noiseOffset;
        var offset = new Vector2(Mathf.PerlinNoise(noisePos.x, noisePos.y), Mathf.PerlinNoise(noisePos.x - 512.754312f, noisePos.y + 74.1236f)) * 2 - Vector2.one;
        var angle = Mathf.Atan2(offset.y, offset.x);
        angle = Mathf.RoundToInt(angle / Mathf.PI * 2f) / 2f * Mathf.PI;
        offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        var deltaFromPlayer = (EnemyBrainUtils.PlayerPosition() - transform.position._xz()).normalized + offset * .5f;
        transform.position += deltaFromPlayer._x0y().normalized * speed * Time.fixedDeltaTime;
        TryTriggerHit();
    }
}
