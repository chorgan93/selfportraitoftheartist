﻿using UnityEngine;
using System.Collections;

public class MainMenuNavigationS : MonoBehaviour {

	private ControlManagerS myController;

	public float allowStartTime = 3f;
	private bool started = false;

	public SpriteRenderer fadeOnZoom;
	public float fadeTime = 1f;
	public float zoomInRate = 3f;
	private float minZoom = 0.3f;

	public GameObject[] textTurnOff;
	public GameObject firstScreenTurnOff;
	private bool onNewScreen = false;

	public GameObject secondScreenObject;

	private Camera myCam;
	private float startOrtho;

	public GameObject secondScreenIntro;
	public GameObject secondScreenLoop;

	public Transform[] menuSelections;
	public TextMesh[] menuSelectionsText;
	private int currentSelection = 0;
	public GameObject selectOrb;
	public GameObject credits;

	private Vector3 selectionScale;
	private Color selectionStartColor;

	private bool stickReset = false;

	public SpriteRenderer loadBlackScreen;
	private bool loading = false;

	private string newGameScene = "IntroCutscene";
	//private string newGameScene = "InfiniteScene";
	private string twitterLink = "http://www.twitter.com/melessthanthree";
	private string twitterLinkII = "http://twitter.com/NicoloDTelesca";
	private string facebookLink = "http://www.facebook.com/lucahgame/";

	private string cheatString = "";
	private bool allowCheats = false; // TURN OFF FOR DEMO

	public InfiniteBGM startMusic;

	// Use this for initialization
	void Start () {

		//Cursor.visible = false;
		PlayerStatsS.godMode = false;

		fadeOnZoom.gameObject.SetActive(false);
		firstScreenTurnOff.SetActive(true);

		foreach (GameObject t in textTurnOff){
			t.SetActive(true);
		}

		myController = GetComponent<ControlManagerS>();

		myCam = GetComponent<Camera>();
		startOrtho = myCam.orthographicSize;

		secondScreenObject.gameObject.SetActive(false);
		secondScreenLoop.SetActive(false);
		loadBlackScreen.gameObject.SetActive(false);
		selectOrb.SetActive(false);

		selectionScale = menuSelections[0].localScale;
		selectionStartColor = menuSelectionsText[0].color;

		startMusic.FadeIn();
	
	}
	
	// Update is called once per frame
	void Update () {

		//CheckCheats();

		if (!started){
			allowStartTime -= Time.deltaTime;
			if (allowStartTime <= 0 && (myController.TalkButton() || Input.GetKeyDown(KeyCode.Return))){
				started = true;
				foreach (GameObject t in textTurnOff){
					t.SetActive(false);
				}
				fadeOnZoom.gameObject.SetActive(true);
			}
		}else if (!onNewScreen){
			if (myCam.orthographicSize > minZoom){
				myCam.orthographicSize -= Time.deltaTime*zoomInRate;
			}else{
				myCam.orthographicSize = minZoom;
			}
			if (fadeOnZoom.color.a >= 1f){
				onNewScreen = true;
				myCam.orthographicSize = startOrtho;
				firstScreenTurnOff.SetActive(false);
				secondScreenObject.SetActive(true);
				fadeOnZoom.gameObject.SetActive(false);
			}
		}else if (!loading){
			if (!secondScreenLoop.activeSelf){
				if (secondScreenIntro == null){
					secondScreenLoop.SetActive(true);
					SetSelection();
					selectOrb.SetActive(true);
					credits.SetActive(true);
				}
			}else{
				
				if (Mathf.Abs(myController.Vertical()) < 0.1f){
					stickReset = true;
				}

				// go down
				if (myController.Vertical() < -0.1f && stickReset){
					if (myController.Vertical() < -0.1f){
						stickReset = false;
					}
					currentSelection ++;
					if (currentSelection > menuSelections.Length-1){
						currentSelection = 0;
					}
					
					SetSelection();
				}

				// go up
				if ((myController.Vertical() > 0.1f && stickReset)){
					if (myController.Vertical() > 0.1f){
						stickReset = false;
					}
					currentSelection --;
					if (currentSelection < 0){
						currentSelection = menuSelections.Length-1;
					}

					SetSelection();
				}

				if ((Input.GetKeyDown(KeyCode.Return) || myController.TalkButton()) && !loading){
					if (currentSelection == 0 || currentSelection == 1){
						startMusic.FadeOut();
					loadBlackScreen.gameObject.SetActive(true);
					loading = true;
					selectOrb.SetActive(false);
						if (currentSelection == 0){
							StoryProgressionS.NewGame(); // reset for new game progress
						}else{
							SaveLoadS.Load();
							newGameScene = GameOverS.reviveScene;
						}
						PlayerStatsS.healOnStart = true;
					}
					//if (currentSelection == 1){
					//	Application.OpenURL(facebookLink);
					//}
					/*if (currentSelection == 1){
						//Application.OpenURL(twitterLink);
					}*/
					if (currentSelection == 2){
						Application.OpenURL(twitterLinkII);
					}
				}
			}
		}else{
			if (loadBlackScreen.color.a >= 1f){
				Application.LoadLevel(newGameScene);
			}
		}
	
	}

	void SetSelection(){

		Color correctCol = Color.white;
		for (int i = 0; i < menuSelections.Length; i++){
			if (i == currentSelection){
				//menuSelections[i].localScale = selectionScale*1.2f;
				selectOrb.transform.position = menuSelections[i].position;
				correctCol = Color.white;
				correctCol.a = menuSelectionsText[i].color.a;
				menuSelectionsText[i].color = correctCol;;
			}else{
				//menuSelections[i].localScale = selectionScale;
				correctCol = selectionStartColor;
				correctCol.a = menuSelectionsText[i].color.a;
				menuSelectionsText[i].color = correctCol;
			}
		}

	}

	private void CheckCheats(){

		if (Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}

		if (allowCheats){
		if (Input.GetKeyDown(KeyCode.G)){
			cheatString += "G";
			if (cheatString == "GGGG"){
				PlayerStatsS.godMode = true;
				Debug.Log("god mode on");
			}
		}

		if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)){
			cheatString = "";
			PlayerStatsS.godMode = false;
			Debug.Log("god mode off");
		}
		}

	}
}
