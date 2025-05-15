using System.Collections;
using UnityEngine;

public abstract class BaseEnemyBrain : MonoBehaviour
{
    Cached<SpriteAnimator> cached_Animator = new(Cached<SpriteAnimator>.GetOption.Children);
    protected SpriteAnimator Animator => cached_Animator[this];
    Coroutine CurrentThinking;
    protected abstract IEnumerator FirstThought();

    protected Vector2 SelfPosition() => transform.position._xz();
}