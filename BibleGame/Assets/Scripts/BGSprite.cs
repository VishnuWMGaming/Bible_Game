using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BGSprite : MonoBehaviour
{

    public Camera mainCamera;
    public float minScaleFactor = 1f; // Minimum allowed scale
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        AutoScale();
    }

    void AutoScale()
    {
        float orthographicHeight = mainCamera.orthographicSize * 2f;
        float spriteHeight = spriteRenderer.sprite.rect.height / spriteRenderer.sprite.pixelsPerUnit;

        float scaleFactor = Mathf.Max(minScaleFactor, orthographicHeight / spriteHeight);

        transform.localScale = Vector3.one * scaleFactor;
    }

    void OnRectTransformDimensionsChange()
    {
        AutoScale();
    }
}
