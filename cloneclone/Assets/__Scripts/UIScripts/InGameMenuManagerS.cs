using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameMenuManagerS : MonoBehaviour {

	private GameMenuS gameMenu;
	private EquipMenuS equipMenu;
	private LevelUpMenu levelUpMenu;


	public LevelUpMenu levelMenu { get { return levelUpMenu; } }
    public EquipMenuS EquipMenu { get { return equipMenu; } }

	private bool gameMenuActive = false;
	public bool gMenuActive { get { return gameMenuActive; } }
	private bool equipMenuActive = false;
	private bool levelMenuActive = false;

    [HideInInspector]
    public bool gameMenuButtonDown = false;
	private bool equipMenuButtonDown = false;
	private bool exitButtonDown = false;

	private bool playerDead = false;

	public static bool allowMenuUse = false;
	public static bool hasUsedMenu = false;
	public static bool allowFastTravel = false;

	public static bool menuInUse = false;

	private float holdEscapeTime = 3f;
	private float holdEscapeCount = 0f;
	private bool holdingEscape = false;
	public Text escapeText;

	private bool preventMUse = false;

	private PlayerController _pRef;
	public PlayerController pRef { get { return _pRef; } }

	public GameObject gamePausedScreen;
	private bool gamePaused = false;
	public bool isPaused { get { return gamePaused; } }

    // Use this for initialization
    private void Awake()
    {
        // need this in awake for descent scenes
        levelUpMenu = GetComponentInChildren<LevelUpMenu>();

    }
    void Start () {

		gameMenu = GetComponentInChildren<GameMenuS>();
		equipMenu = GetComponentInChildren<EquipMenuS>();
		_pRef = GameObject.Find("Player").GetComponent<PlayerController>();

		gameMenu.SetManager(this);

		gameMenu.TurnOff();
		equipMenu.gameObject.SetActive(false);
		levelUpMenu.gameObject.SetActive(false);
		gamePausedScreen.gameObject.SetActive(false);

        equipMenuButtonDown = true;
        gameMenuButtonDown = true;
        exitButtonDown = true;
		escapeText.enabled = false;
	
	}
	
	// Update is called once per frame
	void Update () {

		/*if (holdingEscape){
			if (!Input.GetKey(KeyCode.Escape)){
				holdingEscape = false;
				holdEscapeCount = 0f;
				escapeText.enabled = false;
			}else{
				holdEscapeCount += Time.deltaTime;
				if (holdEscapeCount >= holdEscapeTime){
					Application.Quit();
				}
			}
		}**/

		if (!_pRef.myStats.PlayerIsDead()){

			if (!_pRef.myControl.GetCustomInput(11)){
				gameMenuButtonDown = false;
			}

		

			if (gamePaused){
				if ((_pRef.myControl.GetCustomInput(11) && !gameMenuButtonDown) 
					|| (_pRef.myControl.GetCustomInput(10) && !equipMenuButtonDown) ||
					(_pRef.myControl.GetCustomInput(1) && !exitButtonDown)){
					_pRef.DelayAttackAllow();
					gamePaused = false;
					gameMenuButtonDown = true;
					equipMenuButtonDown = true;
					gamePausedScreen.gameObject.SetActive(false);
					CameraShakeS.C.UnpauseGame();
				}
			}

		if (gameMenuActive){
                if (_pRef.myControl.GetCustomInput(11) && !gameMenuButtonDown && !gameMenu.inControlMenu){
				gameMenuActive = false;
				gameMenuButtonDown = true;
				gameMenu.TurnOff();
				_pRef.SetTalking(false);
			}
				gameMenu.GameMenuUpdate();
		}

		if (equipMenuActive){

			if (!equipMenu.canBeQuit){
				if (_pRef.myControl.GetCustomInput(1)){
					exitButtonDown = true;
					}else{
						exitButtonDown = false;
					}
			}


			if ((_pRef.myControl.GetCustomInput(10) && !equipMenuButtonDown) || 
					((equipMenu.canBeQuit || equipMenu.inMap) && !exitButtonDown && _pRef.myControl.GetCustomInput(1))){
				equipMenuActive = false;
				//equipMenu.gameObject.SetActive(false);
				equipMenu.TurnOff();
				_pRef.SetTalking(false);
					preventMUse=true;
				if (_pRef.myControl.GetCustomInput(1)){
					exitButtonDown = true;
					_pRef.SetCanSwap(false);
				}else{
					equipMenuButtonDown = true;
				}
			}
		}
			if (!levelMenuActive && !gameMenuActive && !equipMenuActive && !_pRef.InAttack() && !_pRef.isBlocking && !_pRef.isDashing
				&& !_pRef.talking){

				if (!_pRef.inCombat){
					// TODO: turn back on once functional
                    if (allowMenuUse && _pRef.myControl.GetCustomInput(10) && !equipMenuButtonDown && !_pRef.isNatalie){
						equipMenuActive = true;
						equipMenu.TurnOn();
						_pRef.SetTalking(true);
						equipMenuButtonDown = true;
						hasUsedMenu = true;
					}
					/*if (allowMenuUse && (ControlManagerS.controlProfile == 1 || ControlManagerS.controlProfile == 2) && Input.GetKeyDown(KeyCode.M) && !preventMUse){
						equipMenuActive = true;
						equipMenu.TurnOn(true);
						_pRef.SetTalking(true);
						equipMenuButtonDown = true;
						hasUsedMenu = true;
					}**/
					preventMUse = false;
					if (allowMenuUse && _pRef.myControl.GetCustomInput(11) && !gameMenuButtonDown){
						gameMenuActive = true;
						gameMenuButtonDown = true;
						gameMenu.TurnOn(null);
						_pRef.SetTalking(true);
					}
					/*if (Input.GetKeyDown(KeyCode.Escape) && !holdingEscape && !exitButtonDown){
					//Application.Quit();
					holdingEscape = true;
					escapeText.enabled = true;
					Debug.Log("Show text!");
				}**/
				}else{
					if (!gamePaused && ((_pRef.myControl.GetCustomInput(11) && !gameMenuButtonDown) 
						|| (_pRef.myControl.GetCustomInput(10) && !equipMenuButtonDown))){
						gamePaused = true;
						gameMenuButtonDown = true;
						equipMenuButtonDown = true;
						gamePausedScreen.gameObject.SetActive(true);
						CameraShakeS.C.PauseGame();
					}
				}
			}
		}else{
			if (!playerDead){
				gameMenuActive = false;
				gameMenu.TurnOff();
				equipMenuActive = false;
				equipMenu.TurnOff();
				gamePausedScreen.gameObject.SetActive(false);
				playerDead = true;
			}
		}

		if (levelMenuActive && _pRef.myControl.GetCustomInput(1)){
			exitButtonDown = true;
		}

		if (!_pRef.myControl.GetCustomInput(10)){
			equipMenuButtonDown = false;
		}

		if (_pRef.myControl.GetCustomInput(11)){
			gameMenuButtonDown = true;
		}
	
        if (!_pRef.myControl.GetCustomInput(1)){
			exitButtonDown = false;
			_pRef.SetCanSwap(true);
		}

	}

	public void TurnOnLevelUpMenu(){
		levelUpMenu.TurnOn();
		levelMenuActive = true;
		menuInUse = true;
	}

	public void TurnOffLevelUpMenu(){
		levelUpMenu.TurnOff();
		levelMenuActive = false;
		menuInUse = false;
	}

	public void TurnOffFromGameMenu(){

		gameMenuActive = false;
		gameMenuButtonDown = true;
		_pRef.SetTalking(false);
		menuInUse = false;
	}
}
