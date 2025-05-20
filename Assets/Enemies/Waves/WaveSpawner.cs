using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] AnimationCurve EnemyCountCurve;
    [SerializeField] GameObject SpawnCircle;

    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;

    [SerializeField] HealthPool PlayerHealthpool;
    [SerializeField] private int StartHealth;

    void Start() => StartCoroutine(SpawnRoutine());
    IEnumerator SpawnRoutine()
    {
        int waveCounter = 0;
        yield return new WaitUntil(() => PlayerHealthpool.Current >= StartHealth);
        while (true)
        {
            OnWaveStarted?.Invoke(waveCounter);
            yield return Spawn(Mathf.RoundToInt(EnemyCountCurve.Evaluate(Mathf.Clamp01(waveCounter / 100f))));
            yield return new WaitUntil(() => EnemyPositionMarker.EnemyCount == 0);
            OnWaveCompleted?.Invoke(waveCounter);
            waveCounter++;
        }
    }

    public IEnumerator Spawn(int amount)
    {
        List<GameObject> SpawnCircles = new(amount);
        for (int i = 0; i < amount; i++)
        {
            var pos = EnemyBrainUtils.RandomPatrolPosition(default, 0, 50);
            var instance = Instantiate(SpawnCircle, pos._x0y(), Quaternion.identity);
            SpawnCircles.Add(instance);
        }
        yield return new WaitUntil(() => SpawnCircles.All(circle => circle == null));
    }
}
