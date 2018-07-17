using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSpriteSwapperS : MonoBehaviour {

    public int actionNum = 0;
    public Sprite[] xboxSprites;
    public Sprite[] ps4Sprites;
    public Sprite[] keySprites;
    private SpriteRenderer myRenderer;

	// Use this for initialization
	void OnEnable () {
        if (!myRenderer)
        {
            myRenderer = GetComponent<SpriteRenderer>();
        }

        switch (ControlManagerS.controlProfile)
        {
            case 0:
                myRenderer.sprite = xboxSprites[ControlManagerS.savedGamepadControls[actionNum]];
                break;
            case 3:
                myRenderer.sprite = ps4Sprites[ControlManagerS.savedGamepadControls[actionNum]];
                break;
            case 2:
                myRenderer.sprite = keySprites[ControlManagerS.savedKeyboardControls[actionNum]];
                break;
            default:
                myRenderer.sprite = keySprites[ControlManagerS.savedKeyboardandMouseControls[actionNum]];
                break;
        }
    }
	
}
