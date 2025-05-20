using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]
public class AudioClipGroup : ScriptableObject
{
    [field: SerializeField] public List<AudioClip> Clips { get; private set; } = new();
    [field: SerializeField] public AudioMixerGroup MixerGroup { get; private set; }
    [field: SerializeField] private Vector2 pitchRange = new(1, 1);

    public bool TryGetRandom(out AudioClip clip)
    {
        if (Clips == null || Clips.Count == 0)
        {
            clip = null;
            return false;
        }

        clip = Clips[Random.Range(0, Clips.Count)];
        return true;
    }

    public float RandomPitch() => Random.Range(pitchRange.x, pitchRange.y);
}