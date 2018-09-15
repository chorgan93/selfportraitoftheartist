using UnityEngine;
using System.Collections;

public class InGameCinemaActivateS : MonoBehaviour {

	public float activateTime = -1f;
	public int myCinemaStep = 0;

	public GameObject[] onObjects;
	public GameObject[] offObjects;
	public EnemySpawnerS[] offEnemies;
	public PlayerController turnOffBuddy;

	public bool cleanUpBlood = false;

	// Use this for initialization
	void Start () {

		foreach (GameObject on in onObjects){
			if (on.GetComponent<MatchCinemaPositionS>() != null){
				on.transform.position = on.GetComponent<MatchCinemaPositionS>().GetTargetPos();
			}
			on.SetActive(true);
		}

		foreach (GameObject off in offObjects){
            if (off != null)
            {
                off.SetActive(false);
            }
		}

		if (offEnemies != null){
			for (int i = 0; i < offEnemies.Length; i++){
				if (offEnemies[i].currentSpawnedEnemy != null){
					offEnemies[i].currentSpawnedEnemy.gameObject.SetActive(false);
				}
			}
		}

		if (turnOffBuddy != null){
			if (turnOffBuddy.myBuddy != null){
				turnOffBuddy.myBuddy.gameObject.SetActive(false);
			}
		}

		if (cleanUpBlood){
			PlayerInventoryS.I.dManager.ClearBattleBlood();
		}
	
	}

}
