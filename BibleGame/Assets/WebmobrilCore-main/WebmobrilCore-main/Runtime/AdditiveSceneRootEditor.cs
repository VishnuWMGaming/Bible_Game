#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdditiveSceneRoot))]
public class AdditiveSceneRootEditor : Editor
{
    private AdditiveSceneRoot root;
    private void OnEnable()
    {
        root = (AdditiveSceneRoot) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Load Additive Scenes"))
        { 
            
            root.LoadAdditiveScene();
        }

        if (GUILayout.Button("Unload All  Scene"))
        {
            root.UnloadAllScenes();
        }
        
        
        
    }
}
#endif