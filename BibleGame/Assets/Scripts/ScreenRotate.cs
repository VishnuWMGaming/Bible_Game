using System.Runtime.InteropServices;
using UnityEngine;

public class ScreenRotate : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RotateToLandscape();

    [DllImport("__Internal")]
    private static extern void RotateToPortrait();
#endif

    [SerializeField] private RectTransform videoPanel;

    public void RotateLandscape()
    {
        Vector3 rotation = videoPanel.localEulerAngles;
        rotation.z = -90f;
        videoPanel.localEulerAngles = rotation;
    }

    public void RotatePortrait()
    {
        Vector3 rotation = videoPanel.localEulerAngles;
        rotation.z = 0f;
        videoPanel.localEulerAngles = rotation;
    }

    public void SetLandscape()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RotateToLandscape();
        RotateLandscape();

      //Screen.orientation = ScreenOrientation.LandscapeLeft;
#endif
    }

    public void SetPortrait()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RotateToPortrait();
        RotatePortrait();

       // Screen.orientation = ScreenOrientation.Portrait;
#endif
    }
}