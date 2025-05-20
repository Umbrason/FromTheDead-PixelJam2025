using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClipGroup[] groups;
    Cached<AudioSource> cached;
    AudioSource source => cached[this];
    [SerializeField] bool destroyAfter = true;

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        var t = 0f;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            t = Mathf.Clamp01(t);
            source.volume = 1 - t;
        }
        yield return null;
    }

    private IEnumerator Start()
    {
        var lastSong = -1;
        do
        {
            var index = -1;
            do index = Random.Range(0, groups.Length);
            while (index == lastSong);
            lastSong = index;
            var group = groups[index];
            if (group.TryGetRandom(out var clip))
            {
                source.clip = clip;
                source.pitch = group.RandomPitch();
                source.outputAudioMixerGroup = group.MixerGroup;
                source.Play();
                yield return new WaitUntil(() => source.isPlaying);
                yield return new WaitUntil(() => !source.isPlaying);
            }
        }
        while (true);
    }
}
