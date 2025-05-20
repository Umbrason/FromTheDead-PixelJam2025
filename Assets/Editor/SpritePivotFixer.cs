using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class SpritePivotFixer : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        var factory = new SpriteDataProviderFactories();
        factory.Init();

        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(assetImporter);
        dataProvider.InitSpriteEditorDataProvider();

        SetPivot(dataProvider, new Vector2(0.5f, 0.5f));
        dataProvider.Apply();
    }

    static void SetPivot(ISpriteEditorDataProvider dataProvider, Vector2 pivot)
    {
        var spriteRects = dataProvider.GetSpriteRects();
        foreach (var rect in spriteRects)
        {
            if (rect.alignment == SpriteAlignment.Center)
            {
                var roundedPivot = (Vector2Int.FloorToInt(rect.rect.size * pivot) + Vector2.one / 2f) / rect.rect.size;
                rect.pivot = roundedPivot;
                rect.alignment = SpriteAlignment.Custom;
            }
        }
        dataProvider.SetSpriteRects(spriteRects);
    }
}
