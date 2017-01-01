using UnityEngine;
using System.Collections;

public class CombatManagerS : MonoBehaviour {

	public int combatID = -1;
	public EnemySpawnerS[] enemies;
	public BarrierS[] barriers;
	public GameObject darknessHolder;
	int defeatedEnemies = 0;

	PlayerController playerRef;
	private Vector3 _resetPos;
	public Vector3 resetPos { get { return _resetPos; } }

	private bool completed = false;
	private bool activated = false;

	public GameObject[] turnOffOnEnd;
	public GameObject[] turnOnOnEnd;
	
	// Update is called once per frame
	void Update () {

		if (!completed && activated){
			CheckForCompletion();
		}
	
	}

	void CheckForCompletion(){

		defeatedEnemies = 0;
		foreach (EnemySpawnerS e in enemies){
			if (e.EnemiesDefeated()){
				defeatedEnemies++;
			}
		}
		if (defeatedEnemies >= enemies.Length){
			CompleteCombat();
		}

	}

	void CompleteCombat(){
		foreach (BarrierS b in barriers){
			b.TurnOff();
		}
		playerRef.SetCombat(false);
		completed = true;
		CameraEffectsS.E.ResetEffect(true);
		VerseDisplayS.V.EndVerse();
		if (combatID > -1){
			PlayerInventoryS.I.dManager.AddClearedCombat(combatID);
		}
		TurnOnObjects();
		TurnOffObjects();
	}

	public void SetPlayerRef(PlayerController p){
		activated = true;
		playerRef = p;
		_resetPos = playerRef.transform.position;
		playerRef.SetCombat(true);
		Initialize();
	}

	public void Initialize(bool itemReset = false){
		foreach (EnemySpawnerS e in enemies){
			if (e.gameObject.activeSelf){
				e.RespawnEnemies();
			}else{
				e.gameObject.SetActive(true);
			}
		}

		foreach (BarrierS b in barriers){
			b.gameObject.SetActive(true);
		}

		if (itemReset){
			if (playerRef.myBuddy != null){
				Vector3 buddyPos = playerRef.myBuddy.transform.position-playerRef.transform.position;
				playerRef.myBuddy.transform.position = _resetPos+buddyPos;
			}
			playerRef.transform.position = _resetPos;

		}
	}
	void TurnOnObjects(){
		if (turnOnOnEnd != null){
			for (int i = 0; i < turnOnOnEnd.Length; i++){
				turnOnOnEnd[i].SetActive(true);
			}
		}
	}

	void TurnOffObjects(){
		if (turnOffOnEnd != null){
			for (int i = 0; i < turnOffOnEnd.Length; i++){
				turnOffOnEnd[i].SetActive(false);
			}
		}
	}
}
