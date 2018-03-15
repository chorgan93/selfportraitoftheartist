using UnityEngine;
using System.Collections;

public class SacramentStepS : MonoBehaviour {

	private SacramentHandlerS _myHandler;
	public SacramentHandlerS myHandler { get { return _myHandler; } }
	private bool _initialized = false;

	private bool _stepActive = false;
	public bool stepActive { get { return _stepActive; } }

	private bool waitOnOption = false;
	public bool waitOnOptions { get { return waitOnOption; } }

	[Header("Item Properties")]
	public SacramentTextS[] sacramentTexts;
	private int currentText = 0;
	public SacramentOptionS[] sacramentOptions;
	public SacramentImageS[] sacramentImages;
	public SacramentCombatS sacramentCombat;
	private bool imageStep = false;
	private bool combatStep = false;
	private int currentImage;
	private bool delayOptionSetup = false;

	[Header("Navigation Properties")]
	public SacramentStepS nextStep;
	public string ChangeNextScene = "";
	public int changeStartStep = 0;
	public SacramentChangeNextStep changeStep;

	[Header("Background Properties")]
	public SacramentBackgroundS backgroundFadeIn;
	public SacramentBackgroundS backgroundFadeOut;

	[Header("Sound Properties")]
	public GameObject onSound;
	public GameObject offSound;
	public SacramentBGMS musicTrigger;

	private int _currentOption = 0;
	public int currentOption { get { return _currentOption; } }
	private bool _optionsActive = false;

	private bool stickMoved = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_optionsActive && !_myHandler.usingMouse){
			if (!stickMoved){
			if (_myHandler.StickMoved() > 0f || _myHandler.StickMoved() < 0f){
					stickMoved = true;
				if (_myHandler.StickMoved() > 0f){
					ChangeCurrentOptionSelect(1);
				}else{
					ChangeCurrentOptionSelect(-1);
				}
			}
		}
			else{
				if (Mathf.Abs(_myHandler.StickMoved()) < 0.1f){
					stickMoved = false;
				}
			}
		}

	}

	public void AdvanceText(){
		sacramentTexts[currentText].DeactivateText();
		currentText++;
		if (currentText < sacramentTexts.Length){
			sacramentTexts[currentText].ActivateText(this);
		}else if (!waitOnOption){
			_myHandler.AdvanceStep();
		}else if (delayOptionSetup){
			delayOptionSetup = false;
			DelayedOptionSetup();
		}

	}

	public void AdvanceImage(){
		sacramentImages[currentImage].DeactivateImage();
		currentImage++;
		if (currentImage < sacramentImages.Length){
			sacramentImages[currentImage].ActivateImage(this);
		}else{
			_myHandler.AdvanceStep();
		}
	}

	public void ActivateStep(){
		if (musicTrigger){
			musicTrigger.Activate();
		}
		if (changeStep){
			changeStep.Activate();
		}
		stickMoved = true;
		_optionsActive = false;
		_currentOption=0;
		_stepActive = true;
	delayOptionSetup = false;
		_myHandler.DeactivateWait();
		SetUpImages();
		SetUpOptions();
		SetUpTexts();
		gameObject.SetActive(true);
		SetUpCombats();

		if (backgroundFadeIn){
			backgroundFadeIn.ActivateBackground();
		}
		if (backgroundFadeOut){
			backgroundFadeOut.DeactivateBackground();
		}

		if (onSound){
			Instantiate(onSound);
		}

		if (ChangeNextScene != ""){
			_myHandler.nextSceneString = ChangeNextScene;
			SacramentHandlerS.startStep = changeStartStep;
		}
	}

	public void DeactivateStep(){
		_stepActive = false;
		gameObject.SetActive(false);
		if (offSound){
			Instantiate(offSound);
		}
	}

	public void Initialize(SacramentHandlerS handler){
		if (!_initialized){
			_myHandler = handler;
			_initialized = true;
		}
	}

	void SetUpTexts(){
		currentText = 0;
		if (sacramentTexts.Length > 0){
			for (int i = 0; i < sacramentTexts.Length; i++){
				if (i == 0){
			sacramentTexts[i].ActivateText(this);
				}else{
					sacramentTexts[i].gameObject.SetActive(false);
				}
			}
		}
	}

	void SetUpOptions(){
		if (sacramentTexts.Length > 0){
			if (sacramentOptions.Length > 0){
				for (int i = 0; i < sacramentOptions.Length; i++){
					sacramentOptions[i].Hide();
				}
				delayOptionSetup = true;
				waitOnOption = true;
			}
		}else{
		if (sacramentOptions.Length > 0){
				_currentOption = 0;
				_optionsActive = true;
				for (int i = 0; i < sacramentOptions.Length; i++){
					sacramentOptions[i].Initialize(_myHandler);
					if (i == _currentOption && !sacramentOptions[i].canBeSelected){
						_currentOption++;
					}
					else if (i == _currentOption && sacramentOptions[i].canBeSelected && !_myHandler.usingMouse){
						sacramentOptions[i].StartHover();
					}
				}
			waitOnOption = true;
		}
		}
	}

	void SetUpImages(){
		currentImage = 0;
		if (sacramentImages.Length > 0){
			imageStep = true;
			for (int i = 0; i < sacramentImages.Length; i++){
				if (i == 0){
				sacramentImages[i].ActivateImage(this);
				}else{
					sacramentImages[i].Hide();}
			}
		}
	}

	void SetUpCombats(){
		if (sacramentCombat != null){
			combatStep = true;
			sacramentCombat.StartCombat(this);
		}
	}

	public void EndCombat(SacramentStepS nextStep){
		_myHandler.GoToStep(nextStep);
	}

	void DelayedOptionSetup(){
		_currentOption = 0;
		_optionsActive = true;
		for (int i = 0; i < sacramentOptions.Length; i++){
			sacramentOptions[i].Initialize(_myHandler);
			if (i == _currentOption && !sacramentOptions[i].canBeSelected){
				_currentOption++;
			}
			else if (i == _currentOption && sacramentOptions[i].canBeSelected && !_myHandler.usingMouse){
				sacramentOptions[i].StartHover();
			}
		}
	}

	void ChangeCurrentOptionSelect(int dir){
		if (dir > 0){
			_currentOption++;
			if (_currentOption > sacramentOptions.Length-1){
				_currentOption = 0;
			}
			if (!sacramentOptions[_currentOption].canBeSelected){
				ChangeCurrentOptionSelect(1);
			}else{
				sacramentOptions[_currentOption].StartHover();
			}
		}else{
			_currentOption--;
			if (_currentOption < 0){
				_currentOption = sacramentOptions.Length-1;
			}
			if (!sacramentOptions[_currentOption].canBeSelected){
				ChangeCurrentOptionSelect(-1);
			}else{
			sacramentOptions[_currentOption].StartHover();
		}
		}
	}
}
