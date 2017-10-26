using UnityEngine;
using System.Collections;

public class LookOnTouchS : MonoBehaviour {

	public GameObject[] lookPositions;
	public float[] lookDurations;

	private float lookCountdown = 0;
	private int currentTarget = 0;
	private bool isLooking = false;

	private bool activated = false;

	[Header("IgnoreProperties")]
	public int doNotActivateOnRewindNum = -1;
	public int doNotActivateOnHealNum = -1;
	public int doNotActivateOnVirtueNum = -1;
	public float delayCountdown = 0f;
	private bool doDelay = false;

	bool canActivate(){
		bool canAct = true;
		if (doNotActivateOnRewindNum >= 0){
			if (PlayerInventoryS.I.CheckHeal(doNotActivateOnRewindNum)){
				canAct = false;
			}
		}
		if (doNotActivateOnHealNum >= 0){
			if (PlayerInventoryS.I.CheckCharge(doNotActivateOnHealNum)){
				canAct = false;
			}
		}
		if (doNotActivateOnVirtueNum >= 0){
			if (PlayerInventoryS.I.earnedVirtues.Contains(doNotActivateOnVirtueNum)){
				canAct = false;
			}
		}
		return canAct;
	}
	
	// Update is called once per frame
	void Update () {

		if (activated && doDelay){
			delayCountdown -= Time.deltaTime;
			if (delayCountdown <= 0){
				doDelay = false;
				isLooking = true;
				CameraFollowS.F.SetNewPOI(lookPositions[currentTarget]);
				lookCountdown = lookDurations[currentTarget];
			}
		}
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
					if (delayCountdown <= 0){
						doDelay = canActivate();
					}
			else if (canActivate()){
			isLooking = true;
			CameraFollowS.F.SetNewPOI(lookPositions[currentTarget]);
			lookCountdown = lookDurations[currentTarget];
			}
		}
	
	}

}
