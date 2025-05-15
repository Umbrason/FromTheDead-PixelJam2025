using UnityEngine;

[CreateAssetMenu(menuName = "SpriteAnimation/Random Variant")]
public class RandomVariantSpriteAnimation : ScriptableSpriteAnimation
{
    [SerializeField] private ScriptableSpriteAnimation[] Variants;
    ScriptableSpriteAnimation GetAnimation(int seed)
    {
        var random = new System.Random(seed);
        return Variants[random.Next(0, Variants.Length)];
    }

    public override int GetFrameRate(int loopID, int seed) => GetAnimation(seed)?.GetFrameRate(loopID, seed + 1) ?? 0;
    public override Sprite[] GetFrames(int loopID, int seed) => GetAnimation(seed)?.GetFrames(loopID, seed + 1);
}