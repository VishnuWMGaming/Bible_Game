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

public class GramBox : MonoBehaviour,IBeginDragHandler, IEndDragHandler, IDragHandler
{
    RectTransform boxTransform;
    public RectTransform BoxT => boxTransform;
   

    [SerializeField] public RectTransform maskArea;

    [SerializeField]  Canvas canvas;

    [SerializeField] Vector2 startPosition;
    public Vector2 BoxV => startPosition;

    [Space]
    [SerializeField] bool isDragging = false;
    [SerializeField] int index;
    public int Index{
        get { return index; }
        set { index = Index; }
    }

    [SerializeField] string value;

    public string Value
    {
        get { return value; }
        set { this.value = Value; }
    }

    [SerializeField] TMP_Text letterText;

    [Header("CorrectWord")]
    [SerializeField] GameObject correctW;
    public bool isCorrect => image.color == Color.green;

    Image image;

    public IBox callback;

    private void OnEnable()
    {
        boxTransform = GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>();
        boxTransform.anchorMax = new Vector2(0.5f, 0.5f);
        boxTransform.anchorMin = new Vector2(0.5f, 0.5f);

        startPosition =/* this.transform.position;*/boxTransform.anchoredPosition;

        image = GetComponent<Image>();

        SetLetter();
    }

    public void SetStartPos(Vector2 startPosition)
    {
        this.startPosition = startPosition;
    }

    public Vector2 GetStartPos()
    {
        return this.startPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
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
            out  newPosition
        );

        Vector2 localPosition = AnagramUtils.GetMousePositionInCanvasSpace(canvas);
        bool isInsideMask = AnagramUtils.IsPositionInside(localPosition,maskArea);

        if (!isInsideMask)
        {
            Debug.LogError("Outside the area !!!!");

            boxTransform.DOAnchorPos(startPosition,0.5f);
            return;
        }

        boxTransform.anchoredPosition = newPosition;

        callback.UpdatedPos(index, localPosition);
    }

    public void SetPos(Vector2 pos ,int newIndex, bool isRun = false)
    {
        if(isRun) 
        boxTransform.DOAnchorPos(pos, 0.2f);

        index = newIndex;

        startPosition = pos;
    }

    public void SetLetter()
    {
        letterText.text = value;
    }
    
    public void SetLetter(string value)
    {
        letterText.text = value;
        this.value = value;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void SetCorrectWord(bool enable) { image.color = enable ? Color.green : Color.red; }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        boxTransform.DOAnchorPos(startPosition, 0.2f);

        callback.ResultAction();
    }
}
