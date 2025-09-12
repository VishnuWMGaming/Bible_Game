using UnityEditor;
using UnityEngine;

public static class ClearPlayerPrefs
{
#if UNITY_EDITOR

    [MenuItem("Webmobril/ClearPrefs/Clear")]
    [MenuItem("Assets/Webmobril/ClearPrefs", false, 1)]
    //[CreateAssetMenu( fileName = "3DAssetFolder",menuName = "Webmobril/Game/2DGame")]
    public static void ClearPlayerPrefsData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Cleared");
    }

#endif
}