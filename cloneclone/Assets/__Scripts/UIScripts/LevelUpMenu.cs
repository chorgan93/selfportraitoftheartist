using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelUpMenu : MonoBehaviour {

	public RectTransform cursorObj;
	public RectTransform cursorObjLvl;
	public RectTransform cursorObjTravel;
	private int currentPos = 0;

	private PlayerController pRef;
	private ControlManagerS myControl;

	[Header("Main Menu Selections")]
	public RectTransform[] mainMenuSelectPositions;
	public Text[] mainMenuTextObjs;
	public Color textStartColor;
	private int textStartSize;
	public float textSelectSizeMult = 1.2f;
	public GameObject mainMenuObj;

	[Header("Level Up Menu Selections")]
	public Text playerName;
	public Text playerLvl;
	public GameObject levelMenuProper;
	private bool onLevelMenu = false;
	private bool onTravelMenu = false;
	public LevelUpItemS[] levelMenuItems;
	public Image[] levelMenuItemOutlines;
	public RectTransform[] levelMenuPositions;
	
	[Header("Travel Menu Selections")]
	public GameObject travelMenuProper;
	public RectTransform[] travelMenuPositions;
	public Text[] travelMenuChoices;
	public int[] travelMenuSceneNums;
	private bool travelStarted = false;
	private List<int> openTravelChoices = new List<int>();

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

	[HideInInspector]
	public bool sendExitMessage = false;

	int currentTravelScene = -1;

	// Use this for initialization
	void Start () {
	
		levelMenuProper.gameObject.SetActive(false);
		travelMenuProper.gameObject.SetActive(false);

		playerName.text = TextInputUIS.playerName;

		if (PlayerInventoryS.I.CheckpointsReached() > 1){
			canTravel = true;
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (!onLevelMenu && !onTravelMenu){

			_canBeExited = true;

			if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.1f ||
			                            Mathf.Abs(myControl.Vertical()) > 0.1f)){
				_controlStickMoved = true;

				mainMenuTextObjs[currentPos].fontSize = textStartSize;
				mainMenuTextObjs[currentPos].color = textStartColor;

				if (myControl.Horizontal() > 0f ||
				    myControl.Vertical() < 0f){
					currentPos++;
					if (currentPos > mainMenuSelectPositions.Length-1){
						currentPos = 0;
					}
				}else{
					currentPos--;
					if (currentPos < 0){
						currentPos = mainMenuSelectPositions.Length-1;
					}
				}

				if (currentPos != 1 || (currentPos == 1 && canTravel)){
					mainMenuTextObjs[currentPos].fontSize = Mathf.RoundToInt(textStartSize*textSelectSizeMult);
					mainMenuTextObjs[currentPos].color = Color.white;
				}else{
					mainMenuTextObjs[currentPos].fontSize = textStartSize;
					mainMenuTextObjs[currentPos].color = textStartColor;
				}
			}

			cursorObj.anchoredPosition = mainMenuSelectPositions[currentPos].anchoredPosition;

			if (!_selectButtonDown && myControl.MenuSelectButton()){
				_selectButtonDown = true;
				if (currentPos == 0){
					TurnOnLevelUpMenu();
				}
				
				else if (currentPos == 1 && canTravel){
					TurnOnTravelMenu();
				}

				else if (currentPos == 2){
					TurnOff();
					sendExitMessage = true;
				}
			}

		}

		if (onLevelMenu){

			if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.1f ||
			                            Mathf.Abs(myControl.Vertical()) > 0.1f)){
				_controlStickMoved = true;

				levelMenuItemOutlines[currentPos].color = textStartColor;
				
				if (myControl.Horizontal() > 0f ||
				    myControl.Vertical() < 0f){
					currentPos++;
					if (currentPos > levelMenuItems.Length-1){
						currentPos = 0;
					}
				}else{
					currentPos--;
					if (currentPos < 0){
						currentPos = levelMenuItems.Length-1;
					}
				}

				levelMenuItemOutlines[currentPos].color = Color.white;
				levelMenuItems[currentPos].ShowText();
			}

			cursorObjLvl.anchoredPosition = levelMenuPositions[currentPos].anchoredPosition;

			if (!_selectButtonDown && myControl.MenuSelectButton()){
				_selectButtonDown = true;
				if (levelMenuItems[currentPos].CanBeUpgraded()){
					pRef.myStats.AddStat(levelMenuItems[currentPos].upgradeID);
					levelMenuItems[currentPos].BuyUpgrade(currentPos, levelHandler);
					currentPos = 0;
					UpdateAvailableLevelUps();
				}
			}

			if (!_exitButtonDown && myControl.ExitButton() && !doingEffect){
				TurnOffLevelUpMenu();
			}
		}

		if (onTravelMenu){

			if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.1f ||
			                            Mathf.Abs(myControl.Vertical()) > 0.1f) && !travelStarted){
				_controlStickMoved = true;
				
				travelMenuChoices[currentPos].color = textStartColor;
				
				if (myControl.Horizontal() > 0f ||
				    myControl.Vertical() < 0f){
					AdvanceTravelPos(1);
				}else{
					AdvanceTravelPos(-1);
				}

				if (travelMenuSceneNums[currentPos] != currentTravelScene){
				travelMenuChoices[currentPos].color = Color.white;
				}else{
					travelMenuChoices[currentPos].color = textStartColor;
				}
			}
			
			cursorObjTravel.anchoredPosition = travelMenuPositions[currentPos].anchoredPosition;

			if (!_selectButtonDown && myControl.MenuSelectButton()){
				_selectButtonDown = true;

				// load level if we're not already there
				if (travelMenuSceneNums[currentPos] != Application.loadedLevel && !travelStarted){

					travelMenuProper.gameObject.SetActive(false);
					travelStarted = true;
					_canBeExited = false;

					int nextSceneIndex = travelMenuSceneNums[currentPos];
					int nextSceneSpawn = PlayerInventoryS.I.ReturnCheckpointSpawnAtScene(travelMenuSceneNums[currentPos]);

					List<int> saveBuddyList = new List<int>();
					saveBuddyList.Add(pRef.ParadigmIBuddy().buddyNum);
					if (pRef.ParadigmIIBuddy() != null){
						saveBuddyList.Add(pRef.ParadigmIIBuddy().buddyNum);
					}
					PlayerInventoryS.I.SaveLoadout(pRef.equippedWeapons, pRef.subWeapons, saveBuddyList);
					
					CameraEffectsS.E.SetNextScene(nextSceneIndex);
					CameraEffectsS.E.FadeIn();
					
					VerseDisplayS.V.EndVerse();
					
					SpawnPosManager.whereToSpawn = nextSceneSpawn;

					pRef.TriggerResting();
					PlayerController.doWakeUp = true;
					SpawnPosManager.spawningFromTeleport = true;

				}
			}
			if (!_exitButtonDown && myControl.ExitButton() && !travelStarted && !doingEffect){
				TurnOffTravelMenu();
			}
		}

		if (myControl.ExitButtonUp()){
			_exitButtonDown = false;
		}
		if (myControl.MenuSelectUp()){
			_selectButtonDown = false;
		}
		if (Mathf.Abs(myControl.Horizontal()) < 0.1f && Mathf.Abs(myControl.Vertical()) < 0.1f){
			_controlStickMoved = false;
		}
	
	}

	private void TurnOnLevelUpMenu(){
		if (pRef.myStats.currentLevel < 10){
			playerLvl.text = "LV. 0" + pRef.myStats.currentLevel;
		}else{
			playerLvl.text = "LV. " + pRef.myStats.currentLevel;
		}
		pRef.myStats.uiReference.cDisplay.SetShowing (true);
		cursorObj.gameObject.SetActive(false);
		levelMenuProper.gameObject.SetActive(true);
		mainMenuObj.SetActive(false);
		_canBeExited = false;
		currentPos = 0;
		onLevelMenu = true;
		CameraFollowS.F.SetZoomIn(true);
		_controlStickMoved = true;
		UpdateAvailableLevelUps();
		pRef.TriggerResting();
	}

	private void TurnOnTravelMenu(){
		//pRef.myStats.uiReference.cDisplay.SetShowing (true);
		cursorObj.gameObject.SetActive(false);
		StartCoroutine(TurnOnTravelNames());
		travelMenuProper.gameObject.SetActive(true);
		mainMenuObj.SetActive(false);
		_canBeExited = false;
		currentTravelScene = Application.loadedLevel;
		for (int i = 0; i < travelMenuSceneNums.Length; i++){
			if(travelMenuSceneNums[i] == currentTravelScene){
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

	private void UpdateAvailableLevelUps(){
		int i = 0;
		foreach (LevelUpItemS l in levelMenuItems){
			l.Initialize(levelHandler.nextLevelUps[i], pRef.myStats.uiReference.cDisplay);
			i++;
		}
		levelMenuItems[currentPos].ShowText();
		UpdateUpgradeEffect();
		
		StoryProgressionS.SaveProgress();
	}

	private void TurnOffLevelUpMenu(){
		pRef.myStats.uiReference.cDisplay.SetShowing (false);
		cursorObj.gameObject.SetActive(true);
		levelMenuProper.gameObject.SetActive(false);
		mainMenuObj.SetActive(true);
		_canBeExited = true;
		currentPos = 0;
		onLevelMenu = false;
		CameraFollowS.F.SetZoomIn(false);
		pRef.TurnOffResting();
	}

	private void TurnOffTravelMenu(){
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

	public void TurnOn(){

		if (!_initialized){
			
			pRef = GetComponentInParent<InGameMenuManagerS>().pRef;
			myControl = pRef.myControl;
			//textStartColor = mainMenuTextObjs[0].color;
			textStartSize = mainMenuTextObjs[0].fontSize;
			_initialized = true;
			levelHandler = PlayerInventoryS.I.GetComponent<LevelUpHandlerS>();
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
		
		cursorObj.anchoredPosition = mainMenuSelectPositions[currentPos].anchoredPosition;
		mainMenuTextObjs[currentPos].fontSize = Mathf.RoundToInt(textStartSize*textSelectSizeMult);
		mainMenuTextObjs[currentPos].color = Color.white;

		foreach(Image i in levelMenuItemOutlines){
			i.color = textStartColor;
		}
	}

	public void TurnOff(){
		foreach(Text t in mainMenuTextObjs){
			t.fontSize = textStartSize;
			t.color = textStartColor;
		}
		currentPos = 0;
		mainMenuTextObjs[0].color = Color.white;
		mainMenuTextObjs[0].fontSize = Mathf.RoundToInt(textStartSize*textSelectSizeMult);
		
		mainMenuTextObjs[1].color = mainMenuTextObjs[2].color = textStartColor;
		mainMenuTextObjs[1].fontSize = mainMenuTextObjs[2].fontSize = textStartSize;
		gameObject.SetActive(false);
		levelMenuProper.gameObject.SetActive(false);
		travelMenuProper.gameObject.SetActive(false);
	}

	private void UpdateUpgradeEffect(){
		int index = 0;
		foreach (LevelUpItemS l in levelMenuItems){
			l.TurnOffVisual();
			levelMenuItemOutlines[index].enabled = false;
			index++;
		}
		if (pRef.myStats.currentLevel < 10){
			playerLvl.text = "LV. 0" + pRef.myStats.currentLevel;
		}else{
			playerLvl.text = "LV. " + pRef.myStats.currentLevel;
		}
		StartCoroutine(TurnOnUpgrades());
	}

	private IEnumerator TurnOnUpgrades(){
		doingEffect = true;
		
		yield return new WaitForSeconds(timeBetweenImageOn);

		int index = 0;
		while (index < levelMenuItems.Length){

			levelMenuItems[index].TurnOnVisual();
			levelMenuItemOutlines[index].enabled = true;
			index++;
			
			yield return new WaitForSeconds(timeBetweenImageOn);

		}
		doingEffect = false;
	}

	private IEnumerator TurnOnTravelNames(){
		doingEffect = true;
		openTravelChoices.Clear();
		for (int i = 0; i < travelMenuChoices.Length; i++){
			travelMenuChoices[i].enabled = false;
			travelMenuChoices[i].color = textStartColor;
		}
		
		yield return new WaitForSeconds(timeBetweenImageOn);

		for (int i = 0; i < travelMenuChoices.Length; i++){

			if (PlayerInventoryS.I.HasReachedScene(travelMenuSceneNums[i])){
				travelMenuChoices[i].color = textStartColor;
				travelMenuChoices[i].enabled = true;
				openTravelChoices.Add(i);
				yield return new WaitForSeconds(timeBetweenImageOn);
			}
			
		}
		doingEffect = false;

	}

	void AdvanceTravelPos(int direction){
		int nextPos = currentPos;
		if (direction > 0){
			for (int i = 0; i < openTravelChoices.Count; i++){
				if (openTravelChoices[i] > currentPos &&
					((currentPos == nextPos) || (currentPos != nextPos && openTravelChoices[i] < nextPos)) ){
					nextPos = openTravelChoices[i];
				}
			}
		}else{
			for (int i = 0; i < openTravelChoices.Count; i++){
				if (openTravelChoices[i] < currentPos &&
					((currentPos == nextPos) || (currentPos != nextPos && openTravelChoices[i] > nextPos)) ){
					nextPos = openTravelChoices[i];
				}
			}
		}
		currentPos = nextPos;
	}
}
