using BibleGame.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] List<LevelObj> levelObjs = new List<LevelObj>();
    public List<LevelObj> LevelObjs => levelObjs;

    bool isLoaded = false;
    public bool IsLoaded => isLoaded;

    public void Init(List<ChapterData> chapterdatas, ILevelObj callbackIN)
    {
        //ClearAll();

        isLoaded = true;

        for (int i = 0; i < levelObjs.Count; i++)
            levelObjs[i].gameObject.SetActive(false);

        for (int i=0;i< chapterdatas.Count;i++)
        {
            levelObjs[i].gameObject.SetActive(true);
            levelObjs[i].Intialise(chapterdatas[i].id, callbackIN);
        }
    }

    public void ClearAll()
    {
        for (int i = 0; i < levelObjs.Count; i++)
            levelObjs[i].Clear();
    }
}
