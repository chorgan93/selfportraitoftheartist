using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelUpMenu : MonoBehaviour
{

    public RectTransform cursorObj;
    public RectTransform cursorObjLvl;
    public RectTransform cursorObjTravel;
    public RectTransform cursorObjRevert;
    public RectTransform cursorObjDescent;
    private int currentPos = 0;

    private PlayerController pRef;
    private ControlManagerS myControl;

    [Header("Main Menu Selections")]
    public RectTransform[] mainMenuSelectPositions;
    public Text[] mainMenuTextObjs;
    public Color textStartColor;
    public Color textRevertLockedColor;
    private int textStartSize;
    public float textSelectSizeMult = 1.2f;
    public GameObject mainMenuObj;
    private bool onRevertProgressMenu = false;

    [Header("Level Up Menu Selections")]
    public GameObject levelUpSound;
    public GameObject levelDownSound;
    public GameObject levelShuffleSound;
    public Text playerName;
    public Text playerLvl;
    public GameObject levelMenuProper;
    private bool onLevelMenu = false;
    private bool onTravelMenu = false;
    public LevelUpItemS[] levelMenuItems;
    public Image[] levelMenuItemOutlines;
    public RectTransform[] levelMenuPositions;
    //public Text currentLa;

    [Header("Travel Menu Selections")]
    public GameObject travelMenuProper;
    public RectTransform[] travelMenuPositions;
    public Text[] travelMenuChoices;
    public int[] travelMenuSceneNums;
    private bool travelStarted = false;
    private List<int> openTravelChoices = new List<int>();

    [Header("Revert Menu Selections")]
    public GameObject revertMenuProper;
    public RectTransform[] revertMenuPositions;
    public Text[] revertMenuChoiceNames;
    public Text[] revertMenuCorruptionTexts;
    public TrackRevertProgressScriptableObject[] revertMenuDataObjs;
    private int allowRevertProgressNum = 667;
    private bool allowRevertProgress = false;
    private int currentRevertConfirmPos;
    private bool inRevertConfirm = false;
    private bool inExitDescentConfirm = false;
    public GameObject revertMenuConfirm;
    public RectTransform revertMenuConfirmCursor;
    public RectTransform[] revertConfirmPositions;

    public RectTransform[] descentMenuPositions;
    public GameObject descentMenuProper;
    private string descentReturnScene = "theDescent_Intro";

    private float timeBetweenImageOn = 0.1f;
    private bool doingEffect = false;

    private bool _canBeExited = false;
    public bool canBeExited { get { return _canBeExited; } }

    private bool _controlStickMoved = false;
    private bool _selectButtonDown = false;
    private bool _exitButtonDown = false;

    private bool _initialized = false;

    private LevelUpHandlerS levelHandler;
    private bool canTravel = false;
    private bool canShuffle = false;
    private bool canRevert = false;

    [HideInInspector]
    public bool sendExitMessage = false;

    int currentTravelScene = -1;

    public bool lockOnlyTravel = false;

    private bool descentScreen = false;

    // Use this for initialization
    void Start()
    {

        levelMenuProper.gameObject.SetActive(false);
        travelMenuProper.gameObject.SetActive(false);

        playerName.text = TextInputUIS.playerName;

        allowRevertProgress = (StoryProgressionS.storyProgress.Contains(allowRevertProgressNum) || lockOnlyTravel);
        //allowRevertProgress = true; // TODO colin turn off after build!
        if (!allowRevertProgress)
        {
            TurnOffRevertProgressOption();
        }else{
            SetUpRevertProgressScreen();
        }

        if (PlayerInventoryS.I.CheckpointsReached() > 1 && !lockOnlyTravel)
        {
            canTravel = true;
        }
        if (PlayerInventoryS.I.CheckForItem(421))
        {
            canShuffle = true;
        }
        if (PlayerInventoryS.I.CheckForItem(422))
        {
            canRevert = true;
        }
        if (canShuffle && !canRevert)
        {
            levelMenuPositions[levelMenuPositions.Length - 2].anchoredPosition
            = levelMenuPositions[levelMenuPositions.Length - 1].anchoredPosition;
            levelMenuItemOutlines[levelMenuItemOutlines.Length - 2].rectTransform.anchoredPosition
            = levelMenuItemOutlines[levelMenuPositions.Length - 1].rectTransform.anchoredPosition;

        }

    }

    // Update is called once per frame
    void Update()
    {

        if (!onLevelMenu && !onTravelMenu && !onRevertProgressMenu && !inExitDescentConfirm)
        {

            _canBeExited = true;
#if UNITY_SWITCH
            if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.5f ||
                                        Mathf.Abs(myControl.Vertical()) > 0.5f))
#else
            if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.1f ||
                                        Mathf.Abs(myControl.Vertical()) > 0.1f))
#endif
            {
                _controlStickMoved = true;

                mainMenuTextObjs[currentPos].fontSize = textStartSize;
                mainMenuTextObjs[currentPos].color = textStartColor;

#if UNITY_SWITCH
                if (myControl.Horizontal() > 0.5f ||
                    myControl.Vertical() < -0.5f)
#else
                if (myControl.Horizontal() > 0f ||
                    myControl.Vertical() < 0f)
#endif
                {
                    pRef.ResetTimeMax();
                    currentPos++; if (currentPos == 2 && !allowRevertProgress)
                    {
                        currentPos++;
                    }
                    if (currentPos > mainMenuSelectPositions.Length - 1)
                    {
                        currentPos = 0;
                    }
                }
                else
                {
                    currentPos--;
                    pRef.ResetTimeMax();
                    if (currentPos == 2 && !allowRevertProgress)
                    {
                        currentPos--;
                    }
                    if (currentPos < 0)
                    {
                        currentPos = mainMenuSelectPositions.Length - 1;
                    }
                }

                if (currentPos != 1 || (currentPos == 1 && canTravel))
                {
                    mainMenuTextObjs[currentPos].fontSize = Mathf.RoundToInt(textStartSize * textSelectSizeMult);
                    mainMenuTextObjs[currentPos].color = Color.white;
                }
                else
                {
                    mainMenuTextObjs[currentPos].fontSize = textStartSize;
                    mainMenuTextObjs[currentPos].color = textStartColor;
                }
            }

            cursorObj.anchoredPosition = mainMenuSelectPositions[currentPos].anchoredPosition;

            if (!_selectButtonDown && myControl.GetCustomInput(3))
            {
                _selectButtonDown = true;
                if (currentPos == 0)
                {
                    pRef.ResetTimeMax();
                    TurnOnLevelUpMenu();
                }

                else if (currentPos == 1 && canTravel)
                {
                    pRef.ResetTimeMax();
                    TurnOnTravelMenu();
                }
                else if (currentPos == 2 && allowRevertProgress)
                {
                    pRef.ResetTimeMax();
                    TurnOnRevertMenu();
                }

                else if (currentPos == 3)
                {
                    pRef.ResetTimeMax();
                    TurnOff();
                    sendExitMessage = true;
                }
            }

        }

        if (onLevelMenu)
        {
#if UNITY_SWITCH
            if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.5f ||
                                        Mathf.Abs(myControl.Vertical()) > 0.5f))
#else
            if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.1f ||
                                        Mathf.Abs(myControl.Vertical()) > 0.1f))
#endif
            {
                _controlStickMoved = true;

                levelMenuItemOutlines[currentPos].color = textStartColor;

#if UNITY_SWITCH
                if (myControl.Horizontal() > 0.5f ||
                    myControl.Vertical() < -0.5f)
#else
                if (myControl.Horizontal() > 0f ||
                    myControl.Vertical() < 0f)
#endif
                {
                    pRef.ResetTimeMax();
                    currentPos++;
                    if (currentPos == levelMenuItems.Length - 2 && !PlayerInventoryS.I.CheckForItem(421))
                    {
                        currentPos++;
                    }
                    if (currentPos == levelMenuItems.Length - 1 && !PlayerInventoryS.I.CheckForItem(422))
                    {
                        currentPos++;
                    }
                    if (currentPos > levelMenuItems.Length - 1)
                    {
                        currentPos = 0;
                    }
                }
                else
                {
                    currentPos--;
                    if (currentPos == levelMenuItems.Length - 1 && !PlayerInventoryS.I.CheckForItem(422))
                    {
                        currentPos--;
                    }
                    if (currentPos == levelMenuItems.Length - 2 && !PlayerInventoryS.I.CheckForItem(421))
                    {
                        currentPos--;
                    }
                    pRef.ResetTimeMax();
                    if (currentPos < 0)
                    {
                        currentPos = levelMenuItems.Length - 1;
                        if (currentPos == levelMenuItems.Length - 1 && !PlayerInventoryS.I.CheckForItem(422))
                        {
                            currentPos--;
                        }
                        if (currentPos == levelMenuItems.Length - 2 && !PlayerInventoryS.I.CheckForItem(421))
                        {
                            currentPos--;
                        }
                    }
                }

                levelMenuItemOutlines[currentPos].color = Color.white;
                levelMenuItems[currentPos].ShowText();
            }

            cursorObjLvl.anchoredPosition = levelMenuPositions[currentPos].anchoredPosition;

            if (!_selectButtonDown && myControl.GetCustomInput(3))
            {
                _selectButtonDown = true;
                pRef.ResetTimeMax();
                if (levelMenuItems[currentPos].CanBeUpgraded())
                {
                    if (levelMenuItems[currentPos].IsRevertUpgrade)
                    {
                        pRef.myStats.AddStat(PlayerInventoryS.I.GetNextRevertLevelType(), true);
                    }
                    else
                    {
                        pRef.myStats.AddStat(levelMenuItems[currentPos].upgradeID);
                    }
                    levelMenuItems[currentPos].BuyUpgrade(currentPos, levelHandler);
                    if (levelMenuItems[currentPos].IsRevertUpgrade && levelDownSound != null)
                    {

                        Instantiate(levelDownSound);
                    }
                    else if (levelMenuItems[currentPos].IsShuffleUpgrade && levelShuffleSound != null)
                    {
                        Instantiate(levelShuffleSound);
                    }
                    else if (levelUpSound)
                    {
                        Instantiate(levelUpSound);
                    }
                    if (currentPos <= 3)
                    {
                        currentPos = 0;
                    }

                    UpdateAvailableLevelUps();

                }
            }

            if (!_exitButtonDown && myControl.GetCustomInput(1) && !doingEffect)
            {
                TurnOffLevelUpMenu();
            }
        }

        if (onTravelMenu)
        {
#if UNITY_SWITCH
            if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.5f ||
                                        Mathf.Abs(myControl.Vertical()) > 0.5f) && !travelStarted)
#else
            if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.1f ||
                                        Mathf.Abs(myControl.Vertical()) > 0.1f) && !travelStarted)
#endif
            {
                _controlStickMoved = true;

                pRef.ResetTimeMax();
                travelMenuChoices[currentPos].color = textStartColor;

#if UNITY_SWITCH
                if (myControl.Horizontal() > 0.5f ||
                    myControl.Vertical() < -0.5f)
#else
                if (myControl.Horizontal() > 0f ||
                    myControl.Vertical() < 0f)
#endif
                {
                    pRef.ResetTimeMax();
                    AdvanceTravelPos(1);
                }
                else
                {
                    pRef.ResetTimeMax();
                    AdvanceTravelPos(-1);
                }

                if (travelMenuSceneNums[currentPos] != currentTravelScene)
                {
                    travelMenuChoices[currentPos].color = Color.white;
                }
                else
                {
                    travelMenuChoices[currentPos].color = textStartColor;
                }
            }

            cursorObjTravel.anchoredPosition = travelMenuPositions[currentPos].anchoredPosition;

            if (!_selectButtonDown && myControl.GetCustomInput(3))
            {
                _selectButtonDown = true;
                pRef.ResetTimeMax();

                // load level if we're not already there
                if (travelMenuSceneNums[currentPos] != Application.loadedLevel && !travelStarted)
                {

                    travelMenuProper.gameObject.SetActive(false);
                    travelStarted = true;
                    _canBeExited = false;

                    int nextSceneIndex = travelMenuSceneNums[currentPos];
                    int nextSceneSpawn = PlayerInventoryS.I.ReturnCheckpointSpawnAtScene(travelMenuSceneNums[currentPos]);

                    List<int> saveBuddyList = new List<int>();
                    saveBuddyList.Add(pRef.ParadigmIBuddy().buddyNum);
                    if (pRef.ParadigmIIBuddy() != null)
                    {
                        saveBuddyList.Add(pRef.ParadigmIIBuddy().buddyNum);
                    }
                    if (!pRef.isNatalie)
                    {
                        PlayerInventoryS.I.SaveLoadout(pRef.equippedWeapons, pRef.subWeapons, saveBuddyList);
                    }

                    CameraEffectsS.E.SetNextScene(nextSceneIndex);
                    CameraEffectsS.E.FadeIn();

                    VerseDisplayS.V.EndVerse();

                    SpawnPosManager.whereToSpawn = nextSceneSpawn;

                    pRef.TriggerResting();
                    PlayerController.doWakeUp = true;
                    SpawnPosManager.spawningFromTeleport = true;

                }
            }
            if (!_exitButtonDown && myControl.GetCustomInput(1) && !travelStarted && !doingEffect)
            {
                _exitButtonDown = true;
                TurnOffTravelMenu();
                pRef.ResetTimeMax();
            }
        }

        if (inExitDescentConfirm)
        {
#if UNITY_SWITCH
            if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.5f ||
                                            Mathf.Abs(myControl.Vertical()) > 0.5f) && !travelStarted)
#else
            if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.1f ||
                                            Mathf.Abs(myControl.Vertical()) > 0.1f) && !travelStarted)
#endif
            {
                _controlStickMoved = true;

                pRef.ResetTimeMax();

#if UNITY_SWITCH
                if (myControl.Horizontal() > 0.5f ||
                    myControl.Vertical() < -0.5f)
#else
                if (myControl.Horizontal() > 0f ||
                    myControl.Vertical() < 0f)
#endif
                {
                    pRef.ResetTimeMax();
                    currentRevertConfirmPos++;
                    if (currentRevertConfirmPos > 1)
                    {
                        currentRevertConfirmPos = 1;
                    }
                }
                else
                {
                    pRef.ResetTimeMax();
                    currentRevertConfirmPos--;
                    if (currentRevertConfirmPos < 0)
                    {
                        currentRevertConfirmPos = 0;
                    }
                }
            }
            cursorObjDescent.anchoredPosition = descentMenuPositions[currentRevertConfirmPos].anchoredPosition;
            if (!_selectButtonDown && myControl.GetCustomInput(3))
            {
                _selectButtonDown = true;
                pRef.ResetTimeMax();

                // if yes, end descent
                if (!travelStarted && currentRevertConfirmPos == 1)
                {

                    revertMenuProper.gameObject.SetActive(false);
                    travelStarted = true;
                    _canBeExited = false;
                    descentMenuProper.gameObject.SetActive(false);
                    string nextSceneIndex = descentReturnScene;
                    int nextSceneSpawn = 0;

                    // the below is to prevent "return to last checkpoint" progression breaks
                    GameOverS.reviveScene = nextSceneIndex;
                    GameOverS.revivePosition = 0;
                    DarknessPercentUIS.DPERCENT.ActivateDescentReset(true);

                    List<int> saveBuddyList = new List<int>();
                    saveBuddyList.Add(pRef.ParadigmIBuddy().buddyNum);
                    if (pRef.ParadigmIIBuddy() != null)
                    {
                        saveBuddyList.Add(pRef.ParadigmIIBuddy().buddyNum);
                    }
                    if (!pRef.isNatalie)
                    {
                        PlayerInventoryS.I.SaveLoadout(pRef.equippedWeapons, pRef.subWeapons, saveBuddyList);
                    }

                    CameraEffectsS.E.SetNextScene(nextSceneIndex);
                    CameraEffectsS.E.FadeIn();

                    VerseDisplayS.V.EndVerse();

                    SpawnPosManager.whereToSpawn = nextSceneSpawn;

                    pRef.TriggerResting();
                    PlayerController.doWakeUp = true;
                    SpawnPosManager.spawningFromTeleport = true;


                }
                else if (!travelStarted && currentRevertConfirmPos == 0)
                {
                    // go back to previous menu
                    currentRevertConfirmPos = 0;
                    inExitDescentConfirm = false;
                    TurnOffRevertMenu(false);
                }
            }
            if (!_exitButtonDown && myControl.GetCustomInput(1) && !travelStarted)
            {
                if (currentRevertConfirmPos == 1)
                {
                    currentRevertConfirmPos = 0;
                }
                else
                {
                    inExitDescentConfirm = false;
                    descentMenuProper.SetActive(false);
                    TurnOffRevertMenu(false);
                }
                _exitButtonDown = true;
                pRef.ResetTimeMax();
            }
        }

        if (onRevertProgressMenu)
        {
            if (inRevertConfirm)
            {
#if UNITY_SWITCH
                if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.5f ||
                                            Mathf.Abs(myControl.Vertical()) > 0.5f) && !travelStarted)
#else
                if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.1f ||
                                            Mathf.Abs(myControl.Vertical()) > 0.1f) && !travelStarted)
#endif
                {
                    _controlStickMoved = true;

                    pRef.ResetTimeMax();

#if UNITY_SWITCH
                    if (myControl.Horizontal() > 0.5f ||
                        myControl.Vertical() < -0.5f)
#else
                    if (myControl.Horizontal() > 0f ||
                        myControl.Vertical() < 0f)
#endif
                    {
                        pRef.ResetTimeMax();
                        currentRevertConfirmPos++;
                        if (currentRevertConfirmPos > 1)
                        {
                            currentRevertConfirmPos = 1;
                        }
                    }
                    else
                    {
                        pRef.ResetTimeMax();
                        currentRevertConfirmPos--;
                        if (currentRevertConfirmPos < 0)
                        {
                            currentRevertConfirmPos = 0;
                        }
                    }
                }

                revertMenuConfirmCursor.anchoredPosition = revertConfirmPositions[currentRevertConfirmPos].anchoredPosition;

                if (!_selectButtonDown && myControl.GetCustomInput(3))
                {
                    _selectButtonDown = true;
                    pRef.ResetTimeMax();

                    // if yes, revert to selected chapter
                    if (!travelStarted && currentRevertConfirmPos == 1)
                    {

                        revertMenuProper.gameObject.SetActive(false);
                        travelStarted = true;
                        _canBeExited = false;

                        RevertToPreviousTrack(currentPos);

                        string nextSceneIndex = revertMenuDataObjs[currentPos].revertSceneName;
                        int nextSceneSpawn = revertMenuDataObjs[currentPos].revertSceneNameLoadNum;

                        // the below is to prevent "return to last checkpoint" progression breaks
                        GameOverS.reviveScene = nextSceneIndex;
                        GameOverS.revivePosition = 0;

                        List<int> saveBuddyList = new List<int>();
                        saveBuddyList.Add(pRef.ParadigmIBuddy().buddyNum);
                        if (pRef.ParadigmIIBuddy() != null)
                        {
                            saveBuddyList.Add(pRef.ParadigmIIBuddy().buddyNum);
                        }
                        if (!pRef.isNatalie)
                        {
                            PlayerInventoryS.I.SaveLoadout(pRef.equippedWeapons, pRef.subWeapons, saveBuddyList);
                        }

                        CameraEffectsS.E.SetNextScene(nextSceneIndex);
                        CameraEffectsS.E.FadeIn();

                        VerseDisplayS.V.EndVerse();

                        SpawnPosManager.whereToSpawn = nextSceneSpawn;

                        pRef.TriggerResting();
                        PlayerController.doWakeUp = true;
                        SpawnPosManager.spawningFromTeleport = true;

                        if (currentPos > 0 && currentPos < 9)
                        {
                            pRef.myStats.DeathCountUp(false, true);
                            PlayerStatsS._currentDarkness = PlayerInventoryS.I.revertDarknessNums[currentPos];
                        }

                    }else if (!travelStarted && currentRevertConfirmPos == 0){
                        // go back to previous menu
                        currentRevertConfirmPos = 0;
                        inRevertConfirm = false;
                        revertMenuConfirm.SetActive(false);
                    }
                }
                if (!_exitButtonDown && myControl.GetCustomInput(1) && !travelStarted)
                {
                    if (currentRevertConfirmPos == 1)
                    {
                        currentRevertConfirmPos = 0;
                    }else
                    {
                        inRevertConfirm = false;
                        revertMenuConfirm.SetActive(false);
                    }
                    _exitButtonDown = true;
                    pRef.ResetTimeMax();
                }
            }
            else
            {
#if UNITY_SWITCH
                if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.5f ||
                                            Mathf.Abs(myControl.Vertical()) > 0.5f) && !travelStarted)
#else
                if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.1f ||
                                            Mathf.Abs(myControl.Vertical()) > 0.1f) && !travelStarted)
#endif
                {
                    _controlStickMoved = true;

                    pRef.ResetTimeMax();
                    if (currentPos < PlayerInventoryS.I.revertDarknessNums.Count)
                    {
                        revertMenuChoiceNames[currentPos].color = revertMenuCorruptionTexts[currentPos].color = textStartColor;
                    }
                    else
                    {

                        revertMenuChoiceNames[currentPos].color = revertMenuCorruptionTexts[currentPos].color = textRevertLockedColor;
                    }
#if UNITY_SWITCH
                    if (myControl.Horizontal() > 0.5f ||
                        myControl.Vertical() < -0.5f)
#else
                    if (myControl.Horizontal() > 0f ||
                        myControl.Vertical() < 0f)
#endif
                    {
                        pRef.ResetTimeMax();
                        currentPos++;
                        if (currentPos > 9)
                        {
                            currentPos = 9;
                        }
                        if (currentPos == 9 && PlayerInventoryS.I.revertDarknessNums.Count < 9)
                        {
                            currentPos = 8;
                        }
                    }
                    else
                    {
                        pRef.ResetTimeMax();
                        currentPos--;
                        if (currentPos < 0)
                        {
                            currentPos = 0;
                        }
                    }

                    if (currentPos < PlayerInventoryS.I.revertDarknessNums.Count)
                    {
                        revertMenuChoiceNames[currentPos].color = revertMenuCorruptionTexts[currentPos].color = Color.white;
                    }
                    else
                    {
                        revertMenuChoiceNames[currentPos].color = revertMenuCorruptionTexts[currentPos].color = textRevertLockedColor;
                    }
                }

                cursorObjRevert.anchoredPosition = revertMenuPositions[currentPos].anchoredPosition;

                if (!_selectButtonDown && myControl.GetCustomInput(3))
                {
                    _selectButtonDown = true;
                    pRef.ResetTimeMax();

                    // turn on confirm menu
                    if (!travelStarted && currentPos < PlayerInventoryS.I.revertDarknessNums.Count)
                    {

                        currentRevertConfirmPos = 0;
                        inRevertConfirm = true;
                        revertMenuConfirmCursor.anchoredPosition = revertConfirmPositions[currentRevertConfirmPos].anchoredPosition;
                        revertMenuConfirm.gameObject.SetActive(true);


                    }
                }
                if (!_exitButtonDown && myControl.GetCustomInput(1) && !travelStarted)
                {
                    TurnOffRevertMenu();
                    pRef.ResetTimeMax();
                }
            }
        }

        if (!myControl.GetCustomInput(1))
        {
            _exitButtonDown = false;
        }
        if (!myControl.GetCustomInput(3))
        {
            _selectButtonDown = false;
        }
        if (Mathf.Abs(myControl.Horizontal()) < 0.1f && Mathf.Abs(myControl.Vertical()) < 0.1f)
        {
            _controlStickMoved = false;
        }

    }

    private void TurnOnLevelUpMenu()
    {
        if (pRef.myStats.currentLevel < 10)
        {
            playerLvl.text = "LV. 0" + pRef.myStats.currentLevel;
        }
        else
        {
            playerLvl.text = "LV. " + pRef.myStats.currentLevel;
        }
        pRef.myStats.uiReference.cDisplay.SetShowing(true);
        cursorObj.gameObject.SetActive(false);
        levelMenuProper.gameObject.SetActive(true);
        if (canShuffle)
        {
            levelMenuItems[levelMenuItems.Length - 2].gameObject.SetActive(true);
            levelMenuItemOutlines[levelMenuItemOutlines.Length - 2].gameObject.SetActive(true);
        }
        else
        {
            levelMenuItems[levelMenuItems.Length - 2].gameObject.SetActive(false);
            levelMenuItemOutlines[levelMenuItemOutlines.Length - 2].gameObject.SetActive(false);
        }
        if (canRevert)
        {
            levelMenuItems[levelMenuItems.Length - 1].gameObject.SetActive(true);
            levelMenuItemOutlines[levelMenuItemOutlines.Length - 1].gameObject.SetActive(true);
        }
        else
        {
            levelMenuItems[levelMenuItems.Length - 1].gameObject.SetActive(false);
            levelMenuItemOutlines[levelMenuItemOutlines.Length - 1].gameObject.SetActive(false);
        }
        mainMenuObj.SetActive(false);
        _canBeExited = false;
        currentPos = 0;
        //currentLa.text = PlayerCollectionS.currencyCollected.ToString() + " la";
        onLevelMenu = true;
        CameraFollowS.F.SetZoomIn(true);
        _controlStickMoved = true;
        UpdateAvailableLevelUps();
        pRef.TriggerResting();
    }

    private void TurnOnTravelMenu()
    {
        //pRef.myStats.uiReference.cDisplay.SetShowing (true);
        cursorObj.gameObject.SetActive(false);
        StartCoroutine(TurnOnTravelNames());
        travelMenuProper.gameObject.SetActive(true);
        mainMenuObj.SetActive(false);
        _canBeExited = false;
        currentTravelScene = Application.loadedLevel;
        for (int i = 0; i < travelMenuSceneNums.Length; i++)
        {
            if (travelMenuSceneNums[i] == currentTravelScene)
            {
                currentPos = i;
            }
        }
        onTravelMenu = true;
        //CameraFollowS.F.SetZoomIn(true);
        _controlStickMoved = true;

        travelMenuChoices[currentPos].color = textStartColor;
        //UpdateAvailableLevelUps();
        //pRef.TriggerResting();
    }

    private void UpdateAvailableLevelUps(bool doEffect = true)
    {
        for (int i = 0; i < levelMenuItems.Length; i++)
        {
            if (i < 4)
            {
                    levelMenuItems[i].Initialize(levelHandler.nextLevelUps[i], pRef.myStats.uiReference.cDisplay);

            }
            else if (i == 4)
            {
                levelMenuItems[i].Initialize(null, pRef.myStats.uiReference.cDisplay, true);
            }
            else
            {
                levelMenuItems[i].Initialize(null, pRef.myStats.uiReference.cDisplay, false, true);
            }
        }
        levelMenuItems[currentPos].ShowText();
        if (doEffect)
        {
            UpdateUpgradeEffect();
        }

        //currentLa.text = PlayerCollectionS.currencyCollected.ToString() + " la";
        StoryProgressionS.SaveProgress();
    }

    private void TurnOffLevelUpMenu()
    {
        pRef.myStats.uiReference.cDisplay.SetShowing(false);
        cursorObj.gameObject.SetActive(true);
        levelMenuProper.gameObject.SetActive(false);
        mainMenuObj.SetActive(true);
        _canBeExited = true;
        currentPos = 0;
        onLevelMenu = false;
        CameraFollowS.F.SetZoomIn(false);
        pRef.TurnOffResting();
    }

    private void TurnOffTravelMenu()
    {
        //pRef.myStats.uiReference.cDisplay.SetShowing (false);
        cursorObj.gameObject.SetActive(true);
        travelMenuProper.gameObject.SetActive(false);
        mainMenuObj.SetActive(true);
        _canBeExited = true;
        currentPos = 1;
        onTravelMenu = false;
        //CameraFollowS.F.SetZoomIn(false);
        //pRef.TurnOffResting();
    }

    public void TurnOn()
    {

        if (!_initialized)
        {

            pRef = GetComponentInParent<InGameMenuManagerS>().pRef;
            myControl = pRef.myControl;
            //textStartColor = mainMenuTextObjs[0].color;
            textStartSize = mainMenuTextObjs[0].fontSize;
            _initialized = true;
            levelHandler = PlayerInventoryS.I.GetComponent<LevelUpHandlerS>();
            levelHandler.LevelMenuInitialize();
        }

        gameObject.SetActive(true);
        mainMenuObj.SetActive(true);
        _canBeExited = true;
        onLevelMenu = false;
        onTravelMenu = false;
        currentPos = 0;

        _selectButtonDown = true;
        _exitButtonDown = true;
        _controlStickMoved = true;

        cursorObj.gameObject.SetActive(true);
        cursorObj.anchoredPosition = mainMenuSelectPositions[currentPos].anchoredPosition;
        mainMenuTextObjs[currentPos].fontSize = Mathf.RoundToInt(textStartSize * textSelectSizeMult);
        mainMenuTextObjs[currentPos].color = Color.white;

        foreach (Image i in levelMenuItemOutlines)
        {
            i.color = textStartColor;
        }
    }

    public void TurnOff()
    {
        foreach (Text t in mainMenuTextObjs)
        {
            t.fontSize = textStartSize;
            t.color = textStartColor;
        }
        currentPos = 0;
        mainMenuTextObjs[0].color = Color.white;
        mainMenuTextObjs[0].fontSize = Mathf.RoundToInt(textStartSize * textSelectSizeMult);

        mainMenuTextObjs[1].color = mainMenuTextObjs[2].color = textStartColor;
        mainMenuTextObjs[1].fontSize = mainMenuTextObjs[2].fontSize = textStartSize;
        gameObject.SetActive(false);
        levelMenuProper.gameObject.SetActive(false);
        travelMenuProper.gameObject.SetActive(false);
        TurnOffRevertMenu(true);
    }

    private void UpdateUpgradeEffect()
    {
        int index = 0;
        foreach (LevelUpItemS l in levelMenuItems)
        {

            if (index < 4)
            {
                l.TurnOffVisual();
                levelMenuItemOutlines[index].enabled = false;
            }
            index++;
        }
        if (pRef.myStats.currentLevel > LevelUpItemS.MAX_LEVEL_UP)
        {
            playerLvl.text = "LV. MAX";
        }
        else if (pRef.myStats.currentLevel < 10)
        {
            playerLvl.text = "LV. 0" + pRef.myStats.currentLevel;
        }
        else
        {
            playerLvl.text = "LV. " + pRef.myStats.currentLevel;
        }
        StartCoroutine(TurnOnUpgrades());
    }

    private IEnumerator TurnOnUpgrades()
    {
        doingEffect = true;

        yield return new WaitForSeconds(timeBetweenImageOn);

        int index = 0;
        while (index < levelMenuItems.Length)
        {

            levelMenuItems[index].TurnOnVisual();
            levelMenuItemOutlines[index].enabled = true;
            index++;

            yield return new WaitForSeconds(timeBetweenImageOn);

        }
        doingEffect = false;
    }

    private IEnumerator TurnOnTravelNames()
    {
        doingEffect = true;
        openTravelChoices.Clear();
        for (int i = 0; i < travelMenuChoices.Length; i++)
        {
            travelMenuChoices[i].enabled = false;
            travelMenuChoices[i].color = textStartColor;
        }

        yield return new WaitForSeconds(timeBetweenImageOn);

        for (int i = 0; i < travelMenuChoices.Length; i++)
        {

            if (PlayerInventoryS.I.HasReachedScene(travelMenuSceneNums[i]))
            {
                travelMenuChoices[i].color = textStartColor;
                travelMenuChoices[i].enabled = true;
                openTravelChoices.Add(i);
                yield return new WaitForSeconds(timeBetweenImageOn);
            }

        }
        doingEffect = false;

    }

    void AdvanceTravelPos(int direction)
    {
        int nextPos = currentPos;
        if (direction > 0)
        {
            for (int i = 0; i < openTravelChoices.Count; i++)
            {
                if (openTravelChoices[i] > currentPos &&
                    ((currentPos == nextPos) || (currentPos != nextPos && openTravelChoices[i] < nextPos)))
                {
                    nextPos = openTravelChoices[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < openTravelChoices.Count; i++)
            {
                if (openTravelChoices[i] < currentPos &&
                    ((currentPos == nextPos) || (currentPos != nextPos && openTravelChoices[i] > nextPos)))
                {
                    nextPos = openTravelChoices[i];
                }
            }
        }
        currentPos = nextPos;
    }

    void TurnOffRevertProgressOption()
    {
        mainMenuTextObjs[2].gameObject.SetActive(false);
        mainMenuSelectPositions[3].anchoredPosition = mainMenuSelectPositions[2].anchoredPosition;
        Vector2 repositionDown = Vector2.zero;
        for (int i = 0; i < mainMenuTextObjs.Length; i++)
        {
            repositionDown.y = 5;
            mainMenuSelectPositions[i].anchoredPosition -= repositionDown;
        }
    }

    void SetUpRevertProgressScreen(){
        
        for (int i = 0; i < revertMenuChoiceNames.Length; i++){
            if (i == 9 && PlayerInventoryS.I.revertDarknessNums.Count < 9){
                revertMenuChoiceNames[i].enabled = false;
                revertMenuCorruptionTexts[i].enabled = false;
            }
            if (i == 0)
            {
                revertMenuChoiceNames[i].text = "Track -1 : " + revertMenuDataObjs[i].chapterName;
            }
            else{
                revertMenuChoiceNames[i].text = "Track  " + i + " : " + revertMenuDataObjs[i].chapterName;
            }
            if (i < 9 && i < PlayerInventoryS.I.revertDarknessNums.Count){


                if (i > 0)
                {
                    revertMenuCorruptionTexts[i].text = "Corruption : " + PlayerInventoryS.I.revertDarknessNums[i].ToString("F2") + "%";
                    revertMenuChoiceNames[i].color = revertMenuCorruptionTexts[i].color = textStartColor;
                }else{
                    revertMenuCorruptionTexts[i].text = "Corruption : ---";
                    revertMenuChoiceNames[i].color = revertMenuCorruptionTexts[i].color = Color.white;
                }
            }else{

                revertMenuCorruptionTexts[i].text = "Corruption : ????";
                revertMenuChoiceNames[i].color = revertMenuCorruptionTexts[i].color = textRevertLockedColor;
            }

        }
    }

    void SetUpDescentQuitScreen(){
        descentScreen = true;
        mainMenuTextObjs[2].text = "End Descent";
    }

    void TurnOnRevertMenu()
    {
        if (descentScreen)
        {
            inExitDescentConfirm = true;
            currentPos = 0;
            currentRevertConfirmPos = 0;
            _canBeExited = false;
            mainMenuObj.SetActive(false);
            cursorObj.gameObject.SetActive(false);
            _controlStickMoved = true;
            cursorObjDescent.anchoredPosition = descentMenuPositions[currentPos].anchoredPosition;
            descentMenuProper.gameObject.SetActive(true);
            Debug.Log("In descent screen!");
        }
        else
        {
            inRevertConfirm = false;
            cursorObj.gameObject.SetActive(false);
            revertMenuProper.gameObject.SetActive(true);
            mainMenuObj.SetActive(false);
            _canBeExited = false;
            currentPos = 0;
            cursorObjRevert.anchoredPosition = revertMenuPositions[currentPos].anchoredPosition;
            onRevertProgressMenu = true;
            _controlStickMoved = true;

            revertMenuChoiceNames[currentPos].color = revertMenuCorruptionTexts[currentPos].color = textStartColor;
        }
    }
    void TurnOffRevertMenu(bool fullExit = false)
    {
        if (!fullExit){
            cursorObj.gameObject.SetActive(true);
        mainMenuObj.SetActive(true);
        _canBeExited = true;
        currentPos = 2;
            onRevertProgressMenu = false;
    }
        descentMenuProper.SetActive(false);
        inRevertConfirm = false;
        inExitDescentConfirm = false;
        revertMenuProper.gameObject.SetActive(false);
    }

    public void RevertToPreviousTrack(int trackToReturnTo, bool fadeMusic = true){

        // first, resave any currently reverted progress
        PlayerInventoryS.I.MergeRevertedData();
        
            // overwrite everything, then load new scene

        CheckpointS.lastSavePointName = revertMenuDataObjs[trackToReturnTo].lastCheckpointName;
        ChapterTrigger.lastChapterTriggered = revertMenuDataObjs[trackToReturnTo].chapterName;

        PlayerController.killedFamiliar = false;

        if (fadeMusic && BGMHolderS.BG != null){
            BGMHolderS.BG.EndAllLayers(false,true);
        }

        // roll back data to selected chapter
        PlayerInventoryS.I.OverwriteReversionData();
        for (int i = revertMenuDataObjs.Length - 1; i >= trackToReturnTo; i--){

            PlayerInventoryS.I.RevertDataForChapter(revertMenuDataObjs[i]);
        }

        PlayerInventoryS.I.dManager.ClearAll();

    }

    public void LockTravel(bool midDescent = false){
        lockOnlyTravel = true;
        canTravel = false;
        allowRevertProgress = true;
        if (!midDescent)
        {
            SetUpRevertProgressScreen();
        }else{
            SetUpDescentQuitScreen();

            // also turn off player ambient darkness
            GameObject.Find("Player").GetComponent<PlayerStatsS>().TurnOffAmbientDarknessGain();
        }
    }
}
