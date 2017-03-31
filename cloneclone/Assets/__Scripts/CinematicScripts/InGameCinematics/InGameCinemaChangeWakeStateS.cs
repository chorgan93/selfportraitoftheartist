using UnityEngine;
using System.Collections;

public class InGameCinemaChangeWakeStateS : MonoBehaviour {

	public Animator pRef;
	public float setWakeSpeed = 0f;


	// Use this for initialization
	void Start () {
	
		pRef.SetFloat("WakeSpeed", setWakeSpeed);

	}
}
