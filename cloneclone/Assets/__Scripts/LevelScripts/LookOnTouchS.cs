using UnityEngine;
using System.Collections;

public class LookOnTouchS : MonoBehaviour {

	public GameObject[] lookPositions;
	public float[] lookDurations;

	private float lookCountdown = 0;
	private int currentTarget = 0;
	private bool isLooking = false;

	private bool activated = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (activated && isLooking){
			lookCountdown-=Time.deltaTime;
			if (lookCountdown <= 0){
				currentTarget++;
				if (currentTarget > lookPositions.Length-1){
					isLooking = false;
					CameraFollowS.F.ResetPOI();
				}else{
					CameraFollowS.F.SetNewPOI(lookPositions[currentTarget]);
					lookCountdown = lookDurations[currentTarget];
				}
			}
		}

	}

	void OnTriggerEnter(Collider other){
	
		if (other.gameObject.tag == "Player" && !activated){
			activated = true;
			isLooking = true;
			CameraFollowS.F.SetNewPOI(lookPositions[currentTarget]);
			lookCountdown = lookDurations[currentTarget];
		}
	
	}

}
