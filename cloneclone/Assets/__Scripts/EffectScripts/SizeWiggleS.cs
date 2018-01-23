using UnityEngine;
using System.Collections;

public class SizeWiggleS : MonoBehaviour {

	public Vector3 wiggleMaxDifference;
	public float wiggleRate;
	private float wiggleCountdown;
	private Vector3 startSize;
	public bool setCutscene = true;

	void Awake(){
		if (setCutscene){
		CinematicHandlerS.inCutscene = true;
		}
	}

	// Use this for initialization
	void Start () {

		startSize = transform.localScale;
		wiggleCountdown = wiggleRate;
	
	}
	
	// Update is called once per frame
	void Update () {

		wiggleCountdown -= Time.deltaTime;
		if (wiggleCountdown <= 0){
			wiggleCountdown = wiggleRate;
			transform.localScale = startSize + wiggleMaxDifference*Random.insideUnitCircle.x;
		}
	
	}
}
