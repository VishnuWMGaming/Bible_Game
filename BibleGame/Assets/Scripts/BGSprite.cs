using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BGSprite : MonoBehaviour
{

    private Camera _camera;
    private SpriteRenderer _spriteRenderer;

    public float depth = -10f;
    public float padding = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _spriteRenderer = GetComponent<SpriteRenderer>();

        ScaleBackgroundImage();
    }

    private void ScaleBackgroundImage()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float orthographicSize = _camera.orthographicSize;
        float orthographicWidth = orthographicSize * _camera.aspect;

        float spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        float spriteHeight = GetComponent<SpriteRenderer>().bounds.size.y;

        float scaleX = (orthographicWidth * (1 + padding)) / spriteWidth;
        float scaleY = (orthographicSize * (1 + padding)) / spriteHeight;

        transform.localScale = Vector3.one * Mathf.Max(scaleX, scaleY);

        // Adjust position to center the sprite
        transform.position = new Vector3(0, 0, depth);

    }
}
