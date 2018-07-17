using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadFileMenu : MonoBehaviour
{

    public TextMesh instructionText;
    private string instructionLoad = "Select save file to load.";
    private string instructionOverwrite = "File limit reached. Select file to overwrite.";

    private bool overwriteActive = false;
    public GameObject overwriteContainer;
    public TextMesh overwriteYes;
    public TextMesh overwriteNo;
    private int currentOverwritePos = 0;
    public SpriteRenderer overwriteCursor;

    private MainMenuNavigationS myMenu;
    private bool stickReset = false;
    private bool selectButtonUp = false;
    private bool backButtonUp = false;

    private bool willNeedToOverwrite = true;

    public LoadFileS[] myLoadFiles;
    private int currentLoadFile;
    public SpriteRenderer loadFileCursor;

    public void TurnOn(MainMenuNavigationS myM, int currentLoad = 0, bool forOverwrite = false){
        myMenu = myM;
        if (forOverwrite)
        {
            currentLoadFile = 0;
        }
        else
        {
            currentLoadFile = currentLoad;
        }
        loadFileCursor.transform.position = myLoadFiles[currentLoad].nameText.transform.position;

        willNeedToOverwrite = forOverwrite;
        if (forOverwrite){
            instructionText.text = instructionOverwrite;
        }else{
            instructionText.text = instructionLoad;
        }
        selectButtonUp = false;
        backButtonUp = false;
        stickReset = false;
            overwriteActive = false;
        overwriteContainer.SetActive(false);
        currentOverwritePos = 0;
        overwriteCursor.transform.position = overwriteNo.transform.position;
        gameObject.SetActive(true);
    }

    void TurnOnOverwriteDialogue(){

        overwriteContainer.transform.position = myLoadFiles[currentLoadFile].transform.position;
        myLoadFiles[currentLoadFile].turnOffOnNonexist.SetActive(false);
            overwriteActive = true;
        overwriteContainer.SetActive(true);
        currentOverwritePos = 0;
        selectButtonUp = false;
        backButtonUp = false;
        stickReset = false;
        overwriteCursor.transform.position = overwriteNo.transform.position;
    }

	private void Update()
    {
        stickReset |= (Mathf.Abs(myMenu.controlRef.HorizontalMenu()) < 0.1f && Mathf.Abs(myMenu.controlRef.VerticalMenu()) < 0.1f);
        selectButtonUp |= !myMenu.controlRef.GetCustomInput(12);
        backButtonUp |= !myMenu.controlRef.GetCustomInput(13);
        if (overwriteActive){
            if (stickReset && Mathf.Abs(myMenu.controlRef.HorizontalMenu()) > 0.1f){
                if (myMenu.controlRef.HorizontalMenu() < 0){
                    currentOverwritePos--;
                    if (currentOverwritePos < 0){
                        currentOverwritePos = 0;
                    }
                    overwriteCursor.transform.position = overwriteNo.transform.position;
                }else{
                    currentOverwritePos++;
                    if (currentOverwritePos > 1)
                    {
                        currentOverwritePos = 1;
                    }
                    overwriteCursor.transform.position = overwriteYes.transform.position;
                }
                stickReset = false;
            }
            if (backButtonUp && myMenu.controlRef.GetCustomInput(13)){
                if (currentOverwritePos == 0)
                {
                    // cancel overwrite
                    overwriteActive = false;
                    overwriteContainer.SetActive(false);
                    myLoadFiles[currentLoadFile].turnOffOnNonexist.SetActive(true);

        selectButtonUp = false;
                    backButtonUp = false;
                    stickReset = false;
                }
                else
                {
                    currentOverwritePos = 0;

                    overwriteCursor.transform.position = overwriteNo.transform.position;
                }
            }
            if (selectButtonUp && myMenu.controlRef.GetCustomInput(12)){
                if (currentOverwritePos == 0){
                    // cancel overwrite
                    overwriteActive = false;
                    overwriteContainer.SetActive(false);
                    myLoadFiles[currentLoadFile].turnOffOnNonexist.SetActive(true);

        selectButtonUp = false;
                    backButtonUp = false;
                    stickReset = false;
                }else if (currentOverwritePos == 1){
                    // overwrite file
                    BackToMain(true, true);
                }
            }
        }else{
            if (stickReset && Mathf.Abs(myMenu.controlRef.VerticalMenu()) > 0.1f)
            {
                if (myMenu.controlRef.VerticalMenu() < 0)
                {
                    currentLoadFile++;
                    if (currentLoadFile >= myMenu.NumSaveFiles)
                    {
                        currentLoadFile = 0;
                    }
                    loadFileCursor.transform.position = myLoadFiles[currentLoadFile].nameText.transform.position;
                }
                else
                {
                    currentLoadFile--;
                    if (currentLoadFile < 0)
                    {
                        currentLoadFile = myMenu.NumSaveFiles-1;
                    }
                    loadFileCursor.transform.position = myLoadFiles[currentLoadFile].nameText.transform.position;
                }
                stickReset = false;
            }
            if (backButtonUp && myMenu.controlRef.GetCustomInput(13))
            {
                BackToMain();
            }
            if (selectButtonUp && myMenu.controlRef.GetCustomInput(12))
            {
                if (willNeedToOverwrite) {
                    TurnOnOverwriteDialogue();
                }
                else
                {
                    BackToMain(false, true);
                }
            }
            
        }
	}

    private void BackToMain(bool overwrite = false, bool loadComplete = false){
        currentOverwritePos = 0;
        gameObject.SetActive(false);
                    overwriteActive = false;
        overwriteContainer.SetActive(false);
        myMenu.inLoadMenu = false;
        if (loadComplete){
            SaveLoadS.currentSaveSlot = currentLoadFile;
            myMenu.triggerSecondScreen = true;

            if (!overwrite) {
                SaveLoadS.Load(SaveLoadS.currentSaveSlot);
            }
        }
    }
}
