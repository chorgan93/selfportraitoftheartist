using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizableControlsUIS : MonoBehaviour {

    private GameMenuS myParentMenu;

    public Text controlTypeText;
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
    public Sprite[] mouseIcons;

    private bool inReplaceMode = false;
    public bool InReplaceMode { get { return inReplaceMode; }}

    private int currentPos = 0;
    public RectTransform cursor;
    public Vector2 cursorOffset = new Vector2(-1, 0);

    private bool[] buttonsAreUp= new bool[12];
	
	// Update is called once per frame
	void Update () {

        HandleMovement();
        if (currentPos == 0 && !inReplaceMode)
        {
            HandleControlType();
        }
        HandleControlSwapping();
        if (Mathf.Abs(myControl.HorizontalMenu()) < 0.2f && Mathf.Abs(myControl.VerticalMenu()) < 0.2f){
            stickReset = true;
        }
        if (!myControl.GetCustomInput(12)){
            selectButtonUp = true;
        }

        if (!myControl.GetCustomInput(13)){
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

	void HandleControlType(){
        if (myControl.HorizontalMenu() > 0.2f && stickReset)
        {
            stickReset = false;
            ControlManagerS.controlProfile++;
            if ((ControlManagerS.controlProfile > 2 && !myControl.CanSelectPS4) || (ControlManagerS.controlProfile > 3 && myControl.CanSelectPS4))
            {
                if (myControl.ControllerAttached() && !myControl.CanSelectPS4)
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
        if (myControl.HorizontalMenu() < -0.2f && stickReset)
        {
            stickReset = false;
            ControlManagerS.controlProfile--;
            if ((ControlManagerS.controlProfile < 0 && myControl.ControllerAttached() && !myControl.CanSelectPS4)
                || (ControlManagerS.controlProfile < 1 && (!myControl.ControllerAttached() || myControl.CanSelectPS4)))
            {
                if (myControl.CanSelectPS4)
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

        if (selectButtonUp && myControl.GetCustomInput(12))
        {

            ControlManagerS.controlProfile++;
            if ((ControlManagerS.controlProfile > 2 && !myControl.CanSelectPS4) || (ControlManagerS.controlProfile > 3 && myControl.CanSelectPS4))
            {
                if (myControl.ControllerAttached() && !myControl.CanSelectPS4)
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

    void HandleMovement(){
        if (stickReset && !inReplaceMode){
            if (myControl.VerticalMenu() > 0.2f){
                currentPos--;
                SetCursorPos();
                stickReset = false;
            }
            if (myControl.VerticalMenu() < -0.2f){
                currentPos++;
                SetCursorPos();
                stickReset = false;
            }
        }
    }

    void HandleControlSwapping(){
        if (currentPos > 0){
            if (selectButtonUp && !inReplaceMode && myControl.GetCustomInput(12))
            {
                for (int i = 0; i < buttonsAreUp.Length; i++)
                {
                    buttonsAreUp[i] = !myControl.GetCustomInput(i);
                }
                inReplaceMode = true;
                selectButtonUp = false;
                SetInstructionText();
            }
            else if (inReplaceMode)
            {
                CheckForReplacement();
            }
        }
        if (backButtonUp && myControl.GetCustomInput(13) && !inReplaceMode){
            TurnOff(null);
        }
    }

    void SetInstructionText(){
        if (inReplaceMode){
            instructionText.text = "Press the button you wish to use for the selected action.";
        }else{
            instructionText.text = "Select the action for which you want to customize control.";
        }
    }

    void CheckForReplacement()
    {

        // gamepad control only for now
        if (ControlManagerS.controlProfile == 0 || ControlManagerS.controlProfile == 3) { 
            if (myControl.GetCustomInput(0) && buttonsAreUp[0])
        {
            myControl.SetCustomInput(currentPos-1, 0);
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

    }

    void UpdateControlImages()
    {
        for (int i = 0; i < currentControlImages.Length; i++){
            if (i == currentControlImages.Length - 1)
            {
                if (ControlManagerS.controlProfile == 0)
                {
                    currentControlImages[i].sprite = xboxButtonIcons[i];
                }
                else if (ControlManagerS.controlProfile == 3)
                {
                    currentControlImages[i].sprite = ps4ButtonIcons[i];
                }
            }
            else
            {
                if (ControlManagerS.controlProfile == 0)
                {
                    currentControlImages[i].sprite = xboxButtonIcons[ControlManagerS.savedGamepadControls[i]];
                }
                else if (ControlManagerS.controlProfile == 3)
                {
                    currentControlImages[i].sprite = ps4ButtonIcons[ControlManagerS.savedGamepadControls[i]];
                }
            }
        }
    }

    void UpdateControlSettingText(){
        if (ControlManagerS.controlProfile == 0)
        {
            currentControlTypeText.text = "Gamepad";
            Cursor.visible = false;
        }
        else if (ControlManagerS.controlProfile == 3)
        {
            currentControlTypeText.text = "Gamepad (PS4)";
            Cursor.visible = false;
        }
        else if (ControlManagerS.controlProfile == 1)
        {
            currentControlTypeText.text = "Keyboard & Mouse";
            Cursor.visible = true;
        }
        else if (ControlManagerS.controlProfile == 2)
        {
            currentControlTypeText.text = "Keyboard (No Mouse)";
            Cursor.visible = false;
        }
        UpdateControlImages();
    }

    public void TurnOn(GameMenuS newM){


        inReplaceMode = false;
        SetInstructionText();
        currentPos = 0;
        SetCursorPos();

        selectButtonUp = false;
        backButtonUp = false;
        stickReset = false;
            
        UpdateControlSettingText();


        myParentMenu = newM;
        myControl = myParentMenu.MyControl;

        gameObject.SetActive(true);
        
    }

    void SetCursorPos(){
        if (currentPos > customControlTexts.Length){
            currentPos = customControlTexts.Length;
        }
        if (currentPos < 0){
            currentPos = 0;
        }
        if (currentPos == 0){
            cursor.anchoredPosition = controlTypeText.rectTransform.anchoredPosition + cursorOffset;
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
        gameObject.SetActive(false);

        
    }
}
