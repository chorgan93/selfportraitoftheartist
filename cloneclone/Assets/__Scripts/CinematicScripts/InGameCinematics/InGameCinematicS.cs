using UnityEngine;
using System.Collections;

public class InGameCinematicS : MonoBehaviour {

	private PlayerController _pRef;
	public PlayerController pRef { get { return _pRef; } }

	public InGameCinemaTextS[] cinemaDialogues;
	public InGameCinemaCameraS[] cinemaCameraMoves;
	public InGameCinemaActivateS[] cinemaOnOff;
	public InGameCinemaMoveObjS[] cinemaMoveObj;
	public InGameCinemaLerpObjS[] cinemaLerpObj;
	public InGameCinemaWaitS[] cinemaWait;

    public CreditsManagerS cinemaCredits;
    private bool creditsScene = false;

	private int currentStep = 0;
	private float currentCountdown = 0f;
	private bool timedStep = false;

	public string endCinemaScene = "";
	public int endCinemaSpawn = 0;
	public bool resetInventoryStats = false;
	public float fadeRate = 0.4f;
	public bool wakeNextScene = true;
	public bool noBuddy = false;
	public bool resetPOIOnEnd = false;
	public bool allowWalk = false;

    private bool waitForTaunt = false;

    private bool endCredits = false;

	[HideInInspector]
	public bool dialogueDone = true;

	public static bool turnOffBuddies = false;

	public static bool inGameCinematic = false;

	[Header("Ending Properties")]
	public bool healDarknessScene = false;
    public bool skippable = false;
    private bool _skipMode = false;
    public bool skipMode { get { return _skipMode;  } }
    public int skippableSceneIndex = -1;

    private bool skipButtonDown = false;
    public GameObject[] forceONonSkip;
    public GameObject[] forceOFFonSkip;
	

	// Use this for initialization
	void Awake () {
	
		inGameCinematic = true;
		if(noBuddy){
			turnOffBuddies = true;
		}
		_pRef = GameObject.Find("Player").GetComponent<PlayerController>();

	}

	void Start () {

		CheckCurrentStep();

		_pRef.SetTalking(true, allowWalk);

        if (cinemaCredits){
            creditsScene = true;
        }

	}
	
	// Update is called once per frame
	void Update () {

        if (creditsScene)
        {
            if (!endCredits && cinemaCredits.creditsFinished){
                AdvanceCinematic();
                Debug.Log("Going to next scene!");
            }
        }
        else
        {
            if (skippable)
            {
                if (!_skipMode)
                {
                    if (pRef.myControl.GetCustomInput(10))
                    {
                        if (!skipButtonDown)
                        {
                            if (PlayerInventoryS.I.SkippableScenes.Contains(skippableSceneIndex))
                            {
                                _skipMode = true;
                                if (SkipSceneS.instance)
                                {
                                    SkipSceneS.instance.ShowMessage(true);
                                }
                            }
                            else if (SkipSceneS.instance)
                            {
                                SkipSceneS.instance.ShowMessage(false);
                                skipButtonDown = true;
                            }

                        }
                    }
                    else
                    {
                        skipButtonDown = false;
                    }
                }
            }

            if (waitForTaunt)
            {
                if (pRef.inAttackDuration)
                {
                    AdvanceCinematic();
                    waitForTaunt = false;
                }
            }
            else if (timedStep)
            {
                if (_skipMode)
                {
                    currentCountdown = 0;
                    dialogueDone = true;
                }
                else
                {
                    currentCountdown -= Time.deltaTime;
                }
                if (currentCountdown <= 0 && dialogueDone)
                {
                    AdvanceCinematic();
                }
            }
            else if (_skipMode)
            {
                dialogueDone = true;
                DialogueManagerS.D.EndText(true, false);
                AdvanceCinematic();
            }
        }
	
	}

	public void TurnOnTime(){
		timedStep = true;
	}

	public void AdvanceCinematic(){

		timedStep = false;
		currentCountdown = 0f;

		if (!pRef.myStats.PlayerIsDead()){
		currentStep++;

		if (CheckCurrentStep()){
				if (endCinemaScene != ""){

					if (healDarknessScene){
						DarknessPercentUIS.DPERCENT.ActivateColinReset();
						CameraEffectsS.E.SetNextScene(GameOverS.tempReviveScene);
						SpawnPosManager.whereToSpawn = GameOverS.tempRevivePosition;
					//	GameOverS.tempReviveScene = "";
					//	GameOverS.tempRevivePosition = 0;
						BGMHolderS.BG.FadeInAll();
					}else{

						CameraEffectsS.E.SetNextScene(endCinemaScene);
						SpawnPosManager.whereToSpawn = endCinemaSpawn;
					}
				if (resetInventoryStats){
					pRef.GetComponent<GameOverS>().PrepareForRespawn();
				}
				CameraEffectsS.E.FadeIn(fadeRate);
				PlayerController.doWakeUp = wakeNextScene;
				turnOffBuddies = false;
                    if (creditsScene)
                    {
                        endCredits = true;
                    }
			}else{
				EndCinematic();
			}
		}
		}

	}

	private bool CheckCurrentStep(){
        if (creditsScene){
            return true;
        }

		currentCountdown = 0f;

		dialogueDone = true;
		bool cinematicDone = true;
		if (cinemaDialogues != null){
			for (int i = 0; i < cinemaDialogues.Length; i++){
				if (cinemaDialogues[i].myCinemaStep == currentStep){
					cinematicDone = false;
					cinemaDialogues[i].gameObject.SetActive(true);
					dialogueDone = false;
				}
			}
		}
		if (cinemaCameraMoves != null){
		foreach (InGameCinemaCameraS c in cinemaCameraMoves){
			if (c.myCinemaStep == currentStep){
				cinematicDone = false;
				c.gameObject.SetActive(true);
				if (c.moveTime > 0){
					timedStep = true;
						if (c.moveTime > currentCountdown){
						currentCountdown = c.moveTime;
					}
				}
			}
		}
		}
		if (cinemaMoveObj != null){
		foreach (InGameCinemaMoveObjS c in cinemaMoveObj){
			if (c.myCinemaStep == currentStep){
				cinematicDone = false;
				c.gameObject.SetActive(true);
				if (c.moveTime > 0){
					timedStep = true;
						if (c.turnOnEnd != null){
						if (c.moveTime+c.turnOnTime > currentCountdown){
							currentCountdown = c.moveTime+c.turnOnTime;
					}
						}else{
							if (c.moveTime > currentCountdown){
								currentCountdown = c.moveTime;
							}
						}
				}
			}
		}
		}
		if (cinemaLerpObj != null){
			foreach (InGameCinemaLerpObjS c in cinemaLerpObj){
				if (c.myCinemaStep == currentStep){
					cinematicDone = false;
					c.gameObject.SetActive(true);
					if (c.moveTime > 0){
						timedStep = true;
						if (c.turnOnEnd != null){
							if (c.moveTime+c.turnOnTime > currentCountdown){
								currentCountdown = c.moveTime+c.turnOnTime;
							}
						}else{
							if (c.moveTime > currentCountdown){
								currentCountdown = c.moveTime;
							}
						}
					}
				}
			}
		}
		if (cinemaOnOff != null){
		foreach (InGameCinemaActivateS a in cinemaOnOff){
			if (a.myCinemaStep == currentStep){
				cinematicDone = false;
				a.gameObject.SetActive(true);
				if (a.activateTime > 0){
					timedStep = true;
					if (a.activateTime > currentCountdown){
						currentCountdown = a.activateTime;
					}
				}
			}
		}
		}
		if (cinemaWait != null){
		foreach (InGameCinemaWaitS w in cinemaWait){
			if (w.myCinemaStep == currentStep){
				cinematicDone = false;
					w.gameObject.SetActive(true);
                    waitForTaunt = w.waitForTaunt;
                    if (waitForTaunt){
                        pRef.SetTalking(true, false, false, true);
                    }
				if (w.waitTime > 0){
					timedStep = true;
					if (w.waitTime > currentCountdown){
						currentCountdown = w.waitTime;
					}
				}
			}
		}
		}

		return cinematicDone;

	}

	private void EndCinematic(){
		inGameCinematic = false;
		turnOffBuddies = false;
        if (DialogueManagerS.D.showingText){
            DialogueManagerS.D.EndText();
        }
		pRef.SetTalking(false);
		if (noBuddy){
			pRef.SetBuddy(true);
		}
		if (resetPOIOnEnd){
			CameraFollowS.F.ResetPOI();
		}
        if (skippableSceneIndex > -1){
            PlayerInventoryS.I.AddSkippableScene(skippableSceneIndex);
        }
        if (_skipMode){
            ActivateOnSkip();
        }
        if (SkipSceneS.instance){
            SkipSceneS.instance.HideMessage();
        }
		Destroy(gameObject);

	}

    void ActivateOnSkip(){
        if (forceONonSkip != null){
            for (int i = 0; i < forceONonSkip.Length; i++)
            {
                forceONonSkip[i].SetActive(true);
            }

        }
        if (forceOFFonSkip != null){
            for (int i = 0; i < forceOFFonSkip.Length; i++){
                forceOFFonSkip[i].SetActive(false);
            }
        }
    }
}
