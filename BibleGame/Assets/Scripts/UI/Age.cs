using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IAge
{
    public void AgeSelection(AgeGroup group);
}

[RequireComponent(typeof(Button))]
public class Age : MonoBehaviour
{
    [Header("Age Group:")]
    [SerializeField] AgeGroup ageGroup;

    Button mButton;

    public IAge callback;

    private void OnEnable()
    {
        mButton = GetComponent<Button>();
        mButton?.onClick.AddListener(() => callback.AgeSelection(ageGroup));
    }

    private void OnDisable()
    {
        mButton?.onClick.RemoveAllListeners();
    }
}

public enum AgeGroup
{
   kindergarden,elementary,teenagers,adult
}
