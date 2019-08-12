using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RetryFightUI : MonoBehaviour {

	public GameObject wholeUI;
	public RectTransform selector;
	public RectTransform[] selectorPos;
	private int currentPos;

	private bool _initialized = false;
	private DarknessPercentUIS myDarknessCounter;

	private ControlManagerS myController;
	private bool stickReset = false;
	private bool selectButtonDown = true;

	public static bool allowRetry = false;
	public bool retryActive = false;

    public static RetryFightUI fightRef;
    public int addProgressOnRestart = -1;

    private void Awake()
    {
        fightRef = this;
    }

    void Start(){
		if (!retryActive){
		    TurnOff();
		}
	}

	public void Initialize(DarknessPercentUIS newD){
        if (!_initialized && !newD.standaloneInScene){
		myDarknessCounter = newD;
			myController = newD.pStatRef.GetComponent<ControlManagerS>();
			_initialized = true;
		}
		TurnOff();
	}
	
	// Update is called once per frame
	void Update () {

		if (_initialized && retryActive){

			if (Mathf.Abs(myController.VerticalMenu()) > 0.1f){
				if (stickReset){
					if (myController.VerticalMenu() < 0){
						currentPos++;
						//Debug.Log("Retry cursor moved down! " + currentPos + " / " + selectorPos.Length);
						if (currentPos > selectorPos.Length-1){
							//Debug.Log("Current pos is greater than length-1! " + currentPos + " > " + selectorPos.Length-1);
							currentPos = 0;
						}
					}else{
						currentPos--;
						//Debug.Log("Retry cursor moved up! " + currentPos + " / " + selectorPos.Length);
						if (currentPos < 0){
							//Debug.Log("Current pos is less than 0! " + currentPos + " < 0");
							currentPos = selectorPos.Length-1;
						}
					}
					selector.anchoredPosition = selectorPos[currentPos].anchoredPosition;
				}
				stickReset = false;
			}else{
				stickReset = true;
			}
			if (myController.GetCustomInput(3)){
				if (!selectButtonDown){
				if (currentPos == 0){
					GameOverS.tempReviveScene = Application.loadedLevelName;
						CameraEffectsS.E.SetNextScene(Application.loadedLevelName);
						GameOverS.tempRevivePosition = SpawnPosManager.whereToSpawn;
						PlayerController.doWakeUp = false;
						PlayerStatsS.healOnStart = true;
                        if (addProgressOnRestart > -1){
                            StoryProgressionS.SetStory(addProgressOnRestart);
                            addProgressOnRestart = -1;
                        }
					}else{
						GameOverS.tempReviveScene = "";
						GameOverS.tempRevivePosition = -1;
						CameraEffectsS.E.SetNextScene(GameOverS.reviveScene);
					}
					CameraEffectsS.E.fadeRef.StartLoading();
				myDarknessCounter.SetAdvance();
				TurnOff();
				allowRetry = false;
				}
				selectButtonDown = true;
			}else{
				selectButtonDown = false;
			}
		}
	
	}

	public void TurnOn(){

        //Debug.LogError("Calling Retry UI!");
		if (!retryActive){
            //Debug.LogError("Retry UI should be on!!");
		currentPos = 0;
		selector.anchoredPosition = selectorPos[currentPos].anchoredPosition;
		wholeUI.gameObject.SetActive(true);
			retryActive = true;
		}
	}

	void TurnOff(){
		wholeUI.gameObject.SetActive(false);
		currentPos = 0;
		retryActive = false;
	}
}
