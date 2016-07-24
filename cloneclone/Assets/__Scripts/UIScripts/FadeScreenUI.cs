using UnityEngine;
using System.Collections;

public class FadeScreenUI : MonoBehaviour {

	private bool _fadingOut = false;
	public bool fadingOut { get { return _fadingOut; } }
	
	private bool _fadingIn = false;
	public bool fadingIn { get { return _fadingIn; } }

	public float fadeRate = 1f;

	private SpriteRenderer _myRenderer;
	private Color _myColor;

	private string destinationScene = "WorldGeneration";

	// Use this for initialization
	void Start () {

		_myRenderer = GetComponent<SpriteRenderer>();
		_myColor = _myRenderer.color; 
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_fadingOut){
			
			_myColor = _myRenderer.color;
			_myColor.a -= fadeRate*Time.deltaTime;

			if (_myColor.a <= 0){
				_myColor.a = 0;
				_fadingOut = false;
			}
			_myRenderer.color = _myColor;
		}

		if (_fadingIn){
			
			_myColor = _myRenderer.color;
			_myColor.a += fadeRate*Time.deltaTime;
			
			if (_myColor.a >= 1){
				_myColor.a = 1;
				_fadingIn = false;
				Application.LoadLevel(Application.loadedLevel);
			}
			_myRenderer.color = _myColor;
		}
	
	}

	public void FadeOut(float newRate = 0){

		if (_myRenderer == null){
			_myRenderer = GetComponent<SpriteRenderer>();
			_myColor = _myRenderer.color; 
		}

		if (newRate > 0){
			fadeRate = newRate;
		}

		_myColor = _myRenderer.color; 
		_myColor.a = 1;
		_myRenderer.color = _myColor;

		_fadingOut = true;

	}

	public void FadeIn(string nextScene, float newRate = 0){

		if (nextScene != ""){
			destinationScene = nextScene;
		}
		
		if (_myRenderer == null){
			_myRenderer = GetComponent<SpriteRenderer>();
			_myColor = _myRenderer.color; 
		}
		
		if (newRate > 0){
			fadeRate = newRate;
		}
		
		_myColor = _myRenderer.color; 
		_myColor.a = 0;
		_myRenderer.color = _myColor;
		
		_fadingIn = true;
		
	}
}
