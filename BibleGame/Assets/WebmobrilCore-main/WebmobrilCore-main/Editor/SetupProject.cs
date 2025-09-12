using System.IO;
using UnityEditor;
using UnityEngine;

public static class SetupProject
{
#if UNITY_EDITOR

    [MenuItem("Webmobril/CreateProjectFolders/Game/2DGame")]
    [MenuItem("Assets/Webmobril/Game/2DGame", false, 1)]
    //[CreateAssetMenu( fileName = "3DAssetFolder",menuName = "Webmobril/Game/2DGame")]
    public static void Setup2DDirectories()
    {
        CreateDirectory(Application.productName, "Art", "Script", "Scenes", "Models", "Prefab");
        AssetDatabase.Refresh();
    }

    [MenuItem("Webmobril/CreateProjectFolders/Game/3DGame")]
    [MenuItem("Assets/Webmobril/Game/3DGame", false, 1)]
    public static void Setup3DDirectories()
    {
        CreateDirectory(Application.productName, "Art", "Script", "Scenes", "Models", "Prefab", "Material",
            "Animation");
        AssetDatabase.Refresh();
    }

    [MenuItem("Webmobril/CreateProjectFolders/Asset/3DAssetFolder")]
    [MenuItem("Assets/Webmobril/Asset/3DAssetFolder", false, 1)]
    public static void Setup3dAssetFolder()
    {
        var SelectedFolder = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        Debug.LogFormat("SelectedFolderCount{0}", SelectedFolder.Length);
        if (SelectedFolder.Length != 1)
        {
            EditorUtility.DisplayDialog("Folder Setup Error",
                "Please select a root folder to create the required folders", "ok");
            return;
        }

        string path = AssetDatabase.GetAssetPath(SelectedFolder[0]);
        path = Path.GetFullPath(path);
        Debug.LogFormat("Path {0}", path);
        CreateDirectory(path, "Materials", "Meshes", "Prefabs", "Textures");
        AssetDatabase.Refresh();
    }

    private static void CreateDirectory(string rootFolder, params string[] directories)
    {
        var pathname = Path.Combine(Application.dataPath, rootFolder);
        foreach (var newDir in directories)
        {
            Directory.CreateDirectory(Path.Combine(pathname, newDir));
        }
    }

#endif
}