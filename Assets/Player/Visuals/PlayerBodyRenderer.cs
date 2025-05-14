using System.Linq;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlayerBodyRenderer : MonoBehaviour
{
    Cached<MeshFilter> cached_MeshFilter;
    MeshFilter MeshFilter => cached_MeshFilter[this];

    Cached<Rigidbody> cached_Rigidbody = new(Cached<Rigidbody>.GetOption.Parent);
    Rigidbody Rigidbody => cached_Rigidbody[this];

    Cached<MeshRenderer> cached_MeshRenderer;
    MeshRenderer MeshRenderer => cached_MeshRenderer[this];
    Cached<PlayerAmmunition> cached_PlayerAmmunition = new(Cached<PlayerAmmunition>.GetOption.Parent);
    private Texture2D positionsTexture;
    private Texture2D velocityTexture;

    PlayerAmmunition PlayerAmmunition => cached_PlayerAmmunition[this];
    private MaterialPropertyBlock block;

    void OnEnable()
    {
        MeshFilter.sharedMesh ??= new Mesh() { hideFlags = HideFlags.DontSave };
    }

    void LateUpdate()
    {
        if (!enabled) return;
        var bounds = new Bounds(default, default);
        foreach (var point in PlayerAmmunition.AppendagePositions)
            bounds.Encapsulate(point._x0y());
        bounds.Expand(4f);
        var min = bounds.min._x0z();
        var max = bounds.max._x0z();
        MeshFilter.sharedMesh.vertices = new Vector3[] {
            min,
            new(min.x,0, max.z),
            max,
            new(max.x,0, min.z),
        };
        MeshFilter.sharedMesh.uv = new Vector2[] {
            min._xz(),
            new(min.x, max.z),
            max._xz(),
            new(max.x, min.z),
        };

        //1---2
        //| / |
        //0---3
        MeshFilter.sharedMesh.triangles = new int[]{
            0,1,2,
            0,2,3
        };
        MeshFilter.sharedMesh.RecalculateBounds();
        if ((positionsTexture != null ? positionsTexture.width - 1 : null) != PlayerAmmunition.AppendagePositions.Count ||
            (velocityTexture != null ? velocityTexture.width - 1 : null) != PlayerAmmunition.AppendageVelocities.Count)
        {
            block ??= new();
            DestroyImmediate(positionsTexture);
            DestroyImmediate(velocityTexture);
            positionsTexture = new Texture2D(PlayerAmmunition.AppendagePositions.Count + 1, 1, UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32_SFloat, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
            velocityTexture = new Texture2D(PlayerAmmunition.AppendageVelocities.Count + 1, 1, UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32_SFloat, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
            block.SetTexture("_points", positionsTexture);
            block.SetTexture("_velocities", velocityTexture);
            block.SetInt("_count", positionsTexture.width);
            MeshRenderer.SetPropertyBlock(block);
        }
        var positionsData = PlayerAmmunition.AppendagePositions.Prepend(default).ToArray();
        for (int i = 0; i < positionsData.Length; i++)
            positionsData[i] = (Vector2)Vector2Int.RoundToInt(positionsData[i] * 16 + Vector2.one * .5f) / 16f;
        var velocityData = PlayerAmmunition.AppendageVelocities.Prepend(default).ToArray();
        for (int i = 0; i < velocityData.Length; i++)
            velocityData[i] += Rigidbody.linearVelocity._xz();
        positionsTexture.SetPixelData(positionsData, 0, 0);
        velocityTexture.SetPixelData(velocityData, 0, 0);
        positionsTexture.Apply();
        velocityTexture.Apply();
    }
}
