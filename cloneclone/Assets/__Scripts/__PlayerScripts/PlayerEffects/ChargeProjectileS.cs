using UnityEngine;
using System.Collections;

public class ChargeProjectileS : MonoBehaviour {

	public Material flashMat;
	private Material regMat;
	private Renderer myRenderer;
	public float flashAmt;
	private float flashCount;

	private bool flashing = false;
	private bool onHit = false;
	public Color projColor;

	public float autoKillTime = 0.5f;
	public float fadeRate = -1;
	public bool fadeOnHit = false;
	private bool fadeOut = false;

	// Use this for initialization
	void Start () {

		myRenderer = GetComponent<Renderer>();
		regMat = myRenderer.material;
		regMat.color = projColor;
		myRenderer.material = flashMat;
		flashCount = flashAmt;
		flashing = true;

	
	}
	
	// Update is called once per frame
	void Update () {

		autoKillTime -= Time.deltaTime;
		if (autoKillTime <= 0 && !onHit){
			TriggerHit();
		}

		if (flashing){
			if (fadeOut){
				projColor = regMat.color;
				projColor.a -= fadeRate*Time.deltaTime;
				if (projColor.a <= 0){
					gameObject.SetActive(false);
				}else{
					regMat.color = projColor;
				}
			}else{
			flashCount -= Time.deltaTime;
			if (flashCount <= 0){
				flashing = false;
				if (onHit){
					gameObject.SetActive(false);
				}else{
					myRenderer.material = regMat;
				}
			}
			}
		}
	
	}

	public void TriggerHit(){
		if (fadeRate <= 0|| (fadeRate > 0 && autoKillTime <= 0)){
		if (!myRenderer){
			myRenderer = GetComponent<Renderer>();
			regMat = myRenderer.material;
		} 
		if (fadeRate > 0){
			fadeOut = true;
		}
		flashing = true;
		onHit = true;
		if (!fadeOnHit){
		myRenderer.material = flashMat;
		flashCount = flashAmt;
		}else{
			myRenderer.material = regMat;
		}
		}
	}
}
