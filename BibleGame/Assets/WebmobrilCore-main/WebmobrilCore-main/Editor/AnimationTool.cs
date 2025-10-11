using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public static class AnimationTool 
{


    

    [MenuItem("Assets/Webmobril/Animation/RenameAnimation", false, 1)]
    public static void RenameAnimation()
    {

        foreach (GameObject o in Selection.objects) {
            ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(o));
            var clips = modelImporter.defaultClipAnimations;
            foreach (var c in clips) {
                c.name = o.name;
            }
            modelImporter.clipAnimations = clips;
            modelImporter.SaveAndReimport();
        }
    }

    
    

}

    
#endif

