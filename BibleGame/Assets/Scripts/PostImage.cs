using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PostImage : MonoBehaviour
{
    [SerializeField] AspectRatioFitter aspect;

    [SerializeField] Image image;

    public Sprite sprite => image.sprite;

    Sprite initialSprite;

    private void Awake()
    {
        initialSprite = GetComponent<Image>().sprite;
    }

    private void OnEnable()
    {
        image = GetComponent<Image>();
    }

    public void SetRightSize(Sprite sprite,bool isView = false)
    {
        if(image == null)
        {
            Debug.LogError("post image comp is null");
            return;
        }    

        image.sprite = sprite;
        image.SetNativeSize();

        aspect.aspectMode = isView ? AspectRatioFitter.AspectMode.FitInParent : AspectRatioFitter.AspectMode.EnvelopeParent;

        float aspectRatio = sprite.rect.width / sprite.rect.height;
        aspect.aspectRatio = aspectRatio;
    }


    public void Reset()
    {
        image.sprite = null;
       image.SetNativeSize();

       aspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
    }
}
