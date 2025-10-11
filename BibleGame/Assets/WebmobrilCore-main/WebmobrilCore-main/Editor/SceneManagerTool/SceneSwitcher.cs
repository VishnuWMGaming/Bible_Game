#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), id: ID_SCENE_VIEWER_OVERLAY, displayName: "SceneSwitcher")]
[Icon("Packages/com.webmobril.core/Sprites/Icons/unity_scene.png")] 
public class SceneSwitcher : Overlay
{ 
    private const string ID_SCENE_VIEWER_OVERLAY = "SceneSwitcher";
    private VisualElement root;
    private static readonly string extension = ".unity";


    private static List<SceneData> currentSceneList = new List<SceneData>();

    public override VisualElement CreatePanelContent()
    {

        root = new VisualElement();
        CreateSceneButtons();
        return root;
    }

    private void CreateSceneButtons()
    {
        UpdateSceneList();
        var currentMousePosition = Event.current.mousePosition;
        currentMousePosition.y += 30f;
        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(currentMousePosition)),new SceneSearch(currentSceneList,SelectScene));
        
    }

    private void SelectScene(SceneData sceneData)
    {
        Debug.LogFormat("SceneSelected {0} {1}", sceneData.sceneName , sceneData.GroupName);
        var scenePath = sceneData.FileInfo.GetFileAssetLocation();
        LoadScene(scenePath);
        // fileInfos[selectedScene].GetFileAssetLocation()
    }

    private void LoadScene(string scenePath)
    {
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            int dialogResult = EditorUtility.DisplayDialogComplex(
                "Scene has been modified",
                "Do you want to save the changes you made in the current scene?",
                "Save", "Don't Save", "Cancel");
            
            bool sceneLoadResult = EditorUtility.DisplayDialog(
                "Scene Load Mode",
                "Select Scene Load Type",
                "AdditiveLoad", "Single Load");
            
            OpenSceneMode loadType = (sceneLoadResult) ? OpenSceneMode.Additive : OpenSceneMode.Single;

            switch (dialogResult)
            {
                case 0: //Save and open the new scene
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                    EditorSceneManager.OpenScene(scenePath , loadType);
                    break;
                case 1: //Open the new scene without saving current.
                    EditorSceneManager.OpenScene(scenePath , loadType);
                    break;
                case 2: //Cancel process (Basically do nothing for now.)
                    break;
                default:
                    Debug.LogWarning("Something went wrong when switching scenes.");
                    break;
            }
        }
        else
        {
            bool dialogResult = EditorUtility.DisplayDialog(
                "Scene Load Mode",
                "Select Scene Load Type",
                "AdditiveLoad", "Single Load");

            OpenSceneMode loadType = (dialogResult) ? OpenSceneMode.Additive : OpenSceneMode.Single;
            EditorSceneManager.OpenScene(scenePath , loadType );
        }
    }

    private static void UpdateSceneList()
    {
        currentSceneList.Clear();
        var productNamePath = Application.dataPath + "/" + Application.productName;
        var filePath = (Directory.Exists(productNamePath)) ? productNamePath : Application.dataPath;
       
       var fileInfos = GetSceneFilesInfo(filePath);
        foreach (var fileInfo in fileInfos)
        {
             SceneData sceneData = new SceneData();
             sceneData.FileInfo = fileInfo;
             sceneData.sceneName = fileInfo.GetFileName();
             sceneData.GroupName = fileInfo.Directory.Name;
             currentSceneList.Add(sceneData);
        }
    }
    
    private static FileInfo[] GetSceneFilesInfo(string dirPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            FileInfo[] info = dir.GetFiles($"*{extension}", SearchOption.AllDirectories);
            return info;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

public class SceneData
{
    public FileInfo FileInfo;
    public string sceneName;
    public string GroupName;
}

#endif