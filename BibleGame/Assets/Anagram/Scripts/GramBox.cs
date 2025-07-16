using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface IBox
{
    public void UpdatedPos(int index, Vector2 position);
    public void ResultAction();
}

public class GramBox : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] RectTransform boxTransform;
    public RectTransform BoxT => boxTransform;


    [SerializeField] public RectTransform maskArea;

    [SerializeField] Canvas canvas;

    [SerializeField] Vector2 startPosition;
    public Vector2 BoxV => startPosition;

    [SerializeField] Vector3 worldPos;


    Vector2 initialPosition;

    [Space]
    [SerializeField] bool isDragging = false;
    public bool IsDrag => isDragging;

    [SerializeField] int index;

    public int Index => index;

    [SerializeField] string value;

    public string Value
    {
        get { return value; }
        set { this.value = Value; }
    }

    [SerializeField] TMP_Text letterText;

    [Header("CorrectWord")]
    [SerializeField] GameObject correctW;
    public bool isCorrect => letterText.color == Color.green;

    [SerializeField] Image image;

    [Space]
    [SerializeField] bool isEnable;
    public bool IsEnable => isEnable;

    [SerializeField] LayoutElement layout;

    public void Enable(bool enable) 
    { 
        isEnable = enable; 

        if(!isEnable) index = -1;

       Color32 colorT = letterText.color;
       letterText.color = enable? new Color32(colorT.r, colorT.g, colorT.b, 225) : new Color32(colorT.r, colorT.g, colorT.b, 0);
       letterText.raycastTarget = enable;

        Color32 colorI = image.color;
        image.color = enable? new Color32(colorI.r, colorI.g, colorI.b, 225) : new Color32(colorI.r, colorI.g, colorI.b, 0);
        image.raycastTarget = enable;

        layout.ignoreLayout = !enable;
    }

    public IBox callback;


    private void OnEnable()
    {
        boxTransform = GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>();
        boxTransform.anchorMax = new Vector2(0.5f, 0.5f);
        boxTransform.anchorMin = new Vector2(0.5f, 0.5f);

        startPosition = boxTransform.anchoredPosition;
        initialPosition = boxTransform.anchoredPosition;

        isEnable = false;

        index = -1;

        isDragging = false;
        SetLetter();
    }

    public void SetStartPos(Vector2 startPosition , Vector3 worldPos)
    {
        if (!isEnable) return;

        this.startPosition = startPosition;
        this.worldPos = worldPos;

        //boxTransform.anchoredPosition = startPosition;

        //boxTransform.anchorMin = new Vector2(0.5f, 0.5f);
        //boxTransform.anchorMax = new Vector2(0.5f, 0.5f);

       // boxTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    public void Arrange()
    {
        boxTransform.anchorMin = new Vector2(0.5f, 0.5f);
        boxTransform.anchorMax = new Vector2(0.5f, 0.5f);

        boxTransform.pivot = new Vector2(0.5f, 0.5f);

        boxTransform.position = worldPos;

        boxTransform.anchoredPosition = new Vector2(
                                                     Mathf.Round(boxTransform.anchoredPosition.x),
                                                     Mathf.Round(boxTransform.anchoredPosition.y)
                                                   );

        this.startPosition = boxTransform.anchoredPosition;

    }



    public Vector2 GetStartPos()
    {
        return this.startPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isEnable) return;

       // Debug.Log($"<color=grey>Begin drag:{value}</color>");

        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isEnable) return;

        if (!isDragging) return;

        if (canvas == null)
        {
            Debug.LogError("Canvas is null");
            return;
        }

        Vector2 newPosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            null,
            out newPosition
        );

        Vector2 localPosition = AnagramUtils.GetMousePositionInCanvasSpace(canvas);

        bool isInsideMask = AnagramUtils.IsPositionInside(localPosition, maskArea);

        if (!isInsideMask)
        {
            Debug.LogError("Outside the area !!!!");

            boxTransform.DOAnchorPos(startPosition, 0.5f);
            return;
        }

        boxTransform.anchoredPosition = localPosition;

        callback.UpdatedPos(index, boxTransform.anchoredPosition);
    }

    public void SetPos(Vector2 pos, int newIndex, bool isRun = false)
    {
        if (!isEnable) return;

        if (isRun)
            boxTransform.DOAnchorPos(pos, 0.2f);

        index = newIndex;

        startPosition = pos;
    }

    public void SetLetter()
    {
        if (!isEnable) return;

        letterText.text = value;
    }

    public void SetLetter(string value)
    {
        if (!isEnable) return;

        letterText.text = value;
        this.value = value;
    }

    public void SetIndex(int index)
    {
        if (!isEnable) return;

        this.index = index;
    }

    public void SetCorrectWord(bool enable) { letterText.color = enable ? Color.green : Color.red; }

    public void SetImage(Sprite sprite)
    {
        if (!isEnable) return;

        image.sprite = sprite;
        image.SetNativeSize();

        Vector2 size = boxTransform.sizeDelta;
        boxTransform.sizeDelta = new Vector2(size.x / 4, size.y / 4);
    }
   
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isEnable) return;

        if (!isDragging)
            return;

        isDragging = false;
        boxTransform.DOAnchorPos(startPosition, 0.2f);

        callback.ResultAction();
    }

    public void Restart()
    {
        callback = null;
        boxTransform.anchoredPosition = initialPosition;
        letterText.color = Color.white;

        Enable(false);
    }
}
