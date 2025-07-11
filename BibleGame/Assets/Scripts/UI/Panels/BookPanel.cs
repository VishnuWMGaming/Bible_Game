using BibleGame.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookPanel : MonoBehaviour
{
    [Header("Book Settings:")]
    [SerializeField] GameObject bookPrefab;
    [SerializeField] Transform bookTransform;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    void Intialise()
    {
        GetBiblesAPI.GetBookList((success, res) =>
        {

        });
    }
}
