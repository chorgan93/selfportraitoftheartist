using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteSwapperS : MonoBehaviour
{

    public int actionNum = 0;
    private int startActionNum = -1;
    public Sprite[] xboxSprites;
    public Sprite[] ps4Sprites;
    public Sprite[] keySprites;
    public Sprite[] nintendoSprites;
    private SpriteRenderer myRenderer;
    private Image myImage;
    private bool useImage = false;

    [Header("Specialty Case")]
    public bool useDefaults = false;
    public int overrideForKeyboard = -1;
    public bool overrideColor = false;
    public Color colorToOverrideWith = Color.blue;

    // Use this for initialization
    void OnEnable()
    {
        if (startActionNum < 0)
        {
            startActionNum = actionNum;
        }
        if (!myRenderer)
        {
            if (GetComponent<SpriteRenderer>() != null)
            {
                myRenderer = GetComponent<SpriteRenderer>();
            }
            else
            {
                myImage = GetComponent<Image>();
                useImage = true;
            }
        }

        if ((ControlManagerS.controlProfile == 1 || ControlManagerS.controlProfile == 2) && overrideForKeyboard >= 0)
        {
            actionNum = overrideForKeyboard;
        }
        else { actionNum = startActionNum; }

        int spriteNumToUse = 0;
        switch (ControlManagerS.controlProfile)
        {

            case 0:
                if (actionNum >= ControlManagerS.savedGamepadControls.Count)
                {
                    spriteNumToUse = 14; // we are looking for the left stick/movement option (not saved in controls, but in sprite database)
                }
                else
                {
                    if (useDefaults)
                    {
                        spriteNumToUse = ControlManagerS.defaultGamepadControls[actionNum];
                    }
                    else
                    {
                        spriteNumToUse = ControlManagerS.savedGamepadControls[actionNum];
                    }
                }
#if UNITY_SWITCH
                if (useImage)
                {
                    myImage.sprite = nintendoSprites[spriteNumToUse];
                    if (overrideColor) {
                        myImage.color = colorToOverrideWith;
                    }
                }
                else
                {
                    myRenderer.sprite = nintendoSprites[spriteNumToUse];
                    if (overrideColor)
                    {
                        myImage.color = colorToOverrideWith;
                    }
                }
#else
                if (useImage)
                {
                    myImage.sprite = xboxSprites[spriteNumToUse];
                }
                else
                {
                    myRenderer.sprite = xboxSprites[spriteNumToUse];
                }
#endif
                break;
            case 3:
                if (actionNum >= ControlManagerS.savedGamepadControls.Count)
                {
                    spriteNumToUse = 14; // we are looking for the left stick/movement option (not saved in controls, but in sprite database)
                }
                else
                {if (useDefaults)
                    {
                        spriteNumToUse = ControlManagerS.defaultGamepadControls[actionNum];
                    }
                    else
                    {
                        spriteNumToUse = ControlManagerS.savedGamepadControls[actionNum];
                    }
                }
                if (useImage)
                {
                    myImage.sprite = ps4Sprites[spriteNumToUse];
                }
                else
                {
                    myRenderer.sprite = ps4Sprites[spriteNumToUse];
                }
                break;
            case 2:
                if (actionNum >= ControlManagerS.savedKeyboardControls.Count)
                {
                    spriteNumToUse = 58; // we are looking for the left stick/movement option (not saved in controls, but in sprite database)
                }
                else
                {if (useDefaults)
                    {
                        spriteNumToUse = ControlManagerS.defaultKeyAndMouseControls[actionNum];
                    }
                    else
                    {
                        spriteNumToUse = ControlManagerS.savedKeyboardControls[actionNum];
                    }
                }
                if (useImage)
                {
                    myImage.sprite = keySprites[spriteNumToUse];
                }
                else
                {
                    myRenderer.sprite = keySprites[spriteNumToUse];
                }
                break;
            default:
                if (actionNum >= ControlManagerS.savedKeyboardandMouseControls.Count)
                {
                    spriteNumToUse = 58; // we are looking for the left stick/movement option (not saved in controls, but in sprite database)
                }
                else
                {if (useDefaults)
                    {
                        spriteNumToUse = ControlManagerS.defaultKeyAndMouseControls[actionNum];
                    }
                    else
                    {
                        spriteNumToUse = ControlManagerS.savedKeyboardandMouseControls[actionNum];
                    }
                }
                if (useImage)
                {
                    myImage.sprite = keySprites[spriteNumToUse];
                }
                else
                {
                    myRenderer.sprite = keySprites[spriteNumToUse];
                }
                break;
        }
    }
	
}
