using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] GameObject Target;

    /* private bool Quitting;
    void OnApplicationQuit()
    {
        Quitting = true;
    } */
    void OnDestroy()
    {
        if (!this.gameObject.scene.isLoaded) return;
     /*    if (!Quitting) */ Instantiate(Target, transform.position, transform.rotation);
    }
}
