using UnityEngine;

public abstract class ScriptableSpriteAnimation : ScriptableObject
{
    public abstract Sprite[] GetFrames(int loopID, int seed);
    public abstract int GetFrameRate(int loopID, int seed);
    public int GetLength(int loopID, int seed) => GetFrames(loopID, seed).Length;
    public float GetDuration(int loopID, int seed) => GetLength(loopID, seed) / (float)GetFrameRate(loopID, seed);
}