using UnityEngine;

public static class TransformUtils
{
    public static void SetLayerRecursive(this Transform transform, int layer)
    {
        transform.gameObject.layer = layer;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).SetLayerRecursive(layer);
    }
}