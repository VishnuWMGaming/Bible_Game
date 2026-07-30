using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class IFont : MonoBehaviour
{
    [SerializeField] TMP_Text m_Text;
    [SerializeField] TMP_FontAsset fontAsset;

    private void Start()
    {
        if (fontAsset == null || m_Text == null)
            return;

        m_Text.font = fontAsset;
        m_Text.ForceMeshUpdate();
    }
}
