using UnityEngine;
using System.Collections;

public class FinalPortalS : MonoBehaviour {

	private SpriteRenderer _myRenderer;
	public float fadeRate = 1f;
	public float fadeDelay = 2f;
	private Color myColor;

	private bool fadeIn = false;
	private bool unlocked = false;

	void Awake(){
		
		Initialize();
	}
	// Use this for initialization
	void Start () {
	
		UnlockCheck();

	}
	
	// Update is called once per frame
	void Update () {

		if (fadeIn){
			if (fadeDelay <= 0){
				myColor = _myRenderer.color;
				myColor.a += fadeRate*Time.deltaTime;
				if (myColor.a >= 1){
					myColor.a = 1;
					fadeIn = false;
				}
				_myRenderer.color = myColor;
			}
			else{
				fadeDelay -= Time.deltaTime;
			}
		}
	
	}

	private void Initialize(){

		_myRenderer = GetComponent<SpriteRenderer>();
		myColor = _myRenderer.color;
		myColor.a = 0;
		_myRenderer.color = myColor;

	}

	private void FadeStart(){

		myColor = _myRenderer.color;
		myColor.a = 0;
		_myRenderer.color = myColor;
		fadeIn = true;

	}

	private void UnlockCheck(){
		if (!unlocked){
			FadeStart();
			unlocked = true;
		}

	}

	void OnEnable(){
		UnlockCheck();
	}
}
