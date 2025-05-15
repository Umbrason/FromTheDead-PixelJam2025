using UnityEngine;

[CreateAssetMenu(menuName = "SpriteAnimation/Simple")]
public class SimpleSpriteAnimation : ScriptableSpriteAnimation
{
    [field: SerializeField] public Sprite[] Frames { get; private set; }
    [field: SerializeField] public int FrameRate { get; private set; }
    public override int GetFrameRate(int loopID, int seed) => FrameRate;
    public override Sprite[] GetFrames(int loopID, int seed) => Frames;
}