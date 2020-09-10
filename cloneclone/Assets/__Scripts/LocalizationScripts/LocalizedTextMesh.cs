using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedTextMesh : MonoBehaviour
{

    public string key;
    private int prevLanguage = -1;

    // Use this for initialization
    void OnEnable()
    {
        Translate();
    }

    public void Translate()
    {
        if (prevLanguage != LocalizationManager.currentLanguage)
        {
            TextMesh text = GetComponent<TextMesh>();
            text.text = LocalizationManager.instance.GetLocalizedValue(key).Replace("\\n", System.Environment.NewLine);
            prevLanguage = LocalizationManager.currentLanguage;
        }
    }
}