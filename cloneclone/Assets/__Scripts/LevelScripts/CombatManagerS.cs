using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatManagerS : MonoBehaviour {

	public enum CombatSpecialCondition {None, NoDamage, TimeLimit, OneCombo};
	public CombatSpecialCondition combatCondition = CombatSpecialCondition.None;
	private bool _failedSpecialCondition = false;
	public bool failedSpecialCondition { get { return _failedSpecialCondition; } }
	private ConditionUIS _myConditionUI;

	[Header("Activate Properties")]
	public int combatID = -1;
	public EnemySpawnerS[] enemies;
	public BarrierS[] barriers;
	public GameObject darknessHolder;
	int defeatedEnemies = 0;
    public bool effectOnStart = false;
    public bool resetEffectOnStart = false;
	public float delayEndEffect = 0f;

	PlayerController playerRef;
	public PlayerController pRef { get { return playerRef; } }
	private Vector3 _resetPos;
	public Vector3 resetPos { get { return _resetPos; } }

	private bool completed = false;
	private bool activated = false;
    public bool combatIsActive { get { return (activated && !completed); } }

	public ActivateOnCombatS turnOnAtStart;
	public GameObject[] turnOffOnEnd;
	public GameObject[] turnOnOnEnd;
	public ChangeCameraSizeOnTouch revertCameraSize;
	public InstantiateOnCombatRestartS restartSpawn;
	public bool doNotEndVerse = false;

	[Header("Fight Once Properties")]
	public bool onlyFightOnce = false;
	private bool correctedFightOnce = false;
	public GameObject[] turnOnOnSkip;
	public GameObject[] turnOffOnSkip;
	public bool clearBloodOnComplete = false;
	public bool turnOffEnemiesOnComplete = false;
    public CombatManagerS turnOffOtherEnemies;

	[Header("Special Player Properties")]
	public float changeParryRange = -1;
	public bool turnOffSlowing = false;
	public float newWeight = -1;
	public float enemyDetectionMult = 1f;

	[Header("Scoring Settings")]
	public bool turnOnScoring = false;
	public bool turnOffScoring = false;
	public bool allowRetry = false;
    public bool ignoreMarked = false;

	[Header("Infinite Properties")]
	public bool inInfiniteMode = false;
	public InfinityManagerS myInfiniteManager;
	public int minDifficulty = -1;
	public int maxDifficulty = 9999;
	public int overrideOnDifficulty = -1;
	public float geometryMultiplier = 1f;

	[Header("Scoring Properties")]
	public int[] targetTimesInSeconds = new int[]{25, 20, 15, 15};
	public List<int> rankThresholds = new List<int>(3){2000, 2500, 3000};
    public List<int> newGamePlusThresholds = new List<int>(3) { 12000, 27000, 45000 };
	public bool hasContinuation = false;
	public bool isContinuation = false;
    public float rankEndSpeedMult = 1f;
	
	// Update is called once per frame
	void Update () {

		if (!completed && activated){
			#if UNITY_EDITOR_OSX || UNITY_EDITOR || UNITY_EDITOR_64
			if (Input.GetKeyDown(KeyCode.Minus)){
				HurtAllEnemies();
			}
			#endif
			if (combatCondition == CombatSpecialCondition.TimeLimit && !_failedSpecialCondition){
				_myConditionUI.ReplaceTimeString(RankManagerS.R.TimeLeftInSeconds().ToString());
				if (RankManagerS.R.TimeLeftInSeconds() <= 0){
					_myConditionUI.FailCondition();
					_failedSpecialCondition = true;
				}
			}
			CheckForCompletion();
		}
	
	}

	void HurtAllEnemies(){
		for (int i = 0; i < enemies.Length; i++){
			if (enemies[i].enemySpawned){
				if (enemies[i].currentSpawnedEnemy.GetPlayerReference() != null){
					enemies[i].currentSpawnedEnemy.TakeDamage(enemies[i].currentSpawnedEnemy.transform, Vector3.zero, 9999f, 1f, 1f, 0, 0f, 0f);
				}
			}
		}
	}

	void CheckForCompletion(){

		if (!playerRef.myStats.PlayerIsDead()){
			defeatedEnemies = 0;
			for (int i = 0; i <  enemies.Length; i++){
				if (enemies[i].EnemiesDefeated()){
					defeatedEnemies++;
				}
			}
			if (defeatedEnemies >= enemies.Length && !completed){
				StartCoroutine(CompleteCombat());
			}
		}

	}

	public void CheckCondition(){
		if (combatCondition != CombatSpecialCondition.None){
				if (!_failedSpecialCondition){
				_myConditionUI.SuccessCondition();
				PlayerInventoryS.I.dManager.AddSpecialConditionCompleteID(combatID);
				}else{
					_myConditionUI.FadeOut();
					_failedSpecialCondition = false;
				}
		}
	}

	public void SetWitchTime(bool newTime){
		for (int i = 0; i < enemies.Length; i++){
			enemies[i].SetWitchTime(newTime);
		}
	}

	IEnumerator CompleteCombat(){
		AddDefeatedEnemies();
		CameraFollowS.F.ClearStunnedEnemies();
		CameraShakeS.C.TimeSleepEndCombat(0.3f);
		completed = true;
		yield return new WaitForSeconds(0.2f);
		CameraEffectsS.E.ResetSound();
		yield return new WaitForSeconds(0.1f+delayEndEffect);
		foreach (BarrierS b in barriers){
			b.TurnOff();
		}
		for (int i = 0; i <  enemies.Length; i++){
			enemies[i].DropOnDefeat();
		}
		playerRef.SetCombat(false);
		playerRef.EndWitchTime();
		CameraEffectsS.E.ResetEffect(true);
		playerRef.DeactivateTransform();
		if (combatID > -1 && !RankManagerS.rankEnabled){
			PlayerInventoryS.I.dManager.AddClearedCombat(combatID, -1, RankManagerS.R.ReturnRank());
		}
		RankManagerS.R.EndCombat(hasContinuation, doNotEndVerse);
		TurnOffEnemies();
		TurnOnObjects();
		TurnOffObjects();

		if (inInfiniteMode){
			myInfiniteManager.AddCompletedFight();
		}
		if (clearBloodOnComplete){
			PlayerInventoryS.I.dManager.ClearBattleBlood();
		}
		if (turnOffEnemiesOnComplete){
			for (int i = 0; i < enemies.Length; i++){
				if (enemies[i].currentSpawnedEnemy){
					enemies[i].currentSpawnedEnemy.gameObject.SetActive(false);
				}
			}
		}

		RetryFightUI.allowRetry = false;
		if (revertCameraSize){
			revertCameraSize.CombatEndRevertSize();
		}
	}

	void AddDefeatedEnemies(){
		for (int i = 0; i < enemies.Length; i++){
			enemies[i].SaveEnemyDefeated();
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
        if (turnOffOtherEnemies != null){
            for (int i = 0; i < turnOffOtherEnemies.enemies.Length; i++)
            {
                if (turnOffOtherEnemies.enemies[i].currentSpawnedEnemy)
                {
                    turnOffOtherEnemies.enemies[i].currentSpawnedEnemy.gameObject.SetActive(false);
                }
            }
        }

		RetryFightUI.allowRetry = allowRetry;

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

        if (PlayerAugmentsS.MARKED_AUG && newGamePlusThresholds.Count >= 3){
            rankThresholds = newGamePlusThresholds;
        }

        if (turnOnScoring || (PlayerAugmentsS.MARKED_AUG && !ignoreMarked)){
			RankManagerS.rankEnabled = true;
		}
        if (turnOffScoring && (ignoreMarked || !PlayerAugmentsS.MARKED_AUG)){
			RankManagerS.rankEnabled = false;
		}

		playerRef.playerAug.canUseUnstoppable = true;
		playerRef.myStats.GoToUnstoppableHealth();
		if (itemReset){
			if (restartSpawn){
				restartSpawn.SpawnOnRestart();
			}

			playerRef.EndWitchTime(false, true);
			if (playerRef.myBuddy != null){
				Vector3 buddyPos = playerRef.myBuddy.transform.position-playerRef.transform.position;
				playerRef.myBuddy.transform.position = _resetPos+buddyPos;
			}
			playerRef.transform.position = _resetPos;
			RankManagerS.R.RestartCombat();

		}else{
			if (RankManagerS.rankEnabled){
				RankManagerS.R.StartCombat(targetTimesInSeconds[DifficultyS.GetSinInt()], rankThresholds, combatID, this, isContinuation);
			}
			if (effectOnStart){
                CameraEffectsS.E.ResetEffect(false, !resetEffectOnStart);
			}
			if (inInfiniteMode){
				myInfiniteManager.SetGeometrySize(geometryMultiplier);	
			}
			if (clearBloodOnComplete){
				PlayerInventoryS.I.dManager.SetBattleBlood();
			}
			playerRef.ChangeParryRange(changeParryRange, turnOffSlowing, enemyDetectionMult);
		}

		if (combatCondition != CombatSpecialCondition.None){
			_failedSpecialCondition = false;
			if (!_myConditionUI){
				_myConditionUI = GameObject.Find("ConditionUI").GetComponent<ConditionUIS>();
				_myConditionUI.TurnOnAll(combatCondition);
			}
			SentHurtMessage(false);
			SendComboBreakMessage(false);
		}

		ChangeFeatherCols(playerRef.EquippedWeapon().swapColor);
		CameraPOIS.POI.ChangeEnemyWeight(newWeight);
		
	}
	void TurnOnObjects(){
		if (turnOnOnEnd != null){
			for (int i = 0; i < turnOnOnEnd.Length; i++){
				turnOnOnEnd[i].SetActive(true);
				if (turnOnOnEnd[i].GetComponent<MatchOnCombatActivate>()){
					turnOnOnEnd[i].GetComponent<MatchOnCombatActivate>().MatchTargetPos();
				}
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

	public void TurnOnOnceObjects(){
		if (turnOnOnSkip != null){
			for (int i = 0; i < turnOnOnSkip.Length; i++){
				turnOnOnSkip[i].SetActive(true);
			}
		}
	}
	
	public void TurnOffOnceObjects(){
		if (turnOffOnSkip != null){
			for (int i = 0; i < turnOffOnSkip.Length; i++){
				turnOffOnSkip[i].SetActive(false);
			}
		}
	}

	public void SentHurtMessage(bool newH){
		if (newH){
			if (combatCondition == CombatSpecialCondition.NoDamage){
				_failedSpecialCondition = true;
				_myConditionUI.FailCondition();
			}
		}else{
			if (combatCondition == CombatSpecialCondition.NoDamage){
				_failedSpecialCondition = false;
				_myConditionUI.TurnOnAll(combatCondition);
			}
		}
	}

	public void SendComboBreakMessage(bool newC){
		if (newC){
			if (combatCondition == CombatSpecialCondition.OneCombo){
				_failedSpecialCondition = true;
				_myConditionUI.FailCondition();
			}
		}else{
			if (combatCondition == CombatSpecialCondition.OneCombo){
				_failedSpecialCondition = false;
				_myConditionUI.TurnOnAll(combatCondition);
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
			//Debug.Log("Fight already completed! ID" + combatID + " " +PlayerInventoryS.I.dManager.combatClearedAtLeastOnce.Contains(combatID));
			return false;
		}
	}

	void TurnOffEnemies(){
		for (int i = 0; i < enemies.Length; i++){
			if (enemies[i].enemySpawned && enemies[i].currentSpawnedEnemy != null){
				enemies[i].currentSpawnedEnemy.enabled = false;
			}
		}
	}

	public void ChangeFeatherCols(Color newCol){
		for (int i = 0; i < enemies.Length; i++){
			if (enemies[i].currentSpawnedEnemy != null){
				enemies[i].currentSpawnedEnemy.ChangeFeatherColor(newCol);
			}
		}
	}

	public bool CheckDifficulty(int check){
		if (check == overrideOnDifficulty){
			return true;
		}
		else if (check >= minDifficulty && check <= maxDifficulty){
			return true;
		}else{
			return false;
		}
	}

	public bool GetNotAlone(EnemySpawnerS currentEnemy){
		bool amIAlone = true;
		if (enemies.Length > 1){
			for (int i = 0; i < enemies.Length; i++){
				if (enemies[i] != currentEnemy && enemies[i].currentSpawnedEnemy != null && amIAlone){
					if (!enemies[i].currentSpawnedEnemy.isDead){
						amIAlone = false;
					}
				}
			}
		}

		return amIAlone;
	}

	public EnemyS GetCrowdPoint(EnemySpawnerS currentEnemy){
		List<EnemyS> possibleEnemies = new List<EnemyS>();
		for (int i = 0; i < enemies.Length; i++){
			if (enemies[i] != currentEnemy && enemies[i].currentSpawnedEnemy){
				if (!enemies[i].currentSpawnedEnemy.isDead){
					possibleEnemies.Add(enemies[i].currentSpawnedEnemy);
				}
			}
		}
		if (possibleEnemies.Count > 1){
			return possibleEnemies[(Mathf.FloorToInt(Random.Range(0, possibleEnemies.Count)))];
		}else{
			return possibleEnemies[0];
		}
	}

}
