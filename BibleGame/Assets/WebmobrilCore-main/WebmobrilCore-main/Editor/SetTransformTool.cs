
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR




[InitializeOnLoad]
public  class SetTransformTool
{
    private const string saveTransformKey = "CONTEXT/Component/SetTransform";
    private static readonly Dictionary<Transform, TransformData> setTransforms = new Dictionary<Transform, TransformData>();


    static SetTransformTool()
    {
        EditorApplication.playModeStateChanged += PlayModeStateChanged; 
    }

    private static void PlayModeStateChanged(PlayModeStateChange state)
    {
        //Debug.LogFormat("State {0}",state);

        switch (state)
        {
            case PlayModeStateChange.EnteredPlayMode:
                setTransforms.Clear();
                break;
            case PlayModeStateChange.EnteredEditMode when setTransforms.Count > 0:
            {
                foreach (var transformData in 
                         setTransforms.Where(transformData => transformData.Key != null))
                {
                    transformData.Key.SetTransformData(transformData.Value);
                    EditorUtility.SetDirty(transformData.Key);
                }
                setTransforms.Clear();
                break;
            }
        }
    }

    [MenuItem(saveTransformKey, priority = 501)]
    public static void SaveTransform(MenuCommand menuCommand)
    {
        Transform transform = menuCommand.GetTransform();
        if (transform == null)
        {
            return;
        }
        var   transformData = new TransformData(transform.position,transform.rotation, transform.localScale);
        setTransforms.Add(transform,transformData);
    }
    
    
    [MenuItem(saveTransformKey, validate = true)]
    public static bool SaveTransformValidate(MenuCommand menuCommand)
    {
        Transform component = menuCommand.GetTransform();
        return component != null && Application.isPlaying;
    }


    

}

public static class TransformUtils
{
    public static Transform GetTransform(this MenuCommand menuCommand)
    {
        var component = ((Component) menuCommand.context).gameObject.GetComponent<Transform>();
        return  component;
    }

    public static void SetTransformData(this Transform transform, TransformData transformData)
    {
        transform.position = transformData.Positions;
        transform.rotation = transformData.Rotation;
        transform.localScale = transformData.Scale;
    }
    
}

#endif