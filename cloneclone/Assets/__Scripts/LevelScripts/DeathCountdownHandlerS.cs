using UnityEngine;
using System.Collections;

public class DeathCountdownHandlerS : MonoBehaviour {

	public PlayerStatsS playerRef;
	public float newDeathTime = -1f;
	public bool endTimer = false;
	private float timerAtStart;

	// Use this for initialization
	void Start () {

		DeathCountdownS.DC.currentHandler = this;
		if (endTimer){
			DeathCountdownS.DC.TurnOffCountdown();
		}
		else if (newDeathTime > 0){
			DeathCountdownS.DC.ActivateCountdown(newDeathTime);
		}
		timerAtStart = DeathCountdownS.deathCountdown;
	
	}

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)){
            // test kill Death countdown!!
            DeathCountdownS.DC.ActivateCountdown(1f);
        }
    }
#endif


	public void ActivateDeath(){
		playerRef.TakeDamage(null, 99999f, Vector3.zero, 0.2f, true, true, true);
		Debug.Log("PLAYER DIE!");
		DeathCountdownS.deathCountdown = timerAtStart;
	}
}
