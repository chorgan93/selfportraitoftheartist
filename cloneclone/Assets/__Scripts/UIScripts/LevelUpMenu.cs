using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelUpMenu : MonoBehaviour {

	public RectTransform cursorObj;
	public RectTransform cursorObjLvl;
	private int currentPos = 0;

	private PlayerController pRef;
	private ControlManagerS myControl;

	[Header("Main Menu Selections")]
	public RectTransform[] mainMenuSelectPositions;
	public Text[] mainMenuTextObjs;
	private Color textStartColor;
	private int textStartSize;
	public float textSelectSizeMult = 1.2f;
	public GameObject mainMenuObj;

	public GameObject levelMenuProper;
	private bool onLevelMenu = false;
	private bool onTravelMenu = false;
	public LevelUpItemS[] levelMenuItems;
	public Image[] levelMenuItemOutlines;
	public RectTransform[] levelMenuPositions;

	private float timeBetweenImageOn = 0.1f;
	private bool doingEffect = false;

	private bool _canBeExited = false;
	public bool canBeExited { get { return _canBeExited; } }

	private bool _controlStickMoved = false;
	private bool _selectButtonDown = false;
	private bool _exitButtonDown = false;

	private bool _initialized = false;

	private LevelUpHandlerS levelHandler;

	[HideInInspector]
	public bool sendExitMessage = false;

	// Use this for initialization
	void Start () {
	
		levelMenuProper.gameObject.SetActive(false);

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

				
				mainMenuTextObjs[currentPos].fontSize = Mathf.RoundToInt(textStartSize*textSelectSizeMult);
				mainMenuTextObjs[currentPos].color = Color.white;
			}

			cursorObj.anchoredPosition = mainMenuSelectPositions[currentPos].anchoredPosition;

			if (!_selectButtonDown && myControl.MenuSelectButton()){
				_selectButtonDown = true;
				if (currentPos == 0){
					TurnOnLevelUpMenu();
				}
				if (currentPos == 2){
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

	public void TurnOn(){

		if (!_initialized){
			
			pRef = GetComponentInParent<InGameMenuManagerS>().pRef;
			myControl = pRef.myControl;
			textStartColor = mainMenuTextObjs[0].color;
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

	private void TurnOff(){
		foreach(Text t in mainMenuTextObjs){
			t.fontSize = textStartSize;
			t.color = textStartColor;
		}

		gameObject.SetActive(false);
		levelMenuProper.gameObject.SetActive(false);
	}

	private void UpdateUpgradeEffect(){
		int index = 0;
		foreach (LevelUpItemS l in levelMenuItems){
			l.TurnOffVisual();
			levelMenuItemOutlines[index].enabled = false;
			index++;
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
}
