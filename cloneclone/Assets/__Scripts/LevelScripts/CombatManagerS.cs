﻿using UnityEngine;
using System.Collections;

public class CombatManagerS : MonoBehaviour {

	public int combatID = -1;
	public EnemySpawnerS[] enemies;
	public BarrierS[] barriers;
	public GameObject darknessHolder;
	int defeatedEnemies = 0;

	PlayerController playerRef;
	public PlayerController pRef { get { return playerRef; } }
	private Vector3 _resetPos;
	public Vector3 resetPos { get { return _resetPos; } }

	private bool completed = false;
	private bool activated = false;

	public ActivateOnCombatS turnOnAtStart;
	public GameObject[] turnOffOnEnd;
	public GameObject[] turnOnOnEnd;

	[Header("Fight Once Properties")]
	public bool onlyFightOnce = false;
	private bool correctedFightOnce = false;
	public GameObject[] turnOnOnSkip;
	public GameObject[] turnOffOnSkip;
	public bool clearBloodOnComplete = false;
	
	// Update is called once per frame
	void Update () {

		if (!completed && activated){
			CheckForCompletion();
		}
	
	}

	void CheckForCompletion(){

		if (!playerRef.myStats.PlayerIsDead()){
			defeatedEnemies = 0;
			foreach (EnemySpawnerS e in enemies){
				if (e.EnemiesDefeated()){
					defeatedEnemies++;
				}
			}
			if (defeatedEnemies >= enemies.Length && !completed){
				StartCoroutine(CompleteCombat());
			}
		}

	}

	IEnumerator CompleteCombat(){
		CameraFollowS.F.ClearStunnedEnemies();
		CameraShakeS.C.TimeSleepEndCombat(0.12f);
		completed = true;
		CameraEffectsS.E.ResetSound();
		yield return new WaitForSeconds(0.2f);
		foreach (BarrierS b in barriers){
			b.TurnOff();
		}
		playerRef.SetCombat(false);
		CameraEffectsS.E.ResetEffect(true);
		VerseDisplayS.V.EndVerse();
		if (combatID > -1){
			PlayerInventoryS.I.dManager.AddClearedCombat(combatID);
		}
		TurnOffEnemies();
		TurnOnObjects();
		TurnOffObjects();

		if (clearBloodOnComplete){
			PlayerInventoryS.I.dManager.ClearBattleBlood();
		}
	}

	public void SetPlayerRef(PlayerController p){
		activated = true;
		playerRef = p;
		_resetPos = playerRef.transform.position;
		playerRef.SetCombat(true);
		Initialize();
	}

	public void Initialize(bool itemReset = false){

		if (turnOnAtStart != null){
			turnOnAtStart.Activate();
		}

		foreach (EnemySpawnerS e in enemies){
			e.myManager = this;
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

		}else{
			if (clearBloodOnComplete){
				PlayerInventoryS.I.dManager.SetBattleBlood();
			}
		}

		ChangeFeatherCols(playerRef.EquippedWeapon().swapColor);
		
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

	void TurnOnOnceObjects(){
		if (turnOnOnSkip != null){
			for (int i = 0; i < turnOnOnSkip.Length; i++){
				turnOnOnSkip[i].SetActive(true);
			}
		}
	}
	
	void TurnOffOnceObjects(){
		if (turnOffOnSkip != null){
			for (int i = 0; i < turnOffOnSkip.Length; i++){
				turnOffOnSkip[i].SetActive(false);
			}
		}
	}

	void CorrectCompletedFight(){
		if (darknessHolder != null){
			darknessHolder.gameObject.SetActive(true);
		}
		for (int i = 0; i < barriers.Length; i++){
			barriers[i].gameObject.SetActive(false);
		}
		TurnOnOnceObjects();
		TurnOffOnceObjects();
		correctedFightOnce = true;
	}

	public bool AllowCombat(){
		if (PlayerInventoryS.I.dManager.combatClearedAtLeastOnce == null){
			return true;
		}
		else if (!onlyFightOnce || (onlyFightOnce && !PlayerInventoryS.I.dManager.combatClearedAtLeastOnce.Contains(combatID))){
			return true;
		}else{
			if (!correctedFightOnce){
				CorrectCompletedFight();
			}
			return false;
		}
	}

	void TurnOffEnemies(){
		for (int i = 0; i < enemies.Length; i++){
			enemies[i].currentSpawnedEnemy.enabled = false;
		}
	}

	public void ChangeFeatherCols(Color newCol){
		for (int i = 0; i < enemies.Length; i++){
			if (enemies[i].currentSpawnedEnemy != null){
				enemies[i].currentSpawnedEnemy.ChangeFeatherColor(newCol);
			}
		}
	}
}
