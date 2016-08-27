using UnityEngine;
using System.Collections;

public class BlockDisplay3DS : MonoBehaviour {

	private Renderer myRenderer;
	private PlayerController myPlayer;
	private Quaternion startRotation;
	private Vector3 rotateRate = new Vector3(0,600,0f);
	private float rotateCountdownMax = 0.083f;
	private float rotateCountdown;

	private int flashFramesMax = 5;
	private int currentFlashFrames = 0;
	public Color colorFullPower;
	public Color colorNoPower;
	private Color currentColor;
	private bool isFlashing = false;

	private bool initialized = false;

	private Texture startTexture;
	public Texture flashTexture;
	public Texture hitFlashTexture;

	// Use this for initialization
	void Start () {

		myRenderer = GetComponent<Renderer>();
		myPlayer = GetComponentInParent<PlayerController>();
		startRotation = transform.rotation;
		myRenderer.material.color = currentColor = colorFullPower;

		startTexture = myRenderer.material.GetTexture("_MainTex");
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!initialized){
			myPlayer.myStats.AddBlocker(this);
			initialized = true;
		}

		if (myPlayer.showBlock){
			if (!myRenderer.enabled){
				myRenderer.enabled = true;
				DoFlash();
				transform.rotation = startRotation;
				rotateCountdown = rotateCountdownMax;
			}
			ApplyRotation();

			if (isFlashing){
			currentFlashFrames--;
			if (currentFlashFrames <= 0){
					isFlashing = false;
					myRenderer.material.color = currentColor;
					myRenderer.material.SetTexture("_MainTex", startTexture);
			}
			}
		}
		else{
			if (isFlashing){
				currentFlashFrames--;
				if (currentFlashFrames <= 0){
					isFlashing = false;
					myRenderer.material.color = currentColor;
					myRenderer.material.SetTexture("_MainTex", startTexture);
					myRenderer.enabled = false;
				}
			}else{
				myRenderer.enabled = false;
			}

		}
	
	}

	public void DoStartFlash(){
		
		isFlashing = true;
		myRenderer.material.SetTexture("_MainTex", flashTexture);
		myRenderer.material.color = Color.white;
		currentFlashFrames = flashFramesMax;
		currentColor = Color.Lerp(colorNoPower, colorFullPower, myPlayer.myStats.currentDefense/myPlayer.myStats.maxDefense);
	}

	public void DoFlash(){

		isFlashing = true;
		myRenderer.material.SetTexture("_MainTex", hitFlashTexture);
		myRenderer.material.color = Color.white;
		currentFlashFrames = flashFramesMax;
		currentColor = Color.Lerp(colorNoPower, colorFullPower, myPlayer.myStats.currentDefense/myPlayer.myStats.maxDefense);

	}

	void ApplyRotation(){
		rotateCountdown -= Time.deltaTime;
		if (rotateCountdown <= 0){
		transform.Rotate(rotateRate*Time.deltaTime);
			rotateCountdown = rotateCountdownMax;
		}
	}
}
