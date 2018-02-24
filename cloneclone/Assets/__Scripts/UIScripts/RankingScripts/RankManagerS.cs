using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankManagerS : MonoBehaviour {

	[HideInInspector]
	public int totalRank = 0;
	private int rankCountUp = 0;
	public int currentCombo {get { return currentRankAdd; } }
	private int rankAtCountStart = 0;
	private int currentRankAdd;
	private int currentMultiplier;

	[HideInInspector]
	public RankUIS myUI;

	[Header("Scoring Properties")]
	public int[] scoreTypeAmts;
	private float delayCountUpTime = 0.8f;
	private float delayCountUp;

	[Header("Multiplier Properties")]
	public int[] multiplierStages;
	public float[] dmgToAdvanceMultipliers;
	public float[] dmgAdvanceReductionPenalties; // how much currentDmgAdvance to take away on hit at current stage
	private int currentMultiplierStage = 0;
	public int currentMultStage { get { return currentMultiplierStage; } }
	private float currentDmgAdvance = 0;
	private float timeSinceDealingDmg = 0;
	private float currentMultiplierDecreaseRate =0;

	private float[] diffComboMultipliers = new float[4]{0.9f, 1f, 1.05f, 1f};

	[Header("Multiplier Reduction Properties")]
	public float[] timeForReductionPenalties;
	public float[] reductionPenalties;
	private int currentReductionState = 0;

	private bool _scoringActive = false;
	public bool scoringActive {get {return _scoringActive; } }
	private bool _addingScore = false;
	public bool addingScore { get { return _addingScore;} }
	private bool _countingUp = false;
	private float countUpTime = 0.8f;
	private float countUpCount = 0f;
	private float countUpT;

	//_____________________________BONUS PROPERTIES
	public bool disableInScene = false;
	private bool _noDamage=true;
	public bool noDamage { get { return _noDamage; } }
	private int noDamageBonus = 1000;
	private bool _underTime = true;
	public bool underTime {get { return _underTime; } }
	private float combatDuration;

	private int scoreOnReset = 0;

	private int goalTimeInSeconds;
	private int timeBonus = 1000;
	private List<int> rankScoreTargets;

	private int currentCombatID = -1;
	private List<int> savedCombatIDs = new List<int>();
	private CombatManagerS _finalCombatManager;
	public CombatManagerS finalCombatManager { get {return _finalCombatManager; } }

	private bool _initialized = false;

	public static RankManagerS R;

	public static bool rankEnabled = false;
	private bool endScoringAfterCount = false;
	private bool stopScoring = false;
	private bool doNotResetNoDamage = false;

	private InGameMenuManagerS pauseRef;
	[HideInInspector]
	public bool delayLoad = false;

	void Awake(){
		if (R == null){
			R = this;
			Initialize();
		}else{
			enabled  = false;
		}
	}
	
	// Update is called once per frame
	void Update () {

		#if UNITY_EDITOR_OSX
		if (Input.GetKeyDown(KeyCode.Alpha9)){
			rankEnabled = !rankEnabled;
			if (rankEnabled){
				Debug.Log("Scoring is Active!");
			}else{
				Debug.Log("Scoring is Disabled!");
			}
		}
		#endif

		if (_scoringActive && !pauseRef.isPaused){
			combatDuration+=Time.deltaTime;
			if (delayCountUp > 0){
				delayCountUp -= Time.deltaTime;
			}
			else if (_countingUp){
				countUpCount += Time.deltaTime;
				if (countUpCount >= countUpTime){
					countUpCount = countUpTime;
					_countingUp = false;
					if (endScoringAfterCount){
						endScoringAfterCount  = false;
						myUI.EndCombat();
					}
				}
				countUpT = countUpCount/countUpTime;
				totalRank = Mathf.RoundToInt(Mathf.Lerp(rankAtCountStart, rankCountUp, countUpT));
				myUI.UpdateCurrentScore();
			}

			if (currentDmgAdvance > 0){
			if (currentReductionState < timeForReductionPenalties.Length){
			timeSinceDealingDmg += Time.deltaTime;
			if (timeSinceDealingDmg >= timeForReductionPenalties[currentReductionState] && currentReductionState < timeForReductionPenalties.Length){
				currentReductionState++;
			}
			}
				if (currentReductionState > 0){
					currentDmgAdvance-=reductionPenalties[currentReductionState-1]*Time.deltaTime;

					if (currentDmgAdvance <= 0){
						currentDmgAdvance = 0;
						currentReductionState = 0;
						EndCombo();
						_finalCombatManager.SendComboBreakMessage(true);
					}

					myUI.UpdateMultBar();
				}
			}


		}
	
	}

	void Initialize() {
		if (!_initialized && !disableInScene){
			myUI = GameObject.Find("CombatRankUI").GetComponent<RankUIS>();
			pauseRef = GameObject.Find("Menus").GetComponent<InGameMenuManagerS>();
			myUI.Initialize(this);
			_initialized = true;
		}
	} 

	public void StartCombat(int targetTime, List<int> scores, int combatID, CombatManagerS finalCombat, bool continuation = false){

			currentCombatID = combatID;
		_finalCombatManager = finalCombat;
		if (!continuation){
			goalTimeInSeconds = targetTime;
			rankScoreTargets = scores;
		currentDmgAdvance = timeSinceDealingDmg = 0f;
		currentMultiplierStage = 0;
		currentMultiplier = multiplierStages[currentMultiplierStage];
		currentRankAdd = totalRank = 0;
			myUI.StartCombat();
			myUI.UpdateCurrentScore();
		_scoringActive = true;
			combatDuration = 0;
			_noDamage = true;
			_underTime = false;
		scoreOnReset = 0;
			doNotResetNoDamage = false;
			savedCombatIDs = new List<int>(){currentCombatID};
		}else{
			goalTimeInSeconds += targetTime;
			for (int i = 0; i < rankScoreTargets.Count; i++){
				rankScoreTargets[i] += scores[i];
			}
			savedCombatIDs.Add(currentCombatID);
		}
	}

	public void RestartCombat(){
		if (rankEnabled){
		currentMultiplierStage = 0;
		currentMultiplier = multiplierStages[currentMultiplierStage];
			rankAtCountStart = rankCountUp = 0;
			_countingUp = false;
			currentRankAdd = 0;
			totalRank = scoreOnReset;
			combatDuration = 0;
			if (!doNotResetNoDamage){
			_noDamage = true;
			}
			_underTime = false;
			myUI.ResetCombat();
		}
	}

	public void EndCombat(bool checkpoint = false, bool endVerse = false){
		if (rankEnabled){
			if (!checkpoint){
				EndCombo();
			if (goalTimeInSeconds >= combatDuration){
				_underTime = true;
			}else{
				_underTime = false;
			}
			myUI.doTimeBonus = _underTime;
			myUI.doNoDamage = _noDamage;
			endScoringAfterCount = true;
			}else{
				if (rankCountUp > totalRank){
				scoreOnReset = rankCountUp;
				}else{
					scoreOnReset = totalRank;
				}
				scoreOnReset+=currentRankAdd*currentMultiplier;
				if (!_noDamage){
					doNotResetNoDamage = true;
				}
			}
		}else{
			if (!endVerse){
				VerseDisplayS.V.EndVerse();
			}
		}
	}
	public void DieInCombat(){
		if (rankEnabled){
		stopScoring = true;
		EndCombo();
		myUI.FadeOut();
		}
	}
	public string ReturnRank(){
		string rankString = "C";
		if (rankEnabled){
			if (totalRank >= rankScoreTargets[2]*diffComboMultipliers[DifficultyS.GetSinInt()]){
			rankString = "S";
			}else if (totalRank >= rankScoreTargets[1]*diffComboMultipliers[DifficultyS.GetSinInt()]){
			rankString = "A";
			}else if (totalRank >= rankScoreTargets[0]*diffComboMultipliers[DifficultyS.GetSinInt()]){
			rankString = "B";
		}else{
			rankString = "C";
		}
		}
		return rankString;
	}
	public int GetRankInt(){
		int rankInt = 0;
		if (totalRank >= rankScoreTargets[2]*diffComboMultipliers[DifficultyS.GetSinInt()]){
			rankInt = 3;
		}else if (totalRank >= rankScoreTargets[1]*diffComboMultipliers[DifficultyS.GetSinInt()]){
		rankInt = 2;
		}else if (totalRank >= rankScoreTargets[0]*diffComboMultipliers[DifficultyS.GetSinInt()]){
			rankInt = 1;
		}
		return rankInt;
	}
	public void TakeHit(){
		if (rankEnabled && !stopScoring){
			_noDamage = false;
			if (currentDmgAdvance > 0){
			currentDmgAdvance -= dmgAdvanceReductionPenalties[currentMultiplierStage];
				myUI.UpdateMultBar();
			if (currentDmgAdvance <= 0){
				currentDmgAdvance = 0;
				EndCombo();
			}
			}
		}
	}
	public void EndScoring(){
		_scoringActive = false;
	}

	public void AddFinalScore(){
		for (int i = 0; i < savedCombatIDs.Count; i++){
			if (savedCombatIDs[i] > -1){
				PlayerInventoryS.I.dManager.AddClearedCombat(savedCombatIDs[i], totalRank, ReturnRank());

			}
		}
		delayLoad = false;
	}

	public void ScoreHit(int dmgType, float dmgAmount){
		if (rankEnabled && !stopScoring){
		currentRankAdd += scoreTypeAmts[dmgType];
			if (currentDmgAdvance < 0){
				currentDmgAdvance = 0;
			}
			currentReductionState = 0;
		currentDmgAdvance += dmgAmount;
			while (currentDmgAdvance > dmgToAdvanceMultipliers[currentMultiplierStage]){
			if (currentMultiplierStage < multiplierStages.Length-1){
					currentDmgAdvance -= dmgToAdvanceMultipliers[currentMultiplierStage];
				currentMultiplierStage++;
			}else{
				currentDmgAdvance = dmgToAdvanceMultipliers[currentMultiplierStage];
			}
		}
			currentMultiplier = multiplierStages[currentMultiplierStage];
			myUI.UpdateMultBar();
		timeSinceDealingDmg = 0f;
		currentReductionState = 0;
			myUI.AddScoreItem(dmgType, scoreTypeAmts[dmgType]);
		myUI.UpdateCurrentCombo();
		}

	}

	public void AddBonuses(){
		rankAtCountStart = totalRank;
		int totalBonus = 0;
		if (_noDamage){
			totalBonus += noDamageBonus;
		}
		_noDamage = true;
		if (_underTime){
			totalBonus += timeBonus;
		}
		_underTime = false;
		rankCountUp = rankAtCountStart+totalBonus;
		myUI.StartCountUp(totalBonus, false);
		_countingUp = true;
		countUpCount = 0f;
		delayCountUp = delayCountUpTime;
	}


	void EndCombo(){
		rankAtCountStart = totalRank;
		rankCountUp = rankAtCountStart+ currentRankAdd*currentMultiplier;
		myUI.StartCountUp(currentRankAdd*currentMultiplier);
		currentRankAdd = 0;
		currentMultiplier = 0;
		currentMultiplier = currentRankAdd = currentMultiplierStage = currentReductionState = 0;
		currentDmgAdvance = timeSinceDealingDmg = 0f;
		_countingUp = true;
		countUpCount = 0f;
		delayCountUp = delayCountUpTime;
	}

	public int TimeLeftInSeconds(){
		int timeLeft = 0;
		if (combatDuration < goalTimeInSeconds){
			timeLeft = Mathf.RoundToInt(goalTimeInSeconds-combatDuration);
		}else{
			timeLeft = 0;
		}
		return timeLeft;
	}

	public string CurrentMultiplierDisplay(){
		string currentMult = "x";
		currentMult += currentMultiplier;
		currentMult+=".0";
		return currentMult;

	}

	public float CurrentMultSize(){
		float multSize = currentDmgAdvance / dmgToAdvanceMultipliers[currentMultiplierStage];
		return multSize;
	}
		
}
