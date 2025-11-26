using BibleGame;
using BibleGame.Data;
using DG.Tweening;
using System;
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

    
    public bool m_IsAble = false;

    [SerializeField] LayoutElement layout;

    [Header("Translate")]
    [SerializeField] TranslateLang translateLang;

    [Space]
    [SerializeField] bool isLandscape;

    private void Awake()
    {
       // boxTransform = GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>();
    }

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
        //boxTransform = GetComponent<RectTransform>();
        //canvas = FindObjectOfType<Canvas>();
       // boxTransform.anchorMax = new Vector2(0.5f, 0.5f);
        //boxTransform.anchorMin = new Vector2(0.5f, 0.5f);

        startPosition = boxTransform.anchoredPosition;
        initialPosition = boxTransform.anchoredPosition;

        isEnable = false;

        index = -1;

        isDragging = false;
        m_IsAble = true;

        Actions.ResetBoxPosAction += ResetBoxAct;

        SetLetter();
    }

    private void OnDisable()
    {
        Actions.ResetBoxPosAction -= ResetBoxAct;
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


//        boxTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    public Vector2 GetStartPos()
    {
        return this.startPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isEnable) return;
        if (isCorrect) return;

        if (!m_IsAble) return;

        // Debug.Log($"<color=grey>Begin drag:{value}</color>");

        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isEnable) return;
        if (isCorrect) return;
        if (!m_IsAble) return;

        if (!isDragging) return;

        if (canvas == null)
        {
            Debug.LogError("Canvas is null");
            return;
        }

        Vector2 newPosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            maskArea.transform as RectTransform,
            Input.mousePosition,
            null,
            out newPosition
        );

        Vector2 localPosition = AnagramUtils.GetMousePositionInCanvasSpace(canvas, isLandscape);

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

    public void SetPos(Vector2 pos, int newIndex,Action onComplete = null ,bool isRun = false)
    {
        if (!isEnable) return;
        if (isCorrect) return;

        index = newIndex;

        if(startPosition != pos)
           startPosition = pos;

        if (isRun && !m_IsAble)
            boxTransform.DOAnchorPos(pos, 0.2f).OnComplete(()=> onComplete?.Invoke());
    }

    public void SetLetter()
    {
        if (!isEnable) return;
        if (isCorrect) return;

        letterText.text = value;
    }

    public void SetLetter(string value)
    {
        if (!isEnable) return;
        if (isCorrect) return;

        //translateLang.UpdateText(() =>
        //{

        //})

        letterText.text = value;
        this.value = value;
    }

    public void SetIndex(int index)
    {
        if (!isEnable) return;
        if (isCorrect) return;

        this.index = index;
    }

    public void SetCorrectWord(bool enable) 
    { 
        letterText.color = enable ? Color.green : Color.red;
    }

    public void SetImage(AgeGroup group)
    {
        //if (!isEnable) return;

        image.color = group switch
        {
            AgeGroup.adult => new Color32(129, 227, 250, 70),
            _ => new Color32(224, 141, 43, 100)
        };

        //image.sprite = sprite;
        //image.SetNativeSize();

       // Vector2 size = boxTransform.sizeDelta;
      //  boxTransform.sizeDelta = new Vector2(size.x / 4, size.y / 4);
    }
   
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isEnable) return;
        if (!m_IsAble) return;
        if (isCorrect) return;

        if (!isDragging)
            return;

        isDragging = false;
        boxTransform.DOAnchorPos(startPosition, 0.2f);

        callback.ResultAction();
    }

    public void ResetBoxAct()
    {
        if (!isEnable) return;
        if (isCorrect) return;

        boxTransform.anchoredPosition = startPosition;
    }

    public void Restart()
    {
        callback = null;
        boxTransform.anchoredPosition = initialPosition;
        letterText.color = Color.white;

        m_IsAble = true;

        Enable(false);
    }
}
