using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;





public class AdditiveSceneRoot : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset[] sceneAssets;
#endif

    private List<Scene> loadedScenes = new List<Scene>();

    [SerializeField, HideInInspector] private List<string> sceneNames = new List<string>();

    private void OnValidate()
    {
        gameObject.name = "AdditiveRoot";
#if UNITY_EDITOR
        sceneNames.Clear();
        foreach (var scene in sceneAssets)
        {
            sceneNames.Add(scene.name);
        }
#endif
   


    }

    private void Awake()
    {


 #if !UNITY_EDITOR
                foreach (var sceneName in sceneNames)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
#endif
    }

    public void LoadAdditiveScene()
    {
       
#if UNITY_EDITOR
        loadedScenes.Clear();
        foreach (var sceneAsset in sceneAssets)
        {
           

            loadedScenes.Add(EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(sceneAsset),
                OpenSceneMode.Additive));
        }

 
#else
        foreach (var loadedScene in loadedScenes)
        {
            SceneManager.LoadScene(loadedScene.name, LoadSceneMode.Additive);
        }
        
#endif
 
    }

    public void UnloadAllScenes()
    {
        foreach (var scene in loadedScenes)
        {
#if UNITY_EDITOR
            EditorSceneManager.CloseScene(scene, true);
#endif
        }
    }
}