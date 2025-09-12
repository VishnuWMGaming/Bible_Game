using System.IO;
using UnityEngine.UIElements;

public static class SceneManagerUtils
{
    public static string GetFileAssetLocation(this FileInfo fileInfo)
    {
        return  $"{fileInfo.Directory}/{fileInfo.Name}";
    }

    public static string[] GetFileArrayNames(this FileInfo[] fileInfo)
    {
        var fileNames = new string[ fileInfo.Length ];

        for (var i = 0; i < fileInfo.Length; i++)
        {
            fileNames[i] = fileInfo[i].Name ;
        }

        return fileNames;
    }

    public static string GetFileName(this  FileInfo fileInfo)
    {
        return fileInfo.Name;
    }

}
