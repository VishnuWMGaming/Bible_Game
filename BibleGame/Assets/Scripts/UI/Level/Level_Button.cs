using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using DG.Tweening;

public interface ILevelButton
{
    public void OnLevelSelectAction(int level);
}

[RequireComponent(typeof(Button))]
public class Level_Button : MonoBehaviour
{
    Button button;

    [Header("UI Settings")]
    [SerializeField] List<GameObject> stars = new List<GameObject>();

    [Tooltip("Enter the level value which it represents")]
    [SerializeField] int _levelIndex = 0;

    public int Level => _levelIndex ;

    public ILevelButton callback;

    /// <summary>
    /// Action  implemented on enable
    /// </summary>
    private void OnEnable()
    {
        if (callback == null) 
        button = GetComponent<Button>();

        button.onClick.AddListener(() => LevelSelect(_levelIndex));
    }

    /// <summary>
    /// Action implemented on disable
    /// </summary>
    private void OnDisable()
    {
        if (button != null) 
            button.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Action implemented on level select
    /// </summary>
    /// <param name="level"></param>
    void LevelSelect(int level)
    {
        Display(true);

        callback.OnLevelSelectAction(level);
    }


    void DisplayStars(int value)
    {
        if(value >3 || value <=0)
            return;

        for(int i=0 ; i<stars.Count; i++)
            stars[i].SetActive(false);

        for(int i = 0; i< value; ++i)
            stars[i].SetActive(true);
    }

    /// <summary>
    /// Display animation of the button
    /// </summary>
    /// <param name="isSelect"></param>
    public void Display(bool isSelect)
    {
        if (isSelect)
            button.gameObject.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 1.0f);
        else
            button.gameObject.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 1.0f);
    }
}
