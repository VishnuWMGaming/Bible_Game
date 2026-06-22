using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CountryData", menuName = "BibleGameStore/CountryData")]
public class CountryData : ScriptableObject
{
    [Header("Countries:")]
    [SerializeField] List<Country> countries = new List<Country>();

    
    public Country GetCountry(string country)
    {
        return countries.Find(x => x.name == country);
    }

    public List<Country> GetCountries() { return countries; }
}

[Serializable]
public class Country
{
    public Sprite sprite;
    public string name;
    public string code;
}
