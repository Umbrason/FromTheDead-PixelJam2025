using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class UpscaledCamera : MonoBehaviour
{
    Cached<Camera> cached_Camera;
    Camera Camera => cached_Camera[this];
    Cached<PixelPerfectCamera> cached_ppCamera;
    PixelPerfectCamera ppCamera => cached_ppCamera[this];
    [HideInInspector] private RenderTexture targetTexture;
    [SerializeField] private ResolutionSettings resolutionSettings;

    [System.Serializable]
    public struct ResolutionSettings
    {
        public Vector2Int TargetResolution;
        [Range(0, 1)] public float MatchWidth;
        public Vector2Int CalculateResolution(Vector2 ScreenBufferSize)
        {
            var widthDriven = new Vector2(TargetResolution.x, TargetResolution.x * ScreenBufferSize.y / (float)ScreenBufferSize.x);
            var heightDriven = new Vector2(TargetResolution.y * ScreenBufferSize.x / (float)ScreenBufferSize.y, TargetResolution.y);
            return Vector2Int.RoundToInt(Vector2.Lerp(widthDriven, heightDriven, 1 - MatchWidth)) / 2 * 2;
        }
        public ResolutionSettings(Vector2Int TargetResolution, float MatchWidth = 0.5f)
        {
            this.TargetResolution = TargetResolution;
            this.MatchWidth = MatchWidth;
        }
    }

    private void OnDisable()
    {
        Camera.targetTexture = null;
        UpscaleRenderPass.TargetTexture = null;
        if (targetTexture) DestroyImmediate(targetTexture);
    }

    private void Update()
    {
        var resolution = resolutionSettings.CalculateResolution(new(Screen.width, Screen.height));
        if (targetTexture == null || targetTexture.width != resolution.x || targetTexture.height != resolution.y)
        {
            if (targetTexture != null)
            {
                Camera.targetTexture = null;
                DestroyImmediate(targetTexture);
            }
            targetTexture = new(resolution.x, resolution.y, (int)DepthBits.Depth16, UnityEngine.Experimental.Rendering.GraphicsFormat.B10G11R11_UFloatPack32, 0);
            Camera.targetTexture = targetTexture;
            ppCamera.refResolutionX = Camera.targetTexture.width;
            ppCamera.refResolutionY = Camera.targetTexture.height;
            UpscaleRenderPass.TargetTexture = targetTexture;
        }
    }
}
