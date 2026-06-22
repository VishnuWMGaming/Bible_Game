using DebugUtils;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class CountryLoader : MonoBehaviour,ICountryObj
{
    [Serializable]
    public class CountryPhoneData
    {
        public string mCode;
        public string name;
        public int digitLength;
        public string phoneCode;
        public Sprite flag;
    }

    [SerializeField] private TextAsset countryDataFile;
    [SerializeField] private TextAsset phoneCodeFile;

    [Header("Countries:")]
    public List<CountryPhoneData> mCountries = new List<CountryPhoneData>();

    [SerializeField] List<CountryObj> mCountryObjs = new List<CountryObj>();

    [Header("Country Selection")]
    [SerializeField] Transform mCountryParent;
    [SerializeField] CountryObj obj;

    [Space]
    [SerializeField] TMP_InputField mSearch;

    public Action<CountryPhoneData> SetPhoneCode;


    private void OnEnable()
    {
        LoadCountries();

        mSearch.text = "";
        mSearch.onValueChanged.AddListener((val) =>
        {
            if (mCountries.Count <= 0)
                return;




            foreach (CountryObj country in mCountryObjs)
                country.gameObject.SetActive(false);


            List<CountryObj> searchedCoutries = mCountryObjs.FindAll(
                                                                 item => 
                                                                 item.Data.name.ToLower().StartsWith(val.ToLower(), StringComparison.OrdinalIgnoreCase)
                                                                );

            DevDebug.Log($" {val.ToLower()} Searched Countries: {searchedCoutries.Count}", DebugColor.Orange);

            foreach (CountryObj country in searchedCoutries)
            {
                country.gameObject.SetActive(true);
            }

        });
    }

    private void OnDisable()
    {
        mSearch.onValueChanged.RemoveAllListeners();

        ResetAction();
    }

    private void LoadCountries()
    {
        ResetAction();

        string[] lines = countryDataFile.text.Split(
            new[] { '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries);

        // Skip header row
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');

            if (parts.Length < 3)
                continue;

            string code = parts[0].Trim();

            CountryPhoneData country = new CountryPhoneData
            {
                mCode = parts[0].Trim(),
                name = string.Join(",", parts, 1, parts.Length - 2).Trim(), // Handles commas in names
                digitLength = int.Parse(parts[parts.Length - 1].Trim()),

                flag = Resources.Load<Sprite>($"Flags/{code.ToLower()}")
            };

            mCountries.Add(country);
        }

        string[] phoneCodelines = phoneCodeFile.text.Split(
                new[] { '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries);


        for(int i = 0;i< phoneCodelines.Length;++i)
        {
            string[] parts = phoneCodelines[i].Split(',');

            if (parts.Length < 2)
                continue;

            string code = parts[0].Trim();
            string phonecode = parts[1].Trim();

            mCountries.Find(x => x.mCode.ToLower() == code.ToLower()).phoneCode = phonecode;
        }


        foreach(CountryPhoneData data in mCountries)
        {
            GameObject go = Instantiate(obj.gameObject, mCountryParent);

            CountryObj country = go.GetComponent<CountryObj>();
            country.Set(data, this);

            mCountryObjs.Add(country);
        }

        DevDebug.Log($"Loaded {mCountries.Count} countries.",DebugColor.Green);
    }

    public void SelectedCountry(CountryPhoneData data)
    {
        SetPhoneCode?.Invoke(data);

        this.gameObject.SetActive(false);
    }


    void ResetAction()
    {
        for(int i=0; i<mCountryParent.childCount;++i)
        {
            GameObject go = mCountryParent.GetChild(i).gameObject;
            Destroy(go);
        }

        mCountryObjs.Clear();
        mCountries.Clear();
    }
}
