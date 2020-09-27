using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{

    public string key;
    public string prefixString;
    public string suffixString;
    public string removeString = "";

    private int prevLanguage = -1;

    // Use this for initialization
    void OnEnable()
    {
        Translate();
    }

    public void Translate() {
        if (prevLanguage != LocalizationManager.currentLanguage)
        {
            Text text = GetComponent<Text>();
            text.text = prefixString + LocalizationManager.instance.GetLocalizedValue(key).Replace("\\n", System.Environment.NewLine) + suffixString;
            if (removeString != "") {
                text.text = text.text.Replace(removeString, "");
            }
            prevLanguage = LocalizationManager.currentLanguage;
        }
    }

}