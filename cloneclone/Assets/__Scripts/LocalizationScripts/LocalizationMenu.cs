using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationMenu : MonoBehaviour
{
    private int currentPosition = 0;
    public RectTransform[] languagePositions;
    public GameObject[] turnOnOnEnd;

    public RectTransform cursorArrow;
    public ControlManagerS myControl;

    public GameObject moveSound;
    public GameObject selectSound;
    public GameObject cancelSound;

    public GameMenuS gameMenu;

    // Update is called once per frame
    bool stickReset = false, selectReset = false, cancelReset = false;
    void Update()
    {
        if (Mathf.Abs(myControl.VerticalMenu()) < 0.1f)
        {
            stickReset = true;
        }

        if (!GetSelectInputs()) {
            selectReset = true;
        }

        if (!GetCancelInputs())
        {
            cancelReset = true;
        }

#if UNITY_SWITCH
         if (myControl.VerticalMenu() > 0.45f && stickReset)
#else
        if (myControl.VerticalMenu() > 0.1f && stickReset)
#endif
        {
            stickReset = false;
            currentPosition--;
            if (currentPosition < 0)
            {
                currentPosition = 0;
            }
            cursorArrow.anchoredPosition = languagePositions[currentPosition].anchoredPosition;
            if (moveSound) {
                Instantiate(moveSound);
            }
        }
#if UNITY_SWITCH
            if (myControl.VerticalMenu() < -0.45f && stickReset)
#else
        if (myControl.VerticalMenu() < -0.1f && stickReset)
#endif
        {
            stickReset = false;
            currentPosition++;
            if (currentPosition > languagePositions.Length - 1)
            {
                currentPosition = languagePositions.Length - 1;
            }
            cursorArrow.anchoredPosition = languagePositions[currentPosition].anchoredPosition;
            if (moveSound)
            {
                Instantiate(moveSound);
            }
        }

        if (GetSelectInputs() && selectReset)
        {
            SelectLanguage();
        }
        else if (GetCancelInputs() && cancelReset && gameMenu) {
            cancelReset = false;
            if (cancelSound) {
                Instantiate(cancelSound);
            }
            TurnOff(LocalizationManager.currentLanguage);
        }
    }

    bool GetSelectInputs() {
        return (myControl.GetCustomInput(1) && !gameMenu) || myControl.GetCustomInput(10) || ((myControl.GetCustomInput(11) || keyboardFailsafe()) && !gameMenu) || myControl.GetCustomInput(3);
    }

    bool GetCancelInputs() {
        return (myControl.GetCustomInput(1) && gameMenu) || (myControl.GetCustomInput(11) && gameMenu);
    }

    bool keyboardFailsafe() {
#if UNITY_SWITCH
        return false;
#endif
        return Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space);
    }

    public void TurnOn() {
        stickReset = false;
        selectReset = false;
        cancelReset = false;
        currentPosition = LocalizationManager.currentLanguage;
        cursorArrow.anchoredPosition = languagePositions[currentPosition].anchoredPosition;
        gameObject.SetActive(true);
    }

    void SelectLanguage() {
        selectReset = false;
        if (selectSound)
        { 
            Instantiate(selectSound);
        }

        TurnOff();
    }

    void RetranslateAllActive() {
        LocalizedText[] allTexts = FindObjectsOfType<LocalizedText>();
        LocalizedTextMesh[] allMeshes = FindObjectsOfType<LocalizedTextMesh>();

        for (int i = 0; i < allTexts.Length; i++) {
            allTexts[i].Translate();
        }

        for (int i = 0; i < allMeshes.Length; i++)
        {
            allMeshes[i].Translate();
        }
    }

    void TurnOff(int overridePosition = -1) {
        if (overridePosition > -1)
        {
            Complete(overridePosition);
        }
        else
        {
            Complete(currentPosition);
        }
        gameObject.SetActive(false);
    }

    public MainMenuNavigationS titleScreenMenu;
    public void Complete(int newLanguage)
    {
        if (newLanguage != LocalizationManager.currentLanguage)
        {
            LocalizationManager.currentLanguage = newLanguage;
            RetranslateAllActive();
        }
        for (int i = 0; i < turnOnOnEnd.Length; i++)
        {
            turnOnOnEnd[i].gameObject.SetActive(true);
        }
        if (titleScreenMenu) {
            titleScreenMenu.ReturnFromLanguageMenu();
        }
        if (gameMenu) {
            gameMenu.ReturnFromLanguageMenu();
        }
    }
}
