using UnityEditor;

[CustomEditor(typeof(UpscaleRenderFeature))]
public class UpscaleRendererAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("material"));
    }
}