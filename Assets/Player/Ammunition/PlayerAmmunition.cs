using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class PlayerAmmunition : MonoBehaviour
{
    private readonly List<Projectile> m_ShootingQueue = new();
    public Projectile Next => m_ShootingQueue.FirstOrDefault();
    public IReadOnlyList<Projectile> ShootingQueue => m_ShootingQueue;

    public Action<Projectile> OnProjectileAdded;
    public Action<Projectile> OnProjectileRemoved;
    public Action OnShootingQueueChanged;

    public Projectile RemoveProjectile(Projectile p)
    {
        var idx = m_Projectiles.IndexOf(p);
        appendagePositions.RemoveAt(idx);
        appendageVelocities.RemoveAt(idx);
        m_Projectiles.Remove(p);
        m_ShootingQueue.Remove(p);
        HealthPool.Current = m_Projectiles.Count + 1;
        OnProjectileRemoved?.Invoke(p);
        OnShootingQueueChanged?.Invoke();
        return p;
    }
    public void DestroyProjectile(Projectile p)
    {
        RemoveProjectile(p);
        Destroy(p.gameObject);
    }
    private readonly List<Projectile> m_Projectiles = new();
    public IReadOnlyList<Projectile> Projectiles => m_Projectiles;
    public IReadOnlyList<Vector2> AppendagePositions => Projectiles.Select(p => transform.InverseTransformPoint(p.Rigidbody.position)._xz()).ToList();
    public IReadOnlyList<Vector2> AppendageVelocities => Projectiles.Select(p => transform.InverseTransformDirection(p.Rigidbody.linearVelocity)._xz()).ToList();/* appendageVelocities; */

    Cached<Rigidbody> cached_Rigidbody;
    Rigidbody Rigidbody => cached_Rigidbody[this];
    Cached<HealthPool> cached_HealthPool;
    HealthPool HealthPool => cached_HealthPool[this];

    void OnEnable()
    {
        if (HealthPool) HealthPool.OnModifiedWithSource += HealthChanged;
    }

    void OnDisable()
    {
        if (HealthPool) HealthPool.OnModifiedWithSource -= HealthChanged;
    }


    void OnCollisionEnter(Collision other)
    {
        var projectile = other.collider.GetComponent<Projectile>();
        if (!projectile) return;
        TryCollect(projectile);
    }
    void OnTriggerEnter(Collider other)
    {
        var projectile = other.GetComponent<Projectile>();
        if (!projectile) return;
        TryCollect(projectile);
    }

    public void TryCollect(Projectile projectile)
    {
        if (projectile.CurrentState != Projectile.State.Dropped) return;
        if (HealthPool.Current == HealthPool.Size) return; //full
        if (m_Projectiles.Contains(projectile)) return;
        m_Projectiles.Add(projectile);
        m_ShootingQueue.Add(projectile);
        OnShootingQueueChanged?.Invoke();
        appendagePositions.Add(transform.InverseTransformPoint(projectile.transform.position)._xz());
        appendageVelocities.Add(default);
        projectile.OnCollect(this);
        HealthPool.Current = m_Projectiles.Count + 1;
        OnProjectileAdded?.Invoke(projectile);
    }

    private List<Vector2> appendageVelocities = new();
    private List<Vector2> appendagePositions = new();

    [SerializeField] private float noiseScale = 1;
    [SerializeField] private float noiseStrength = 1;
    [SerializeField] private float floorStrength = 1;
    [SerializeField] private float boidStrength = 1;
    [SerializeField] private float coreAttractionStrength = 1;
    [SerializeField] private float drag = .95f;
    [SerializeField] private float sleepThreshold = .01f;
    [SerializeField] private float squishForce = 20f;
    [SerializeField] private float accelerationForce = 20f;
    private void FixedUpdate()
    {
        while (appendageVelocities.Count < appendagePositions.Count) appendageVelocities.Add(default);
        for (int i = 0; i < appendagePositions.Count; i++)
        {
            var pos = appendagePositions[i];
            appendageVelocities[i] += appendagePositions[i] * (1 - appendagePositions[i].sqrMagnitude) * Time.fixedDeltaTime * coreAttractionStrength;
            appendageVelocities[i] += -Rigidbody.linearVelocity._xz() * Time.fixedDeltaTime * accelerationForce;
            var perpendicular = Vector2.Perpendicular(Rigidbody.linearVelocity._xz());
            appendageVelocities[i] -= Vector2.Dot(appendagePositions[i], perpendicular.normalized) * perpendicular.normalized * squishForce * Time.fixedDeltaTime;
            for (int j = 0; j < appendagePositions.Count; j++)
            {
                var delta = j == i ? pos : pos - appendagePositions[j];
                var sqrMag = delta.sqrMagnitude;
                while ((sqrMag = delta.sqrMagnitude) == 0) delta = UnityEngine.Random.insideUnitCircle * 1f;
                if (sqrMag <= 0.1f)
                    sqrMag = .1f;
                delta = delta.normalized / sqrMag;
                if (delta.sqrMagnitude < 0.1f) delta = default;
                appendageVelocities[i] += delta * Time.fixedDeltaTime * boidStrength;
            }
            appendageVelocities[i] += Vector2.up * Mathf.Max(0, -appendagePositions[i].y * floorStrength) * Time.fixedDeltaTime;
            var perlinSamplePos = appendagePositions[i] / noiseScale / 1.43265f + new Vector2(-523.426346f, 735.2341f);
            var alpha = Mathf.PerlinNoise(perlinSamplePos.x - 267.923f, perlinSamplePos.y + 126.623f) * Mathf.PI * 2f;
            var intensity = Mathf.PerlinNoise(perlinSamplePos.x, perlinSamplePos.y);
            intensity *= intensity;
            var randomForce = new Vector2(Mathf.Cos(alpha), Mathf.Sin(alpha)) * intensity * noiseStrength;
            Debug.DrawLine(appendagePositions[i]._x0y() + transform.position, appendagePositions[i]._x0y() + transform.position + randomForce._x0y(), Color.red, 0f);
            appendageVelocities[i] += randomForce * Time.fixedDeltaTime;
            appendageVelocities[i] *= drag;
        }
        for (int i = 0; i < appendagePositions.Count; i++)
        {
            if (appendageVelocities[i].sqrMagnitude > sleepThreshold * sleepThreshold) appendagePositions[i] += appendageVelocities[i] * Time.fixedDeltaTime * 3f;
            var targetPos = transform.TransformPoint(Vector3.ClampMagnitude(appendagePositions[i]._x0y(), 6));
            var delta = targetPos - m_Projectiles[i].transform.position;
            m_Projectiles[i].Collider.enabled = delta.sqrMagnitude <= 3 * 3;
            var force = delta / Time.fixedDeltaTime - m_Projectiles[i].Rigidbody.linearVelocity;
            if (float.IsNaN(force.x) || float.IsNaN(force.y) || float.IsNaN(force.z)) continue;
            m_Projectiles[i].Rigidbody.AddForce(force, ForceMode.VelocityChange);
        }
    }

    private void HealthChanged(int change, HealthEvent healthEvent)
    {
        if (healthEvent == null) return; //manual update, ignore
        if (change >= 0) return; //ignore
        var projectilesToDrop = -(Mathf.Max(1, m_Projectiles.Count) - HealthPool.Current - 1);
        for (int i = 0; i < projectilesToDrop; i++)
        {
            var hitPos = healthEvent.Source.transform.position;
            var closestToHitPos = m_Projectiles.OrderBy(p => (p.transform.position - hitPos).sqrMagnitude).First();
            DestroyProjectile(closestToHitPos);
        }
    }
}