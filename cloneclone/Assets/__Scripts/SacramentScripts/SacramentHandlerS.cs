using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SacramentHandlerS : MonoBehaviour {

	public static int startStep = 0;

	public List<SacramentStepS> sacramentSteps;
	public Image waitForAdvanceImage;
	public Image chooseOptionImage;
	private Vector2 chooseOptionOffset = new Vector2(0f,25f);
	private Text matchColorOption;
	private Image matchColorOptionImage;
	private bool useImageMatch = false;
	private bool useTextMatch = false;

	public string nextSceneString;
	public int nextSceneSpawnPos;

	private int currentStep = 0;
	private List<int> _stepsSeen = new List<int>();
	public List<int> stepsSeen { get { return _stepsSeen; } }

	[Header("Standalone Properties")]
	public bool quitGameOnEnd = false;
	public bool enableEscToQuit = false;
	public bool loadPlayerDown = false;
	public bool healPlayer = false;
	public int setProgress = -1;
	public bool noFade = false;

	bool startedLoading = false;

	private ControlManagerS myManager;
	public ControlManagerS myControl { get { return myManager; } }
	private bool _usingMouse = true;
	public bool usingMouse { get { return _usingMouse; } }
	private bool talkButtonDown = false;

	AsyncOperation async;

	// Use this for initialization
	void Awake(){
		CinematicHandlerS.inCutscene = true;
		myManager = GetComponent<ControlManagerS>();
		if (chooseOptionImage){
			chooseOptionImage.gameObject.SetActive(false);
		}
	}

	void Start () {

		if (ControlManagerS.controlProfile == 1){
			_usingMouse = true;
		}else{
			_usingMouse = false;
		}
		currentStep = startStep;
		startStep = 0;
		_stepsSeen = new List<int>();
		InitializeSteps();
		sacramentSteps[currentStep].ActivateStep();
	}

	void Update(){
		if (startedLoading){
			if (async.progress >= 0.9f){
				async.allowSceneActivation = true;
			}
		}else{
			if (enableEscToQuit && Input.GetKeyDown(KeyCode.Escape)){
				Application.Quit();
			}
		}
		if (chooseOptionImage.gameObject.activeSelf){
			if (useTextMatch){
			chooseOptionImage.color = matchColorOption.color;
			}if (useImageMatch){
				chooseOptionImage.color = matchColorOptionImage.color;
			}
		}
	}

	void LateUpdate(){
		if (!myControl.GetCustomInput(3)){
			talkButtonDown = false;
		}
	}

	public void GoToStep(SacramentStepS nextStep){

		chooseOptionImage.gameObject.SetActive(false);
		sacramentSteps[currentStep].DeactivateStep();
		currentStep = sacramentSteps.IndexOf(nextStep);
		if (currentStep < sacramentSteps.Count){
			sacramentSteps[currentStep].ActivateStep();
		}else{
			if (quitGameOnEnd){
				Application.Quit();
			}else{
				StartCoroutine(LoadNextScene());
			}
		}
	}

	void InitializeSteps(){
		chooseOptionImage.gameObject.SetActive(false);
		for (int i = 0; i < sacramentSteps.Count; i++){
			sacramentSteps[i].Initialize(this);
			sacramentSteps[i].gameObject.SetActive(false);
		}
	}

	public void AdvanceStep(){

		chooseOptionImage.gameObject.SetActive(false);
		sacramentSteps[currentStep].DeactivateStep();
		if (sacramentSteps[currentStep].nextStep != null){
			GoToStep(sacramentSteps[currentStep].nextStep);
		}else{
		currentStep++;
		if (currentStep < sacramentSteps.Count){
			sacramentSteps[currentStep].ActivateStep();
		}else{
			if (quitGameOnEnd){
				Debug.Log("Exiting Sacrament...");
				Application.Quit();
			}else{
				StartCoroutine(LoadNextScene());
			}
		}
		}
	}

	public void ActivateWait(){
		if (!sacramentSteps[currentStep].waitOnOptions){
			waitForAdvanceImage.gameObject.SetActive(true);
		}
	}
	public void DeactivateWait(){
		waitForAdvanceImage.gameObject.SetActive(false);
	}

	private IEnumerator LoadNextScene(){
		FadeScreenUI.NoFade = noFade;
		if (nextSceneSpawnPos > -1){
			SpawnPosManager.whereToSpawn = nextSceneSpawnPos;
		}
		if (setProgress > -1){
			StoryProgressionS.SetStory(setProgress);
		}
		startedLoading = true;
		async = Application.LoadLevelAsync(nextSceneString);
		async.allowSceneActivation = false;
		yield return async;
	}

	public bool TalkButton(){
		if (myControl.GetCustomInput(3) && !talkButtonDown){
			talkButtonDown = true;
			return true;
		}else{
			return false;
		}
	}
	public float StickMoved(){
		if ((Mathf.Abs(myControl.HorizontalMenu()) > 0.1f || Mathf.Abs(myControl.VerticalMenu()) > 0.1f)){
			float valueToReturn = 0f;
			if (myControl.HorizontalMenu() > 0.1f){
				valueToReturn = 1f;
			}else if (myControl.HorizontalMenu() < -0.1f){
				valueToReturn = -1f;
			}else if (myControl.VerticalMenu() > 0.1f){
				valueToReturn = -1f;
			}else{
				valueToReturn = 1f;
			}
			return valueToReturn;
		}else{
			return 0f;
		}
	}

	public void EndHovering(){
		for (int i = 0; i < sacramentSteps[currentStep].sacramentOptions.Length; i++){
			sacramentSteps[currentStep].sacramentOptions[i].EndHover();
		}
	}

	public void SetOptionMark(Vector2 newPos, Text matchColor){
		if (!_usingMouse){
			chooseOptionImage.rectTransform.parent = matchColor.rectTransform.parent;
			chooseOptionImage.rectTransform.anchoredPosition = newPos+chooseOptionOffset;
			matchColorOption = matchColor;
			useTextMatch = true;
			useImageMatch = false;
			chooseOptionImage.color = matchColorOption.color;
			chooseOptionImage.gameObject.SetActive(true);
		}else{
			chooseOptionImage.gameObject.SetActive(false);
		}
	}

	public void SetChooseColor(Text matchColor){
		matchColorOption = matchColor;
		useImageMatch = false;
		useTextMatch = true;
	}
	public void SetChooseColorImage(Image matchColorImage){
		matchColorOptionImage = matchColorImage;
		useImageMatch = true;
		useTextMatch = false;
	}

	public void ReevaluateMouse(){
		if (ControlManagerS.controlProfile == 1){
			_usingMouse = true;
		}else{
			_usingMouse = false;
		}
	}

	public void AddStepSeen(int newStep){
		if (!_stepsSeen.Contains(newStep)){
			_stepsSeen.Add(newStep);
		}
	}
}
