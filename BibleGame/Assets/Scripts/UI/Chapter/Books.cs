using System;
using BibleGame;
using UnityEngine;
using UnityEngine.UI;


using BibleGame.Data;
using BibleGame.API;

using System.Linq;

public class Books : MonoBehaviour, IMBook
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
        backBtn.onClick.AddListener(OnBackClicked);
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
    private void OnBackClicked()
    {
        // Open TestamentCanvas
        Actions.ChangePanelActions(CanvasType.testament);
        // Optional cleanup
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

            Debug.Log($"<color=grey> book list count : {res.ResponseData.Count} </color>");

            AppData.bookDatas = UserData.testament switch
            {
                Testament.Old => res.ResponseData.GetRange(0, 39),
                Testament.New => res.ResponseData.GetRange(39, 27),
                _ => throw new NotImplementedException()
            };

            if (AppData.bookDatas == null)
            {
                Debug.LogError("Unable to fetch book data");
                return;
            }

            foreach (var book in AppData.bookDatas)
            {
                GameObject go = Instantiate(mBookObj, mBookTransform);
                go.transform.localScale = Vector3.one;

                go.gameObject.name = book.name;

                MBook mBook = go.GetComponent<MBook>();
                mBook.Intialise(book, this);
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

            if (res.ResponseMessage == "Already streak created.")
            {
                PopUp.Instance.ShowMessage("streak has been already created", () => Actions.ChangePanelActions(CanvasType.selectStreak));
                return;
            }

            Debug.Log($"<color=green> Streak has been created: {res.ResponseData._id}</color>");

            UserData.gameid = res.ResponseData._id;

            PopUp.Instance.EnableLoad(true);
            ChapterAPI.Get((success, res) =>
            {
                PopUp.Instance.EnableLoad(false);

                if (!success)
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
        for (int i = 0; i < mBookTransform.childCount; i++)
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