using UnityEngine;
using System.Collections;

public class BgEffectS : MonoBehaviour {

	public bool fadingIn;
	public bool fadingOut;

	private float fadeMin = 0.2f;
	private float fadeMax = 0.8f;

	public float fadeTimeMax = 3f;
	private float fadeTime;

	public float inbetweenTimeMax = 2f;
	private float inbetweenTime;

	private Renderer myRenderer;
	private Color fadeCol;

	public float xMoveMult = 0.1f;
	public float yMoveMult = 0.06f;
	private Vector2 playerStartPos;
	private Vector2 playerCurrentPos;
	private Vector2 startPos;
	private Vector2 currentPos;
	private Transform playerRef;

	private float bgZ;

	// Use this for initialization
	void Start () {

		fadeTime = fadeTimeMax;

		fadingIn = false;
		fadingOut = true;

		myRenderer = GetComponent<Renderer>();

		playerRef = Camera.main.transform;
		playerStartPos = new Vector2(playerRef.position.x, playerRef.position.y);
		startPos = new Vector2(transform.position.x, transform.position.y);
		bgZ = transform.position.z;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (inbetweenTime > 0){

			inbetweenTime -= Time.deltaTime;

		}
		else{ 
			if (fadingIn){

			fadeTime -= Time.deltaTime;

			if (fadeTime > 0){

				fadeCol = myRenderer.material.color;
				fadeCol.a = fadeMin + ((fadeMax-fadeMin)*(1-(fadeTime/fadeTimeMax)));
				myRenderer.material.color = fadeCol;

			}
			else{

				fadeCol = myRenderer.material.color;
				fadeCol.a = fadeMax;
				myRenderer.material.color = fadeCol;

				fadingIn = false;
				fadingOut = true;
				inbetweenTime = inbetweenTimeMax;

				fadeTime = fadeTimeMax;

			}

		}
		else{

			fadeTime -= Time.deltaTime;
			
			if (fadeTime > 0){
				
				fadeCol = myRenderer.material.color;
				fadeCol.a = fadeMin + ((fadeMax-fadeMin)*(fadeTime/fadeTimeMax));
				myRenderer.material.color = fadeCol;
				
			}
			else{

				fadeCol = myRenderer.material.color;
				fadeCol.a = fadeMin;
				myRenderer.material.color = fadeCol;
				
				fadingIn = true;
				fadingOut = false;
				inbetweenTime = inbetweenTimeMax;
				
				fadeTime = fadeTimeMax;
				
			}

		}
		}

		// scroll effect
		playerCurrentPos = new Vector2(playerRef.position.x, playerRef.position.y);
		currentPos = new Vector2(transform.position.x, transform.position.y);

		float dist = Vector2.Distance(playerCurrentPos, playerStartPos);
		Vector2 offset = dist*(playerCurrentPos-playerStartPos).normalized;
		Vector3 newPos = new Vector3(startPos.x + offset.x*xMoveMult, startPos.y + offset.y*yMoveMult, bgZ);
		transform.position = newPos;
	
	}
}
