using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> Options;
    [SerializeField] private float Delay;
    void Start()
    {
        Invoke(nameof(Spawn), Delay);
    }

    void Spawn()
    {
        Instantiate(Options[Random.Range(0, Options.Count)], transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
