using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DifficultySelectS : MonoBehaviour {

	private enum SelectState { Intro, FadeIn, selectDifficulties, End };
	private SelectState currentState = SelectState.Intro;

	[Header("Intro Animation:")]
	public SpriteRenderer fallingSprite;
	public float fallTime;
	private float fallTimeCount;
	private float fallT;
	public float fallY;
	private Vector3 startPos;
	private Vector3 fallingPos;

	[Header("Sin Assignments:")]
	public Text sinTitleText;
	public Text sinChosenText;
	public Text sinDescriptionText;
	public string[] sinTitles;
	public string[] sinDescriptions;
	public Text sinSelectorLeft;
	public Text sinSelectorRight;
	private int sinSelect = 1;

	[Header("Punishment Assignments:")]
	public Text punishmentTitleText;
	public Text punishmentChosenText;
	public Text punishmentDescriptionText;
	public string[] punishmentTitles;
	public string[] punishmentDescriptions;
	public Text punishSelectorLeft;
	public Text punishSelectorRight;
	private int punishSelect = 1;

	private Color leftRightCol;
	private Color textCol;

	[Header("Effect References")]
	public RectTransform selector;
	public Vector2 selectorPosLeft = new Vector2(0, 20);
	public Vector2 selectorPosRight = new Vector2(0, 20);
	public Image difficultyBG;
	public Text loadingText;
	private float bgAlpha;
	public Image cutToBlack;
	public FadeSpriteObjectS[] airLines;
	public float lineActivateTime = 0.1f;
	private float lineActivateCountdown;
	public float cameraShakeTriggerTime = 0.2f;
	private float cameraShakeTriggerCount;

	private float fadeTimeMax = 1f;
	private float fadeCount;
	private float fadeT;
	private Color fadeColor;

	private bool choosingSin = true;
	private bool choosingPunishment = false;
	private ControlManagerS controller;
	private bool selectButtonUp = true;
	private bool cancelButtonUp = true;
	private bool stickReset = false;

	private float selectTimeMax = 0.1f;
	private float selectTimeCount;

	[Header("Control References")]
	public GameObject controllerInstructions;
	public GameObject keybordInstructions;
	private bool usingController = false;

	private float minLoadTime = 3f;

	//________________LOADING VARIABLES
	private int destinationSceneIndex = 15;
	AsyncOperation async;
	private bool startedLoading = false;

	// Use this for initialization
	void Start () {

		controller = GetComponent<ControlManagerS>();
		usingController = controller.ControllerAttached();

		DifficultyS.SetDifficultiesFromInt(1,1);

		loadingText.enabled = false;

		bgAlpha = difficultyBG.color.a;
		leftRightCol = punishSelectorLeft.color;
		turnOffAllText();
		currentState = SelectState.Intro;
		StartCoroutine(difficultySelectSequence());

		startPos = fallingSprite.transform.position;
		fallingSprite.transform.position = fallingPos = new Vector3(fallingSprite.transform.position.x,
		                                               fallingSprite.transform.position.y+fallY,
		                                               fallingSprite.transform.position.z);

		cameraShakeTriggerCount = cameraShakeTriggerTime;

	//	turnOffAllText();
	//	setText();

		punishSelect = sinSelect = 1;
		fallTimeCount = 0;
	
	}

	void Update(){
		if (currentState != SelectState.End){
			lineActivateCountdown -= Time.deltaTime;
			if (lineActivateCountdown <= 0){
				lineActivateCountdown = lineActivateTime;
				int lineToActivate = Mathf.FloorToInt(Random.Range(0, airLines.Length));
				airLines[lineToActivate].gameObject.SetActive(true);
				airLines[lineToActivate].Reinitialize();
			}

			cameraShakeTriggerCount -= Time.deltaTime;
			if (cameraShakeTriggerCount <= 0){
				cameraShakeTriggerCount = cameraShakeTriggerTime;
				CameraShakeS.C.MicroShake(0.4f);
			}
		}

		if (startedLoading){
			if (minLoadTime > 0){
				minLoadTime -= Time.deltaTime;
			}else if (async.progress >= 0.9f){
				async.allowSceneActivation = true;
			}
		}
	}

	void HandleDifficultySelect(){

		if (currentState == SelectState.Intro){
			while (fallTimeCount < fallTime){
				fallTimeCount += Time.deltaTime;
				fallT = fallTimeCount/fallTime;
			}
		}

	}

	void turnOffAllText(){
		sinTitleText.enabled = false;
		sinDescriptionText.enabled = false;
		sinChosenText.enabled = false;

		selector.gameObject.SetActive(false);

		//controllerInstructions.SetActive(false);
		//keybordInstructions.SetActive(false);

		sinSelectorLeft.enabled = sinSelectorRight.enabled = false;

		difficultyBG.enabled = false;
		
		punishmentTitleText.enabled = false;
		punishmentDescriptionText.enabled = false;
		punishmentChosenText.enabled = false;

		punishSelectorLeft.enabled = punishSelectorRight.enabled = false;

		Color textCol = sinTitleText.color;
		textCol.a = 0;
		sinTitleText.color = sinDescriptionText.color = sinChosenText.color = textCol;
		
		textCol = punishmentTitleText.color;
		textCol.a = 0;
		punishmentTitleText.color = punishmentDescriptionText.color = punishmentChosenText.color = textCol;

		textCol = punishSelectorLeft.color;
		textCol.a = 0;
		punishSelectorLeft.color = punishSelectorRight.color = sinSelectorLeft.color = sinSelectorRight.color
			= textCol;
	}

	void turnOnAllText(){
		setText();
		difficultyBG.enabled = true;
		sinTitleText.enabled = true;
		sinDescriptionText.enabled = true;
		sinChosenText.enabled = true;
		
		sinSelectorLeft.enabled = sinSelectorRight.enabled = true;
		
		punishmentTitleText.enabled = true;
		punishmentDescriptionText.enabled = true;
		punishmentChosenText.enabled = true;
		
		punishSelectorLeft.enabled = punishSelectorRight.enabled = true;
	}

	void updateSelectors(){
		
		sinSelectorLeft.enabled = sinSelectorRight.enabled = true;

		if (sinSelect <= 0){
			sinSelectorLeft.enabled = false;
		}
		if (sinSelect >= sinTitles.Length){
			sinSelectorRight.enabled = false;
		}
		
		punishSelectorLeft.enabled = punishSelectorRight.enabled = true;

		if (punishSelect <= 0){
			punishSelectorLeft.enabled = false;
		}
		if (punishSelect >= punishmentTitles.Length){
			punishSelectorRight.enabled = false;
		}
	}

	void setText(){
		
		sinChosenText.text = sinTitles[sinSelect];
		punishmentChosenText.text = punishmentTitles[punishSelect];

		selector.gameObject.SetActive(true);

		if (choosingSin){
			selector.anchoredPosition = selectorPosLeft;
			sinDescriptionText.text = sinDescriptions[sinSelect];
		}else if (choosingPunishment){
			selector.anchoredPosition = selectorPosRight;
			punishmentDescriptionText.text = punishmentDescriptions[punishSelect];
		}else{
			punishmentDescriptionText.text = "";
		}

	}

	IEnumerator difficultySelectSequence(){

		if (currentState == SelectState.Intro){
			yield return new WaitForSeconds(1f);

			while (fallTimeCount <= fallTime){
				fallTimeCount+=Time.deltaTime;
				fallT = fallTimeCount/fallTime;
				fallT = fallT*fallT * (3f - 2f*fallT);

				fallingSprite.transform.position = Vector3.Lerp(fallingPos, startPos, fallT);
				yield return null;
			}

			currentState = SelectState.FadeIn;
		}

		if (currentState == SelectState.FadeIn){
			turnOnAllText();
			while (sinTitleText.color.a < 1f){
				fadeInText();
				yield return null;
			}
			if (usingController){
			//	controllerInstructions.SetActive(true);
			//	keybordInstructions.SetActive(false);
			}else{
			//	controllerInstructions.SetActive(false);
			//	keybordInstructions.SetActive(true);
			}
			choosingSin = true;
			currentState = SelectState.selectDifficulties;
		}

		if (currentState == SelectState.selectDifficulties){
			while (choosingSin || choosingPunishment){
				
				if (!controller.MenuSelectButton()){
					selectButtonUp = true;
				}
				if (!controller.ExitButton()){
					cancelButtonUp = true;
				}

				if (Mathf.Abs(controller.Horizontal()) < 0.1f && Mathf.Abs(controller.Vertical()) < 0.1f){
					stickReset = true;
				}

				selectTimeCount -= Time.deltaTime;
				if (selectTimeCount <= 0){
					sinSelectorLeft.color = sinSelectorRight.color = punishSelectorLeft.color = punishSelectorRight.color = leftRightCol;
				}

				if (choosingSin){
					if (controller.ExitButton() && cancelButtonUp){
						cancelButtonUp = false;
					}
					if (controller.MenuSelectButton() && selectButtonUp){
						choosingPunishment = true;
						choosingSin = false;
						selectButtonUp = false;
						setText();
					}
					if (stickReset){
						if (controller.Horizontal() >= 0.1f){
							stickReset = false;
							selectTimeCount = selectTimeMax;
							sinSelect++;
							if (sinSelect > 3){
								sinSelect = 3;
							}
							if (sinSelect == 3){
								sinSelectorRight.enabled = false;
								sinSelectorLeft.enabled = true;
							}else{
								sinSelectorRight.enabled = true;
								sinSelectorLeft.enabled = true;
								sinSelectorRight.color = Color.white;
							}
							setText();
						}else if (controller.Horizontal() <= -0.1f){
							stickReset = false;
							selectTimeCount = selectTimeMax;
							sinSelect--;
							if (sinSelect < 0){
								sinSelect = 0;
							}
							if (sinSelect == 0){
								sinSelectorLeft.enabled = false;
								sinSelectorRight.enabled = true;
							}else{
								sinSelectorLeft.enabled = true;
								sinSelectorRight.enabled = true;
								sinSelectorLeft.color = Color.white;
							}
							setText();
						}
					}
				}
				else if (choosingPunishment){
					if (controller.ExitButton() && cancelButtonUp){
						choosingPunishment = false;
						choosingSin = true;
						cancelButtonUp = false;
						setText();
					}
					else if (controller.MenuSelectButton() && selectButtonUp){
						choosingPunishment = false;
						choosingSin = false;
						selectButtonUp = false;
						setText();
					}

					if (stickReset){
						if (controller.Horizontal() >= 0.1f){
							stickReset = false;
							selectTimeCount = selectTimeMax;
							punishSelect++;
							if (punishSelect > 3){
								punishSelect = 3;
							}
							if (punishSelect == 3){
								punishSelectorRight.enabled = false;
								punishSelectorLeft.enabled = true;
							}else{
								punishSelectorRight.enabled = true;
								punishSelectorLeft.enabled = true;
								punishSelectorRight.color = Color.white;
							}
							setText();
						}else if (controller.Horizontal() <= -0.1f){
							stickReset = false;
							selectTimeCount = selectTimeMax;
							punishSelect--;
							if (punishSelect < 0){
								punishSelect = 0;
							}
							if (punishSelect == 0){
								punishSelectorLeft.enabled = false;
								punishSelectorRight.enabled = true;
							}else{
								punishSelectorLeft.enabled = true;
								punishSelectorRight.enabled = true;
								punishSelectorLeft.color = Color.white;
							}
							setText();
						}
					}
				}
				yield return null;

			}
			SetDifficulties();
			currentState = SelectState.End;
		}
		if (currentState == SelectState.End){
			cutToBlack.enabled = true;
			loadingText.enabled = true;
			StartLoading();
		}
	}

	void fadeInText(){
		fadeCount += Time.deltaTime;
		fadeT = fadeCount/fadeTimeMax;
		if (fadeT > 1){
			fadeT = 1f;
		}

		fadeColor = sinTitleText.color;
		fadeColor.a = fadeT;
		sinTitleText.color = punishmentTitleText.color = sinChosenText.color = punishmentChosenText.color = sinDescriptionText.color = 
			punishmentDescriptionText.color = fadeColor;

		fadeColor = sinSelectorLeft.color;
		fadeColor.a = fadeT;
		sinSelectorLeft.color = sinSelectorRight.color = punishSelectorLeft.color = punishSelectorRight.color = fadeColor;

		fadeColor = difficultyBG.color;
		fadeColor.a = fadeT*bgAlpha;
		difficultyBG.color = fadeColor;
	}

	void SetDifficulties(){
		if (sinSelect == 0){
			DifficultyS.selectedSinState = DifficultyS.SinState.Easy;
		}
		if (sinSelect == 1){
			DifficultyS.selectedSinState = DifficultyS.SinState.Normal;
		}
		if (sinSelect == 2){
			DifficultyS.selectedSinState = DifficultyS.SinState.Hard;
		}
		if (sinSelect == 3){
			DifficultyS.selectedSinState = DifficultyS.SinState.Challenge;
		}

		if (punishSelect == 0){
			DifficultyS.selectedPunishState = DifficultyS.PunishState.Easy;
		}
		if (punishSelect == 1){
			DifficultyS.selectedPunishState = DifficultyS.PunishState.Normal;
		}
		if (punishSelect == 2){
			DifficultyS.selectedPunishState = DifficultyS.PunishState.Hard;
		}
		if (punishSelect == 3){
			DifficultyS.selectedPunishState = DifficultyS.PunishState.Challenge;
		}
	}

	private void StartLoading(){
		StartCoroutine(LoadNextScene());
		startedLoading = true;
	}

	private IEnumerator LoadNextScene(){
			
		async = Application.LoadLevelAsync(destinationSceneIndex);
		
		async.allowSceneActivation = false;
		yield return async;
	}
}
