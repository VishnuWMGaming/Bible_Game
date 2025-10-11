#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class SceneSearch : ScriptableObject, ISearchWindowProvider
{
    private List<SceneData> sceneList;
    private Action<SceneData> onSetCallback;
    public SceneSearch(List<SceneData> sceneList, Action<SceneData> callback)
    {
        this.sceneList = sceneList;
        onSetCallback = callback;
          
    }

        
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
        searchList.Add(new SearchTreeGroupEntry(new GUIContent("Select Scene"), 0));


        sceneList = sceneList.OrderBy((data => data.GroupName)).ToList();

        string groupName = sceneList[0].GroupName;
        searchList.Add(new SearchTreeGroupEntry(new GUIContent(groupName), 1));
        
        foreach (var sceneData in sceneList)
        {
            if (groupName != sceneData.GroupName)
            {
                groupName = sceneData.GroupName;
                searchList.Add(new SearchTreeGroupEntry(new GUIContent(groupName), 1));
            }
            var searchTreeEntry = new SearchTreeEntry(new GUIContent(sceneData.sceneName));
            searchTreeEntry.level = 2;
            searchTreeEntry.userData = sceneData;
            searchList.Add(searchTreeEntry);
        }

        return searchList;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        onSetCallback?.Invoke((SceneData)SearchTreeEntry.userData);
        return true;
    }

        
}
#endif