using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioClipPlayer : MonoBehaviour
{
    [SerializeField] AudioClipGroup group;
    Cached<AudioSource> cached;
    AudioSource source => cached[this];

    [SerializeField] bool loop;
    [SerializeField] bool destroyAfter = true;

    [SerializeField] bool playOnStart = true;

    public void Play()
    {
        StartCoroutine(PlayRoutine());
    }


    private void Start()
    {
        if (playOnStart) Play();
    }

    private IEnumerator PlayRoutine()
    {
        do
            if (group.TryGetRandom(out var clip))
            {
                source.clip = clip;
                source.pitch = group.RandomPitch();
                source.outputAudioMixerGroup = group.MixerGroup;
                source.Play();
                yield return new WaitUntil(() => source.isPlaying);
                yield return new WaitUntil(() => !source.isPlaying);
            }
        while (loop);
        if (destroyAfter) Destroy(gameObject);
    }
}
