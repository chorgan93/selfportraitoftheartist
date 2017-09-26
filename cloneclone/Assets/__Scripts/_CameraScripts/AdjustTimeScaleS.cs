using UnityEngine;
using System.Collections;

public class AdjustTimeScaleS : MonoBehaviour {

	public float newTimeScale = 1f;
	// Use this for initialization
	void Start () {
		
		Time.timeScale = newTimeScale;
		PlayerSlowTimeS.witchTimeActive = false;
	}

}
