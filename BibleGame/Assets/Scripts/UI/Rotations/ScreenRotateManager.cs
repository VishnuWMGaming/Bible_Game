using UnityEngine;
using UnityEngine.UI;

public class ScreenRotateManager : MonoBehaviour
{
    [SerializeField] private Button rotateBtn;

    private bool isLandscape = false;

    void Start()
    {
        // Make sure your button is assigned in the Inspector
        if (rotateBtn != null)
            rotateBtn.onClick.AddListener(ToggleRotation);
        else
            Debug.LogError("Rotate Button not assigned in Inspector!");
    }

    private void ToggleRotation()
    {
        AudioManager.Instance.PlayButton();

        if (!isLandscape)
        {
            // Enable landscape orientation
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;

            Screen.orientation = ScreenOrientation.LandscapeLeft;
            isLandscape = true;

            Debug.Log("Switched to Landscape");
        }
        else
        {
            // Back to portrait orientation
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;

            Screen.orientation = ScreenOrientation.Portrait;
            isLandscape = false;

            Debug.Log("Switched to Portrait");
        }

        // Optional: force UI refresh
        Canvas.ForceUpdateCanvases();
    }
}
