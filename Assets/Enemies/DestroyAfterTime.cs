using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float Timer;
    void Awake() => Invoke(nameof(DestroyGameObject), Timer);

    void DestroyGameObject() => Destroy(gameObject);
}
