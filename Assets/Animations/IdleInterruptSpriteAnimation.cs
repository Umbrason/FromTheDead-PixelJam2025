using UnityEngine;

[CreateAssetMenu(menuName = "SpriteAnimation/IdleInterrupt")]
public class IdleInterruptSpriteAnimation : ScriptableSpriteAnimation
{
    [SerializeField] private ScriptableSpriteAnimation BaseAnimation;
    [SerializeField] private ScriptableSpriteAnimation[] InterruptAnimations;
    [SerializeField] private int minGap = 2;
    [SerializeField] private int maxGap = 4;
    private ScriptableSpriteAnimation LoopIDToAnimationIndex(int loopID, int seed)
    {
        if (InterruptAnimations.Length == 0) return BaseAnimation;
        var random = new System.Random(seed);
        var i = 0;
        while (i < loopID)
            i += random.Next(minGap, maxGap);
        bool isInterruptFrame = i == loopID;
        if (!isInterruptFrame) return BaseAnimation;
        int interruptIndex = new System.Random(loopID).Next(0, InterruptAnimations.Length);
        return InterruptAnimations[interruptIndex];
    }
    public override int GetFrameRate(int loopID, int seed) => LoopIDToAnimationIndex(loopID, seed)?.GetFrameRate(loopID, seed + 1) ?? 0;
    public override Sprite[] GetFrames(int loopID, int seed) => LoopIDToAnimationIndex(loopID, seed)?.GetFrames(loopID, seed + 1);
}