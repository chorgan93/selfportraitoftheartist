using UnityEngine;
using System.Collections;

public class BuddyNoChargeEffectS : MonoBehaviour {

	private TextMesh exclamationPoint;
	public TextMesh subPoint;
	private BuddyS myBuddy;

	private float flickerInTime = 0.03f;
	private float flickerOffTime = 0.04f;
	public float showTime = 0.6f;
	private float flickerFinalOffTime = 0.02f;
	public int numFlickersOn = 4;
	public int numFlickersOff = 8;
	public int numFlickersFinalOff = 6;
	private int currentFlicker = 0;

	private bool pointShowing = false;

	public GameObject soundObj;

	// Use this for initialization
	void Start () {
	
		exclamationPoint = GetComponent<TextMesh>();
		myBuddy = GetComponentInParent<BuddyS>();
		myBuddy.SetBuddyNoCharge(this);
		exclamationPoint.color = myBuddy.shadowColor;
		exclamationPoint.text = subPoint.text = "";

	}
	
	public void FireEffect(){
		
		if (soundObj){
			Instantiate(soundObj);
		}

		StopCoroutine(ExclamationEffect());
		myBuddy.playerRef.myStats.warningRef.NewMessage("— INSUFFICIENT Charge —", Color.white, Color.magenta, false, 0);
		StartCoroutine(ExclamationEffect());
	}

	IEnumerator ExclamationEffect(){
		exclamationPoint.text = subPoint.text = "!";
		pointShowing = true;

		currentFlicker = 0;
		while (currentFlicker < numFlickersOn){
			
			yield return new WaitForSeconds(flickerInTime);

			if (pointShowing){
				exclamationPoint.text = "";
			}else{
				exclamationPoint.text = "!";
			}

			subPoint.text = exclamationPoint.text;
			pointShowing = !pointShowing;
			currentFlicker++;
		}

		pointShowing = true;
		exclamationPoint.text = subPoint.text = "!";

		yield return new WaitForSeconds(showTime);

		exclamationPoint.text = subPoint.text = "";
		pointShowing = false;
		currentFlicker = 0;
		while (currentFlicker < numFlickersOff){

			yield return new WaitForSeconds(flickerOffTime);

			if (pointShowing){
				exclamationPoint.text = "";
			}else{
				exclamationPoint.text = "!";
			}

			pointShowing = !pointShowing;
			subPoint.text = exclamationPoint.text;
			currentFlicker++;
		}

		currentFlicker = 0;
		while (currentFlicker < numFlickersFinalOff){

			yield return new WaitForSeconds(flickerFinalOffTime);

			if (pointShowing){
				exclamationPoint.text = "";
			}else{
				exclamationPoint.text = "!";
			}

			subPoint.text = exclamationPoint.text;
			pointShowing = !pointShowing;
			currentFlicker++;
		}

		pointShowing = false;
		exclamationPoint.text = subPoint.text = "";
	}

	void OnDisable(){
		pointShowing = false;
		exclamationPoint.text = subPoint.text = "";
	}
}
