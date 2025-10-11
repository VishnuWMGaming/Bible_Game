using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public  static class AnagramUtils
{
    public static bool IsPositionInside(Vector2 position, RectTransform region)
    {
        Rect maskRect = region.rect;

        return position.x >= maskRect.xMin && position.x <= maskRect.xMax &&
               position.y >= maskRect.yMin && position.y <= maskRect.yMax;
    }

    public static Vector2 GetMousePositionInCanvasSpace(Canvas canvas)
    {
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

        Vector2 mousePosition = Input.mousePosition;
        Vector2 localPoint;

        // Convert the mouse position to local position in the RectTransform
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            mousePosition,
            canvas.worldCamera, // Camera associated with the canvas (null if Overlay mode)
            out localPoint
        );

        return localPoint;
    }

    public static bool AreImagesOverlapping(RectTransform rect1, RectTransform rect2)
    {
        // Get the world corners of the second RectTransform
        Vector3[] corners = new Vector3[4];
        rect2.GetWorldCorners(corners);

        // Create a Rect for the second image based on its world corners
        Rect rectB = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);

        // Get the center point of the first RectTransform in world space
        Vector3 center1 = rect1.position;

        // Check if the center of the first image is within the bounds of the second image
        return rectB.Contains(center1);
    }


}
