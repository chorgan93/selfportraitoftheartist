using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LookOnTouchEnemiesS : MonoBehaviour {

	private List<GameObject> lookPositions;
	public List<EnemySpawnerS> lookEnemies;
	public float[] lookDurations;

	private float lookCountdown = 0;
	private int currentTarget = 0;
	private bool isLooking = false;

	private bool activated = false;

	[Header("IgnoreProperties")]
	public int doNotActivateOnRewindNum = -1;
	public int doNotActivateOnHealNum = -1;
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
		return canAct;
	}

	
	// Update is called once per frame
	void Update () {

		if (activated && doDelay){
			delayCountdown -= Time.deltaTime;
			if (delayCountdown <= 0){
				doDelay = false;
				isLooking = true;
				lookPositions = new List<GameObject>();
				for (int i = 0; i < lookEnemies.Count; i++){
					lookPositions.Add(lookEnemies[i].currentSpawnedEnemy.gameObject);
				}
				CameraFollowS.F.SetNewPOI(lookPositions[currentTarget]);
				lookCountdown = lookDurations[currentTarget];
			}
		}
		if (activated && isLooking){
			lookCountdown-=Time.deltaTime;
			if (lookCountdown <= 0){
				currentTarget++;
				if (currentTarget > lookPositions.Count-1){
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

					if (delayCountdown >= 0){
						doDelay = canActivate();
					}
			else if (canActivate()){
				isLooking = true;
				lookPositions = new List<GameObject>();
				for (int i = 0; i < lookEnemies.Count; i++){
					lookPositions.Add(lookEnemies[i].currentSpawnedEnemy.gameObject);
				}
			CameraFollowS.F.SetNewPOI(lookPositions[currentTarget]);
			lookCountdown = lookDurations[currentTarget];
			}
		}
	
	}

}
