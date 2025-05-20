using System;
using System.Collections;
using UnityEngine;

public class SFXOnHurt : MonoBehaviour
{

    [SerializeField] AudioClipPlayer AudioClipPlayer;

    Cached<HealthPool> cached_HealthPool;
    HealthPool HealthPool => cached_HealthPool[this];

    void OnEnable()
    {
        HealthPool.OnModifiedWithSource += Play;
    }

    private void Play(int arg1, HealthEvent @event)
    {
        if (@event == null) return;
        if (arg1 < 0) Instantiate(AudioClipPlayer, transform.position, Quaternion.identity);
    }
}

