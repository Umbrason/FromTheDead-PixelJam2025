using System.Linq;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlayerBodyRenderer : MonoBehaviour
{
    Cached<MeshFilter> cached_MeshFilter;
    MeshFilter MeshFilter => cached_MeshFilter[this];
    Cached<MeshRenderer> cached_MeshRenderer;
    MeshRenderer MeshRenderer => cached_MeshRenderer[this];
    Cached<PlayerAmmunition> cached_PlayerAmmunition = new(Cached<PlayerAmmunition>.GetOption.Parent);
    private Texture2D positions;

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
        bounds.Expand(2.75f);
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
        if (positions?.width - 1 != PlayerAmmunition.AppendagePositions.Count)
        {
            block ??= new();
            DestroyImmediate(positions);
            positions = new Texture2D(PlayerAmmunition.AppendagePositions.Count + 1, 1, UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32_SFloat, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
            block.SetTexture("_points", positions);
            block.SetInt("_count", positions.width);
            MeshRenderer.SetPropertyBlock(block);
        }
        positions.SetPixelData(PlayerAmmunition.AppendagePositions.Prepend(default).ToArray(), 0, 0);
        positions.Apply();
    }
}
