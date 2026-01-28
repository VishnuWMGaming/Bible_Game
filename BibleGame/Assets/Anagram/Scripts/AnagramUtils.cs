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

    public static Vector2 GetMousePositionInCanvasSpace(Canvas canvas,bool isLandscape)
    {
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

        canvasRectTransform.localEulerAngles = isLandscape ? new Vector3(0, 0, 90) : Vector3.zero;

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

    public static bool AreImagesOverlapping(RectTransform rect1, RectTransform rect2,bool isLandscape = false)
    {

        // Get the world corners of rect2
        Vector3[] rect2Corners = new Vector3[4];
        rect2.GetWorldCorners(rect2Corners);

        // Convert rect1 center to world position
        Vector3 center1 = rect1.position;

        // Use a function to check if a point is inside a polygon (rect2Corners)
        return IsPointInPolygon(center1, rect2Corners);
    }

    private static bool IsPointInPolygon(Vector3 point, Vector3[] poly)
    {
        int j = poly.Length - 1;
        bool inside = false;
        for (int i = 0; i < poly.Length; j = i++)
        {
            if (((poly[i].y > point.y) != (poly[j].y > point.y)) &&
                (point.x < (poly[j].x - poly[i].x) * (point.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }
}
