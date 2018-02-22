using UnityEngine;
using System.Collections;

public class DeathCountdownHandlerS : MonoBehaviour {

	public PlayerStatsS playerRef;
	public float newDeathTime = -1f;

	// Use this for initialization
	void Start () {

		DeathCountdownS.DC.currentHandler = this;
		if (newDeathTime > 0){
			DeathCountdownS.DC.ActivateCountdown(newDeathTime);
		}
	
	}


	public void ActivateDeath(){
		playerRef.TakeDamage(null, 99999f, Vector3.zero, 0.2f, true, true, true);
		Debug.Log("PLAYER DIE!");
	}
}
