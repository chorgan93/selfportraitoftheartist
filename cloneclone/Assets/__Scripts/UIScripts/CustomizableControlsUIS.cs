using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizableControlsUIS : MonoBehaviour
{

    private GameMenuS myParentMenu;

    public Text controlTypeText;
    public Text restoreDefaultText;
    public Text currentControlTypeText;

    public Text[] customControlTexts;
    public Image[] currentControlImages;

    public Text instructionText;

    private ControlManagerS myControl;

    private bool stickReset = false;
    private bool selectButtonUp = false;
    private bool backButtonUp = false;

    [Header("Sprite Databases")]
    public Sprite[] xboxButtonIcons;
    public Sprite[] ps4ButtonIcons;
    public Sprite[] keyboardButtonIcons;
    public Sprite[] nintendoButtonIcons;

    private bool inReplaceMode = false;
    public bool InReplaceMode { get { return inReplaceMode; } }

    private int currentPos = 0;
    public RectTransform cursor;
    public Vector2 cursorOffset = new Vector2(-1, 0);

    private bool[] buttonsAreUp = new bool[12];

    private int checkKeyPress = -1;
    private bool allKeysUp = false;

    // Update is called once per frame
    void Update()
    {

        HandleMovement();
        if (currentPos == 0 && !inReplaceMode)
        {
            HandleControlType();
        }
        if (currentPos == customControlTexts.Length + 1)
        {
            HandleRestoreDefaults();
        }
        HandleControlSwapping();
#if UNITY_SWITCH
        if (Mathf.Abs(myControl.HorizontalMenu()) < 0.5f && Mathf.Abs(myControl.VerticalMenu()) < 0.5f)
        {
#else
        if (Mathf.Abs(myControl.HorizontalMenu()) < 0.2f && Mathf.Abs(myControl.VerticalMenu()) < 0.2f)
        {
#endif
            stickReset = true;
        }
        if (!myControl.GetCustomInput(3))
        {
            selectButtonUp = true;
        }

        if (!myControl.GetCustomInput(1))
        {
            backButtonUp = true;
        }

    }

    private void LateUpdate()
    {
        if (myControl)
        {
            for (int i = 0; i < buttonsAreUp.Length; i++)
            {
                buttonsAreUp[i] = !myControl.GetCustomInput(i);
            }
        }
    }

    void HandleRestoreDefaults()
    {
        if (selectButtonUp && myControl.GetCustomInput(3))
        {

            myControl.RestoreDefaults();
            UpdateControlImages();
            selectButtonUp = false;

        }
    }

    void HandleControlType()
    {
#if UNITY_SWITCH
        if (myControl.HorizontalMenu() > 0.5f && stickReset)
#else
        if (myControl.HorizontalMenu() > 0.2f && stickReset)
#endif
        {
            stickReset = false;
            ControlManagerS.controlProfile++;
            if ((ControlManagerS.controlProfile > 2 && !myControl.canSelectPS4) || (ControlManagerS.controlProfile > 3 && myControl.canSelectPS4))
            {
                if (myControl.ControllerAttached() && !myControl.canSelectPS4)
                {
                    ControlManagerS.controlProfile = 0;
                }
                else
                {
                    ControlManagerS.controlProfile = 1;
                }
            }
            UpdateControlSettingText();
        }
#if UNITY_SWITCH
        if (myControl.HorizontalMenu() < -0.5f && stickReset)
#else
        if (myControl.HorizontalMenu() < -0.2f && stickReset)
#endif
        {
            stickReset = false;
            ControlManagerS.controlProfile--;
            if ((ControlManagerS.controlProfile < 0 && myControl.ControllerAttached() && !myControl.canSelectPS4)
                || (ControlManagerS.controlProfile < 1 && (!myControl.ControllerAttached() || myControl.canSelectPS4)))
            {
                if (myControl.canSelectPS4)
                {
                    ControlManagerS.controlProfile = 3;
                }
                else
                {
                    ControlManagerS.controlProfile = 2;
                }
            }
            UpdateControlSettingText();
        }

        if (selectButtonUp && myControl.GetCustomInput(3))
        {

            ControlManagerS.controlProfile++;
            if ((ControlManagerS.controlProfile > 2 && !myControl.canSelectPS4) || (ControlManagerS.controlProfile > 3 && myControl.canSelectPS4))
            {
                if (myControl.ControllerAttached() && !myControl.canSelectPS4)
                {
                    ControlManagerS.controlProfile = 0;
                }
                else
                {
                    ControlManagerS.controlProfile = 1;
                }
            }
            selectButtonUp = false;

            UpdateControlSettingText();
        }
    }

    void HandleMovement()
    {
        if (stickReset && !inReplaceMode)
        {
#if UNITY_SWITCH
            if (myControl.VerticalMenu() > 0.5f)
            {
#else
            if (myControl.VerticalMenu() > 0.2f)
            {
#endif
                currentPos--;
                SetCursorPos();
                stickReset = false;
            }
#if UNITY_SWITCH
            if (myControl.VerticalMenu() < -0.5f)
            {
#else
            if (myControl.VerticalMenu() < -0.2f)
            {
#endif
                currentPos++;
                SetCursorPos();
                stickReset = false;
            }
        }
    }

    void HandleControlSwapping()
    {
        if (currentPos > 0 && currentPos < customControlTexts.Length + 1)
        {
            if (selectButtonUp && !inReplaceMode && myControl.GetCustomInput(3))
            {
                for (int i = 0; i < buttonsAreUp.Length; i++)
                {
                    buttonsAreUp[i] = !myControl.GetCustomInput(i);
                }
                inReplaceMode = true;
                selectButtonUp = false;
                allKeysUp = false;
                SetInstructionText();
            }
            else if (inReplaceMode)
            {
                CheckForReplacement();
            }
        }
        if (backButtonUp && myControl.GetCustomInput(1) && !inReplaceMode)
        {
            TurnOff(null);
        }
    }

    void SetInstructionText()
    {
        if (inReplaceMode)
        {
            instructionText.text = LocalizationManager.instance.GetLocalizedValue("options_control_replace");
        }
        else
        {
            instructionText.text = LocalizationManager.instance.GetLocalizedValue("options_control_instruction");
        }
    }

    void CheckForReplacement()
    {

        // gamepad control only for now
        if (ControlManagerS.controlProfile == 0 || ControlManagerS.controlProfile == 3)
        {
            if (myControl.GetCustomInput(0) && buttonsAreUp[0])
            {
                myControl.SetCustomInput(currentPos - 1, 0);
                UpdateControlImages();
                inReplaceMode = false;
                backButtonUp = selectButtonUp = false;
                SetInstructionText();
            }
            else if (myControl.GetCustomInput(1) && buttonsAreUp[1])
            {
                myControl.SetCustomInput(currentPos - 1, 1);
                UpdateControlImages();
                inReplaceMode = false;
                backButtonUp = selectButtonUp = false;
                SetInstructionText();
            }
            else if (myControl.GetCustomInput(2) && buttonsAreUp[2])
            {
                myControl.SetCustomInput(currentPos - 1, 2);
                UpdateControlImages();
                inReplaceMode = false;
                backButtonUp = selectButtonUp = false;
                SetInstructionText();
            }
            else if (myControl.GetCustomInput(3) && buttonsAreUp[3])
            {
                myControl.SetCustomInput(currentPos - 1, 3);
                UpdateControlImages();
                inReplaceMode = false;
                backButtonUp = selectButtonUp = false;
                SetInstructionText();
            }
            else if (myControl.GetCustomInput(4) && buttonsAreUp[4])
            {
                myControl.SetCustomInput(currentPos - 1, 4);
                UpdateControlImages();
                inReplaceMode = false;
                backButtonUp = selectButtonUp = false;
                SetInstructionText();
            }
            else if (myControl.GetCustomInput(5) && buttonsAreUp[5])
            {
                myControl.SetCustomInput(currentPos - 1, 5);
                UpdateControlImages();
                inReplaceMode = false;
                backButtonUp = selectButtonUp = false;
                SetInstructionText();
            }
            else if (myControl.GetCustomInput(6) && buttonsAreUp[6])
            {
                myControl.SetCustomInput(currentPos - 1, 6);
                UpdateControlImages();
                inReplaceMode = false;
                backButtonUp = selectButtonUp = false;
                SetInstructionText();
            }
            else if (myControl.GetCustomInput(7) && buttonsAreUp[7])
            {
                myControl.SetCustomInput(currentPos - 1, 7);
                UpdateControlImages();
                inReplaceMode = false;
                backButtonUp = selectButtonUp = false;
                SetInstructionText();
            }
            else if (myControl.GetCustomInput(8) && buttonsAreUp[8])
            {
                myControl.SetCustomInput(currentPos - 1, 8);
                UpdateControlImages();
                inReplaceMode = false;
                backButtonUp = selectButtonUp = false;
                SetInstructionText();
            }
            else if (myControl.GetCustomInput(9) && buttonsAreUp[9])
            {
                myControl.SetCustomInput(currentPos - 1, 9);
                UpdateControlImages();
                inReplaceMode = false;
                backButtonUp = selectButtonUp = false;
                SetInstructionText();
            }
            else if (myControl.GetCustomInput(10) && buttonsAreUp[10])
            {
                myControl.SetCustomInput(currentPos - 1, 10);
                UpdateControlImages();
                inReplaceMode = false;
                backButtonUp = selectButtonUp = false;
                SetInstructionText();
            }
            else if (myControl.GetCustomInput(11) && buttonsAreUp[11])
            {
                myControl.SetCustomInput(currentPos - 1, 11);
                UpdateControlImages();
                inReplaceMode = false;
                backButtonUp = selectButtonUp = false;
                SetInstructionText();
            }

        }
        else
        {
            checkKeyPress = myControl.CheckForKeyPress();
            if (!allKeysUp && checkKeyPress < 0)
            {
                allKeysUp = true;
            }
            if (allKeysUp && checkKeyPress > -1 && (ControlManagerS.controlProfile == 1
                                                    || (ControlManagerS.controlProfile == 2 && (checkKeyPress < 14 || checkKeyPress > 16))))
            {
                myControl.SetCustomInput(currentPos - 1, checkKeyPress);
                UpdateControlImages();
                inReplaceMode = false;
                backButtonUp = selectButtonUp = false;
                SetInstructionText();
                checkKeyPress = -1;
                allKeysUp = false;
            }
        }

    }

    void UpdateControlImages()
    {
        for (int i = 0; i < currentControlImages.Length; i++)
        {
            if (i == currentControlImages.Length - 1)
            {
                if (ControlManagerS.controlProfile == 0)
                {
#if UNITY_SWITCH
                    currentControlImages[i].sprite = nintendoButtonIcons[i];
#else
                    currentControlImages[i].sprite = xboxButtonIcons[i];
#endif
                }
                else if (ControlManagerS.controlProfile == 3)
                {
                    currentControlImages[i].sprite = ps4ButtonIcons[i];
                }
                else
                {
                    currentControlImages[i].sprite = keyboardButtonIcons[keyboardButtonIcons.Length - 1];
                }
            }else if (i == 12){
                // set equal to i = 3
                if (ControlManagerS.controlProfile == 0)
                {
#if UNITY_SWITCH
                    currentControlImages[i].sprite = nintendoButtonIcons[ControlManagerS.savedGamepadControls[3]];
#else
                    currentControlImages[i].sprite = xboxButtonIcons[ControlManagerS.savedGamepadControls[3]];
#endif
                }
                else if (ControlManagerS.controlProfile == 3)
                {
                    currentControlImages[i].sprite = ps4ButtonIcons[ControlManagerS.savedGamepadControls[3]];
                }
                else if (ControlManagerS.controlProfile == 2)
                {
                    currentControlImages[i].sprite = keyboardButtonIcons[ControlManagerS.savedKeyboardControls[3]];
                }
                else
                {
                    currentControlImages[i].sprite = keyboardButtonIcons[ControlManagerS.savedKeyboardandMouseControls[3]];
                }
            }
            else if (i == 13){
                // set equal to i == 1
                if (ControlManagerS.controlProfile == 0)
                {
#if UNITY_SWITCH
                    currentControlImages[i].sprite = nintendoButtonIcons[ControlManagerS.savedGamepadControls[1]];
#else
                    currentControlImages[i].sprite = xboxButtonIcons[ControlManagerS.savedGamepadControls[1]];
#endif
                }
                else if (ControlManagerS.controlProfile == 3)
                {
                    currentControlImages[i].sprite = ps4ButtonIcons[ControlManagerS.savedGamepadControls[1]];
                }
                else if (ControlManagerS.controlProfile == 2)
                {
                    currentControlImages[i].sprite = keyboardButtonIcons[ControlManagerS.savedKeyboardControls[1]];
                }
                else
                {
                    currentControlImages[i].sprite = keyboardButtonIcons[ControlManagerS.savedKeyboardandMouseControls[1]];
                }
            }
            else
            {
                if (ControlManagerS.controlProfile == 0)
                {
#if UNITY_SWITCH
                    currentControlImages[i].sprite = nintendoButtonIcons[ControlManagerS.savedGamepadControls[i]];
#else
                    currentControlImages[i].sprite = xboxButtonIcons[ControlManagerS.savedGamepadControls[i]];
#endif
                }
                else if (ControlManagerS.controlProfile == 3)
                {
                    currentControlImages[i].sprite = ps4ButtonIcons[ControlManagerS.savedGamepadControls[i]];
                }
                else if (ControlManagerS.controlProfile == 2)
                {
                    currentControlImages[i].sprite = keyboardButtonIcons[ControlManagerS.savedKeyboardControls[i]];
                }
                else
                {
                    currentControlImages[i].sprite = keyboardButtonIcons[ControlManagerS.savedKeyboardandMouseControls[i]];
                }
            }
        }


    }

    void UpdateControlSettingText()
    {
        if (ControlManagerS.controlProfile == 0)
        {
            currentControlTypeText.text = LocalizationManager.instance.GetLocalizedValue("options_control_gamepad");
            Cursor.visible = false;
        }
        else if (ControlManagerS.controlProfile == 3)
        {
            currentControlTypeText.text = LocalizationManager.instance.GetLocalizedValue("options_control_ps4");
            Cursor.visible = false;
        }
        else if (ControlManagerS.controlProfile == 1)
        {
            currentControlTypeText.text = LocalizationManager.instance.GetLocalizedValue("options_control_mouse");
            Cursor.visible = true;
        }
        else if (ControlManagerS.controlProfile == 2)
        {
            currentControlTypeText.text = LocalizationManager.instance.GetLocalizedValue("options_control_keyboard");
            Cursor.visible = false;
        }
        UpdateControlImages();
    }

    public void TurnOn(GameMenuS newM)
    {


        inReplaceMode = false;
        SetInstructionText();
        currentPos = 0;
#if UNITY_SWITCH
        // no control type option, so remove and set default position to first button
        currentControlTypeText.gameObject.SetActive(false);
        currentPos = 1;
#endif
        SetCursorPos();

        selectButtonUp = false;
        backButtonUp = false;
        stickReset = false;

        UpdateControlSettingText();
        allKeysUp = false;

        myParentMenu = newM;
        myControl = myParentMenu.MyControl;
        if (myControl.ControllerAttached())
        {
            myControl.DetermineControllerType();
        }
        checkKeyPress = -1;
        gameObject.SetActive(true);
    }

    void SetCursorPos()
    {
        if (currentPos > customControlTexts.Length + 1)
        {
            currentPos = customControlTexts.Length + 1;
        }
#if UNITY_SWITCH
        // there's no alt control types on switch, so disable 
        if (currentPos < 1){
            currentPos = 1;
        }
#else
        if (currentPos < 0){
            currentPos = 0;
        }
#endif
        if (currentPos == 0){
            cursor.anchoredPosition = controlTypeText.rectTransform.anchoredPosition + cursorOffset;
        }else if (currentPos == customControlTexts.Length+1){
            cursor.anchoredPosition = restoreDefaultText.rectTransform.anchoredPosition + cursorOffset;
        }else{

            cursor.anchoredPosition = customControlTexts[currentPos-1].rectTransform.anchoredPosition + cursorOffset;
        }
    }

    public void TurnOff(GameMenuS newM){

        if (newM)
        {
            myParentMenu = newM;
        }
        if (myParentMenu)
        {
            myParentMenu.BackFromControlMenu();
        }
        inReplaceMode = false;
        checkKeyPress = -1;
        allKeysUp = false;
        gameObject.SetActive(false);

        
    }
}
