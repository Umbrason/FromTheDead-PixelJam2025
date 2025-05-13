using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class PlayerAmmunition : MonoBehaviour
{
    private readonly List<Projectile> ShootingQueue = new();
    public Projectile Next => ShootingQueue.FirstOrDefault();
    public Projectile RemoveProjectile(Projectile p)
    {
        var idx = m_Projectiles.IndexOf(p);
        appendagePositions.RemoveAt(idx);
        appendageForces.RemoveAt(idx);
        m_Projectiles.Remove(p);
        HealthPool.RegisterHealthEvent(HealthEvent.Damage(1));
        ShootingQueue.Remove(p);
        return p;
    }
    private readonly List<Projectile> m_Projectiles = new();
    public IReadOnlyList<Projectile> Projectiles => m_Projectiles;
    public IReadOnlyList<Vector2> AppendagePositions => appendagePositions;

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

    void OnTriggerEnter(Collider other)
    {
        var projectile = other.GetComponent<Projectile>();
        if (!(projectile?.enabled ?? true)) Collect(projectile);
    }

    public void Collect(Projectile projectile)
    {
        if (m_Projectiles.Contains(projectile)) return;
        projectile.transform.SetLayerRecursive(LayerMask.NameToLayer("Player"));
        m_Projectiles.Add(projectile);
        projectile.transform.SetParent(transform, true);
        ShootingQueue.Add(projectile);
        appendagePositions.Add(projectile.transform.localPosition._xz());
        HealthPool.RegisterHealthEvent(HealthEvent.Heal(1));
    }

    private List<Vector2> appendageForces = new();
    private List<Vector2> appendagePositions = new();

    [SerializeField] private float noiseScale = 1;
    [SerializeField] private float noiseStrength = 1;
    [SerializeField] private float floorStrength = 1;
    [SerializeField] private float boidStrength = 1;
    [SerializeField] private float coreAttractionStrength = 1;
    [SerializeField] private float drag = .95f;
    private void FixedUpdate()
    {
        while (appendageForces.Count < appendagePositions.Count) appendageForces.Add(default);
        for (int i = 0; i < appendagePositions.Count; i++)
        {
            var pos = appendagePositions[i];
            appendageForces[i] += appendagePositions[i] * (1 - appendagePositions[i].sqrMagnitude) * Time.fixedDeltaTime * coreAttractionStrength;
            appendageForces[i] += Vector2.ClampMagnitude(Rigidbody.linearVelocity._xz(), 20) * Time.fixedDeltaTime * -2;
            for (int j = 0; j < appendagePositions.Count; j++)
            {
                var delta = j == i ? pos : pos - appendagePositions[j];
                var sqrMag = delta.sqrMagnitude;
                if (sqrMag <= 0.1f)
                    sqrMag = .1f;
                delta = delta.normalized / sqrMag;
                if (delta.sqrMagnitude < 0.1f) delta = default;
                appendageForces[i] += delta * Time.fixedDeltaTime * boidStrength;
            }
            appendageForces[i] += Vector2.up * Mathf.Max(0, -appendagePositions[i].y * floorStrength) * Time.fixedDeltaTime;
            var perlinSamplePos = appendagePositions[i] / noiseScale / 1.43265f + new Vector2(-523.426346f, 735.2341f);
            var alpha = Mathf.PerlinNoise(perlinSamplePos.x - 267.923f, perlinSamplePos.y + 126.623f) * Mathf.PI * 2f;
            var intensity = Mathf.PerlinNoise(perlinSamplePos.x, perlinSamplePos.y);
            intensity *= intensity;
            var randomForce = new Vector2(Mathf.Cos(alpha), Mathf.Sin(alpha)) * intensity * noiseStrength;
            Debug.DrawLine(appendagePositions[i]._x0y() + transform.position, appendagePositions[i]._x0y() + transform.position + randomForce._x0y(), Color.red, 0f);
            appendageForces[i] += randomForce * Time.fixedDeltaTime;
            appendageForces[i] *= drag;
        }
        for (int i = 0; i < appendagePositions.Count; i++)
        {
            if (appendageForces[i].sqrMagnitude > .05f) appendagePositions[i] += appendageForces[i] * Time.fixedDeltaTime * 3f;
            m_Projectiles[i].transform.localPosition = appendagePositions[i]._x0y();
        }
    }

    private void HealthChanged(int change, HealthEvent healthEvent)
    {
        if (change >= 0) return; //ignore
        var projectilesToDrop = HealthPool.Current - 1 - m_Projectiles.Count;
        for (int i = 0; i < projectilesToDrop; i++)
        {
            var hitPos = healthEvent.Source.transform.position;
            var closestToHitPos = m_Projectiles.OrderBy(p => (p.transform.position - hitPos).sqrMagnitude).First();
            RemoveProjectile(closestToHitPos);
        }
    }
}