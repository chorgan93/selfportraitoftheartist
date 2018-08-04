using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteSwapperS : MonoBehaviour {

    public int actionNum = 0;
    public Sprite[] xboxSprites;
    public Sprite[] ps4Sprites;
    public Sprite[] keySprites;
    private SpriteRenderer myRenderer;
    private Image myImage;
    private bool useImage = false;

	// Use this for initialization
	void OnEnable () {
        if (!myRenderer)
        {
            if (GetComponent<SpriteRenderer>() != null)
            {
                myRenderer = GetComponent<SpriteRenderer>();
            }else{
                myImage = GetComponent<Image>();
                useImage = true;
            }
        }

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
                    spriteNumToUse = ControlManagerS.savedGamepadControls[actionNum];
                }
                if (useImage)
                {
                    myImage.sprite = xboxSprites[spriteNumToUse];
                }
                else
                {
                    myRenderer.sprite = xboxSprites[spriteNumToUse];
                }
                break;
            case 3:
                if (actionNum >= ControlManagerS.savedGamepadControls.Count)
                {
                    spriteNumToUse = 14; // we are looking for the left stick/movement option (not saved in controls, but in sprite database)
                }
                else
                {
                    spriteNumToUse = ControlManagerS.savedGamepadControls[actionNum];
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
                {
                    spriteNumToUse = ControlManagerS.savedKeyboardControls[actionNum];
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
                {
                    spriteNumToUse = ControlManagerS.savedKeyboardandMouseControls[actionNum];
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
