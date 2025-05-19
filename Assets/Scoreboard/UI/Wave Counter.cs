using System;
using TMPro;
using UnityEngine;

public class WaveCounter : MonoBehaviour
{
    [SerializeField] WaveSpawner spawner;
    [SerializeField] TMP_Text text;

    void OnEnable()
    {
        spawner.OnWaveCompleted += OnWaveCompleted;
    }

    void OnDisable()
    {
        spawner.OnWaveCompleted -= OnWaveCompleted;
    }

    private void OnWaveCompleted(int obj)
    {
        text.text = obj.ToString();
    }
}
