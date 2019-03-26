using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedTextMesh : MonoBehaviour
{

    public string key;

    // Use this for initialization
    void Start()
    {
        TextMesh text = GetComponent<TextMesh>();
        text.text = LocalizationManager.instance.GetLocalizedValue(key).Replace("\\n",System.Environment.NewLine);
    }

}