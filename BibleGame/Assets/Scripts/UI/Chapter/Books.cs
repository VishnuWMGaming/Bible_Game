using System;
using BibleGame;
using UnityEngine;
using UnityEngine.UI;


using BibleGame.Data;
using BibleGame.API;

public class Books : MonoBehaviour,IMBook
{
    [SerializeField] private Button backBtn;

    [Space]
    [SerializeField] GameObject mBookObj;
    [SerializeField] Transform mBookTransform;

    public IBook bookCallback;

    [Header("SpriteData")]
    [SerializeField] SpriteData spriteData;

    StyleUI styleUI;

    private void OnEnable()
    {
        //backBtn.onClick.AddListener(() => Actions.StartPageAction(StartPage.book));
        //book1.onClick.AddListener((() => bookCallback.SelectBook("bookName")));
        //book2.onClick.AddListener((() => bookCallback.SelectBook("bookName")));
        //book3.onClick.AddListener((() => bookCallback.SelectBook("bookName")));
        //book4.onClick.AddListener((() => bookCallback.SelectBook("bookName")));
        //book5.onClick.AddListener((() => bookCallback.SelectBook("bookName")));
        //book6.onClick.AddListener((() => bookCallback.SelectBook("bookName")));

        Intialise();
    }

    private void OnDisable()
    {
        backBtn.onClick.RemoveAllListeners();
        //book1.onClick.RemoveAllListeners();
        //book2.onClick.RemoveAllListeners();
        //book3.onClick.RemoveAllListeners();
        //book4.onClick.RemoveAllListeners();
        //book5.onClick.RemoveAllListeners();
        //book6.onClick.RemoveAllListeners();

        ClearAll();
    }

    void Intialise()
    {
        if (String.IsNullOrEmpty(UserData.bibleId))
            return;

        PopUp.Instance.EnableLoad(true);
        GetBiblesAPI.GetBookList((success, res) =>
        {
            PopUp.Instance.EnableLoad(false);
            if (!success)
            {
                Debug.LogError("Error in book list !!");
                return;
            }

            foreach(var book in res.ResponseData.data)
            {
                GameObject go = Instantiate(mBookObj, mBookTransform);
                go.transform.localScale = Vector3.one;

                MBook mBook = go.GetComponent<MBook>();
                mBook.Intialise(book,this);
            }

        }, UserData.bibleId);
    }

    public void SelectAction(string id)
    {
        if (String.IsNullOrEmpty(id))
            return;

        UserData.bookId = id;

        CreateStrick();
    }

    void CreateStrick()
    {
        int ageVal = UserData.currentAge switch
        {
            AgeGroup.kindergarden => 1,
            AgeGroup.elementary => 2,
            AgeGroup.teenagers => 3,
            AgeGroup.adult => 4,
            _ => 1
        };

        string testamentVal = UserData.testament switch
        {
            Testament.New => "New",
            Testament.Old => "Old",
            _ => "Old"
        };

        StreakRequest request = new StreakRequest()
        {
            bible_id = UserData.bibleId,
            book_id = UserData.bookId,
            age = ageVal.ToString(),
            testament = testamentVal,
        };

        PopUp.Instance.EnableLoad(true);
        StreakAPI.Create((success, res) =>
        {
            PopUp.Instance.EnableLoad(false);

            if (!success)
            {
                Debug.LogError("Error in creating the streak");
                return;
            }

            Debug.Log("<color=green> Streak has been created </color>");

            UserData.gameid = res.ResponseData._id;

            ChapterAPI.Get((success,res) =>
            {
                if(!success)
                {
                    Debug.LogError("Error in getting the chapters");
                    return;
                }

                GameData.mChapterDatas = res.ResponseData.data;
                Actions.StartPageAction(StartPage.selectChapter);

            }, UserData.bibleId, UserData.bookId);

           // Actions.ChangePanelActions(CanvasType.chapter);

        }, request);
    }

    public void ClearAll()
    {
        for(int i = 0;i<mBookTransform.childCount;i++)
        {
            GameObject go = mBookTransform.GetChild(i).gameObject;
            Destroy(go);
        }
    }
}

public interface IBook
{
    public void SelectBook(string bookName);
    
    public void BackToCover();
}