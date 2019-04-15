using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{

    public string key;
    public string prefixString;
    public string suffixString;

    // Use this for initialization
    void Start()
    {
        Text text = GetComponent<Text>();
        text.text = prefixString + LocalizationManager.instance.GetLocalizedValue(key).Replace("\\n",System.Environment.NewLine) + suffixString;
    }

}