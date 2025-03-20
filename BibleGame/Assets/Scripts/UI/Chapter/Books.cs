using System;
using BibleGame;
using UnityEngine;
using UnityEngine.UI;

public class Books : MonoBehaviour
{
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button backBtn;
    [SerializeField] private Button book1;
    [SerializeField] private Button book2;
    [SerializeField] private Button book3;
    [SerializeField] private Button book4;
    [SerializeField] private Button book5;
    [SerializeField] private Button book6;

    public IBook bookCallback;

    private void OnEnable()
    {
        homeBtn.onClick.AddListener(() => Actions.ChangePanelActions(CanvasType.home));
        backBtn.onClick.AddListener((() => bookCallback.BackToCover()));
        book1.onClick.AddListener((() => bookCallback.SelectBook("bookName")));
        book2.onClick.AddListener((() => bookCallback.SelectBook("bookName")));
        book3.onClick.AddListener((() => bookCallback.SelectBook("bookName")));
        book4.onClick.AddListener((() => bookCallback.SelectBook("bookName")));
        book5.onClick.AddListener((() => bookCallback.SelectBook("bookName")));
        book6.onClick.AddListener((() => bookCallback.SelectBook("bookName")));
    }

    private void OnDisable()
    {
        homeBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
        book1.onClick.RemoveAllListeners();
        book2.onClick.RemoveAllListeners();
        book3.onClick.RemoveAllListeners();
        book4.onClick.RemoveAllListeners();
        book5.onClick.RemoveAllListeners();
        book6.onClick.RemoveAllListeners();
    }
}

public interface IBook
{
    public void SelectBook(string bookName);
    
    public void BackToCover();
}