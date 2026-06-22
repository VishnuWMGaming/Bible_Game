using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CountryLoader;

public interface ICountryObj
{
    public void SelectedCountry(CountryPhoneData data); 
}

[RequireComponent(typeof(Button))]
public class CountryObj : MonoBehaviour
{
    [SerializeField] TMP_Text mName;
    [SerializeField] Image countryImg;

    public ICountryObj callback;

    [SerializeField] CountryPhoneData currentData;
    public CountryPhoneData Data => currentData;

    Button button;

    private void OnEnable()
    {
        button = GetComponent<Button>();
    }

    public void Set(CountryPhoneData data,ICountryObj callbackIN)
    {
        currentData = data;

        mName.text = data.name;
        countryImg.sprite = data.flag;

        callback = callbackIN;

        button?.onClick.RemoveAllListeners();
        button?.onClick.AddListener(() =>
        {
            callback?.SelectedCountry(data);
        });
    } 
}
