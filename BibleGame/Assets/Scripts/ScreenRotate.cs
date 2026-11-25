using System.Runtime.InteropServices;
using UnityEngine;

public class ScreenRotate : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RotateToLandscape();

    [DllImport("__Internal")]
    private static extern void RotateToPortrait();

    [DllImport("__Internal")]
    private static extern void RotateScreen90();

    [DllImport("__Internal")]
    private static extern void ResetScreenRotation();
#endif

    [SerializeField] private RectTransform videoPanel;

    [Space]
    [SerializeField] bool isRotate = false;

    public void RotateLandscape()
    {
        if(videoPanel == null)
            return;

     

        Vector3 rotation = videoPanel.localEulerAngles;
        rotation.z = -90f;
        videoPanel.localEulerAngles = rotation;
    }

    public void RotatePortrait()
    {
        if(videoPanel == null)
            return;

        Vector3 rotation = videoPanel.localEulerAngles;
        rotation.z = 0f;
        videoPanel.localEulerAngles = rotation;
    }

    public void SetLandscape()
    {
#if UNITY_WEBGL && !UNITY_EDITOR

        //if(isRotate)
        //{
        //  //RotateScreen90();
        //  RotateToLandscape();
        //  return;
        //}

        RotateToLandscape();
        RotateLandscape();
      //Screen.orientation = ScreenOrientation.LandscapeLeft;
#endif
    }

    public void SetPortrait()
    {
#if UNITY_WEBGL  && !UNITY_EDITOR
        
        //if(isRotate)
        //{
        //  //ResetScreenRotation();
        //  RotateToPortrait();
        //  return;
        //}


        RotateToPortrait();
        RotatePortrait();

       // Screen.orientation = ScreenOrientation.Portrait;
#endif
    }
}