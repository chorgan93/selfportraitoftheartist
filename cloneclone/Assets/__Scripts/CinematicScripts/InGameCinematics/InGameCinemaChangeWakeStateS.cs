using UnityEngine;
using System.Collections;

public class InGameCinemaChangeWakeStateS : MonoBehaviour {

	private InGameCinematicS myHandler;
	private PlayerController pRef;
	public float setWakeSpeed = 0f;
	public bool setWakeState = true;


	// Use this for initialization
	void Start () {
	
		myHandler = GetComponent<InGameCinematicS>();
		pRef = myHandler.pRef;
		PlayerController.doWakeUp = setWakeState;
		pRef.GetComponentInChildren<Animator>().SetFloat("WakeSpeed", setWakeSpeed);

	}
}
