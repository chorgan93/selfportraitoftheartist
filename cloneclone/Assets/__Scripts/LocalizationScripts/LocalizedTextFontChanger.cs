using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedTextFontChanger : MonoBehaviour
{

    public Font EN_Font;
    public Font ES_Font;
    public Font FR_Font;

    private int startTextSize = -1;
    public int overrideFontSizeES = -1;
    public int overrideFontSizeFR = -1;


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
                if (ES_Font != null)
                {
                    text.font = ES_Font;
                }
                if (overrideFontSizeES > 0) {
                    text.fontSize = overrideFontSizeES;
                }
            }
            else if (LocalizationManager.currentLanguage == 2)
            {
                if (FR_Font != null)
                {
                    text.font = FR_Font;
                }
                if (overrideFontSizeFR > 0)
                {
                    text.fontSize = overrideFontSizeFR;
                }
            }
            else {
                text.fontSize = startTextSize;
                text.font = EN_Font;
            }
        }
    }

}