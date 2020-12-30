using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedTextFontChanger : MonoBehaviour
{

    public Font EN_Font;
    public Font ES_Font;

    private int startTextSize = -1;
    public int overrideFontSizeES = -1;

    private int prevLanguage = -1;

    // Use this for initialization
    void OnEnable()
    {
        if (startTextSize < 0) {
            startTextSize = GetComponent<Text>().fontSize;
        }
        Translate();
    }

    public void Translate() {
        if (prevLanguage != LocalizationManager.currentLanguage)
        {
            Text text = GetComponent<Text>();
            if (LocalizationManager.currentLanguage == 1)
            {
                text.font = ES_Font;
                if (overrideFontSizeES > 0) {
                    text.fontSize = overrideFontSizeES;
                }
            }
            else {
                text.fontSize = startTextSize;
                text.font = EN_Font;
            }
        }
    }

}