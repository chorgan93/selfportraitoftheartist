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

	public void TriggerHit(){
		if (!myRenderer){
			myRenderer = GetComponent<Renderer>();
			regMat = myRenderer.material;
		} 
		flashing = true;
		onHit = true;
		myRenderer.material = flashMat;
		flashCount = flashAmt;
	}
}
