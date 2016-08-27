using UnityEngine;
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
	private int currentSelection = 0;
	public GameObject selectOrb;

	private Vector3 selectionScale;

	private bool stickReset = false;

	public SpriteRenderer loadBlackScreen;
	private bool loading = false;

	private string newGameScene = "IntroCutscene";

	// Use this for initialization
	void Start () {

		Cursor.visible = false;

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
	
	}
	
	// Update is called once per frame
	void Update () {


		if (!started){
			allowStartTime -= Time.deltaTime;
			if (allowStartTime <= 0 && (myController.BlockButton() || Input.GetKeyDown(KeyCode.Return))){
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
				}
			}else{
				
				if (Mathf.Abs(myController.Vertical()) < 0.1f){
					stickReset = true;
				}

				// go down
				if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || 
				    (myController.Vertical() < -0.5f && stickReset)){
					if (myController.Vertical() < -0.5f){
						stickReset = false;
					}
					currentSelection ++;
					if (currentSelection > menuSelections.Length-1){
						currentSelection = 0;
					}
					
					SetSelection();
				}

				// go up
				if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || 
				    (myController.Vertical() > 0.5f && stickReset)){
					if (myController.Vertical() > 0.5f){
						stickReset = false;
					}
					currentSelection --;
					if (currentSelection < 0){
						currentSelection = menuSelections.Length-1;
					}

					SetSelection();
				}

				if ((Input.GetKeyDown(KeyCode.Return) || myController.BlockButton()) && !loading){
					loadBlackScreen.gameObject.SetActive(true);
					loading = true;
					selectOrb.SetActive(false);
				}
			}
		}else{
			if (loadBlackScreen.color.a >= 1f){
				Application.LoadLevel(newGameScene);
			}
		}
	
	}

	void SetSelection(){

		for (int i = 0; i < menuSelections.Length; i++){
			if (i == currentSelection){
				menuSelections[i].localScale = selectionScale*1.2f;
				selectOrb.transform.position = menuSelections[i].position;
			}else{
				menuSelections[i].localScale = selectionScale;
			}
		}

	}
}
