using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), ExecuteInEditMode]
public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] private ScriptableSpriteAnimation m_Current;
    public ScriptableSpriteAnimation Current
    {
        get => m_Current;
        set
        {
            m_Current = value;
            frameIndex = 0;
            LoopCount = 0;
            SpriteRenderer.sprite = Current == null ? null : Current.GetFrames(LoopCount, seed)[Mathf.FloorToInt(frameIndex)];
        }
    }
    private float frameIndex;

    public int LoopCount { get; private set; }
    public event Action OnCompleteLoop;

    Cached<SpriteRenderer> cached_SpriteRenderer;
    SpriteRenderer SpriteRenderer => cached_SpriteRenderer[this];

#if UNITY_EDITOR
    void OnValidate() => Current = Current;
#endif
    void OnEnable() => Current = Current;
    int seed = new System.Random().Next();
    void Update()
    {
        //if (!Application.isPlaying) return;
        if ((Current?.GetFrames(LoopCount, seed)?.Length ?? 0) == 0)
        {
            SpriteRenderer.enabled = false;
            return;
        }
        SpriteRenderer.enabled = true;
        frameIndex += Time.deltaTime * Current.GetFrameRate(LoopCount, seed);
        while (frameIndex >= Current.GetLength(LoopCount, seed))
        {
            frameIndex -= Current.GetLength(LoopCount, seed);
            LoopCount++;
            OnCompleteLoop?.Invoke();
        }
        SpriteRenderer.sprite = Current.GetFrames(LoopCount, seed)[Mathf.FloorToInt(frameIndex)];
    }
}