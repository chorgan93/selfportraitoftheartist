using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VideotapePlayerS : MonoBehaviour {

	[Header("Object References")]
	public GameObject tvOff;
	public GameObject tvTapeA;
	public GameObject tvTapeB;
	public GameObject tvTapeC;
	public GameObject tvAmbient;
	private int activeTape = -1;

	[Header("Examine Strings")]
	public string examineTapeA;
	public string examineTapeB;
	public string examineTapeC;
	public string examineTapeIn;
	public string examineNoTapes;
	public string examinePostTapes;

	private bool seenOneTape = false; // need this for turning on ambient tv vs. off tv
	private bool seenAllTapes = false;
	private bool tapeIsLoaded = false;

	[Header("Progression Stats")]
	public int keyTapeA; // added to "cleared walls" to see which tapes have been watched
	public int itemNumTapeA; // checks against inventory for unlock
	public int keyTapeB;
	public int itemNumTapeB;
	public int keyTapeC;
	public int itemNumTapeC;

	[Header("Navigation Stats")]
	public string sceneStringTapeA;
	public string sceneStringTapeB;
	public string sceneStringTapeC;
	public BarrierS[] barriersToTurnOff;
	public int progressNumToAdd = -1;

	// management variables
	private PlayerDetectS playerDetect;
	private PlayerController playerRef;
	private ControlManagerS controlRef;
	private bool talkButtonDown;
	private bool talking;

	void TurnOffAllTVs(){
		tvOff.gameObject.SetActive(false);
		tvTapeA.gameObject.SetActive(false);
		tvTapeB.gameObject.SetActive(false);
		tvTapeC.gameObject.SetActive(false);
		tvAmbient.gameObject.SetActive(false);
	}
	void SetUpTape(){

		TurnOffAllTVs();

		bool foundTape = false;
		if (!PlayerInventoryS.I.clearedWalls.Contains(keyTapeA)){
			if (PlayerInventoryS.I.CheckForItem(itemNumTapeA)){
				tvTapeA.gameObject.SetActive(true);
				foundTape = true;
			}
		}

		if (!PlayerInventoryS.I.clearedWalls.Contains(keyTapeB) && !foundTape){
			
		}

		if (!PlayerInventoryS.I.clearedWalls.Contains(keyTapeC) && !foundTape){

		}

		if (!foundTape){
			tvOff.gameObject.SetActive(true);
		}
	}
}
