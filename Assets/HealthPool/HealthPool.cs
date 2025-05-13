using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthPool : MonoBehaviour
{
    [field: SerializeField] public int Size { get; private set; }
    [field: SerializeField] private int m_Current;
    public int Current
    {
        get => m_Current; set
        {
            if (value == m_Current) return;
            var prevHealth = m_Current;
            this.m_Current = Mathf.Clamp(value, 0, Size);
            OnModified?.Invoke(m_Current - prevHealth);
            if (Current == 0) OnDepleted?.Invoke();
        }
    }
    public event Action<int, HealthEvent> OnModifiedWithSource;
    public event Action<int> OnModified;
    public event Action OnDepleted;
    void FixedUpdate()
    {
        ProcessHealthEvents();
    }
    public void OnEnable() => Current = Current;

    private readonly Dictionary<Guid, (HealthEvent healthEvent, int multiplier)> unprocessedDamageEvents = new();
    private readonly HashSet<Guid> processedDamageEvents = new();
    public void RegisterHealthEvent(HealthEvent healthEvent, int multiplier = 1)
    {
        if (processedDamageEvents.Contains(healthEvent.GUID)) return;
        var existingMultiplier = unprocessedDamageEvents.ContainsKey(healthEvent.GUID) ? unprocessedDamageEvents[healthEvent.GUID].multiplier : 0;
        unprocessedDamageEvents[healthEvent.GUID] = (healthEvent, Mathf.Max(existingMultiplier, multiplier));
    }

    public void Resize(int newSize)
    {
        var diff = newSize - Size;
        var prev = m_Current;
        m_Current = Mathf.Min(newSize, m_Current + Mathf.Max(0, diff));
        Size = newSize;
        OnModified?.Invoke(m_Current - prev);
        if (m_Current == 0)
            OnDepleted?.Invoke();
    }

    private void ProcessHealthEvents()
    {
        if (unprocessedDamageEvents.Count == 0) return;
        foreach ((var healthEvent, var multiplier) in unprocessedDamageEvents.Values)
        {
            var prevHealth = m_Current;
            processedDamageEvents.Add(healthEvent.GUID);
            healthEvent.OnReleased += () => processedDamageEvents.Remove(healthEvent.GUID);
            this.Current += healthEvent.Amount * multiplier;
            if (Current != prevHealth) OnModifiedWithSource?.Invoke(Current - prevHealth, healthEvent);
        }
        foreach ((var healthEvent, var multiplier) in unprocessedDamageEvents.Values)
            healthEvent.ReportHit(this, healthEvent.Amount * multiplier, this.Current == 0);
        unprocessedDamageEvents.Clear();
    }
}