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
	private bool _noDamage=true;
	public bool noDamage { get { return _noDamage; } }
	private int noDamageBonus = 1000;
	private bool _underTime = true;
	public bool underTime {get { return _underTime; } }
	private float combatDuration;

	private int goalTimeInSeconds;
	private int timeBonus = 1000;
	private List<int> rankScoreTargets;

	private int currentCombatID = -1;

	private bool _initialized = false;

	public static RankManagerS R;

	public static bool rankEnabled = false;
	private bool endScoringAfterCount = false;

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
					}

					myUI.UpdateMultBar();
				}
			}


		}
	
	}

	void Initialize() {
		if (!_initialized){
			myUI = GameObject.Find("CombatRankUI").GetComponent<RankUIS>();
			pauseRef = GameObject.Find("Menus").GetComponent<InGameMenuManagerS>();
			myUI.Initialize(this);
			_initialized = true;
		}
	} 

	public void StartCombat(int targetTime, List<int> scores, int combatID){

			currentCombatID = combatID;
			goalTimeInSeconds = targetTime;
			rankScoreTargets = scores;
		currentMultiplierStage = 0;
		currentMultiplier = multiplierStages[currentMultiplierStage];
		currentRankAdd = totalRank = 0;
			myUI.StartCombat();
			myUI.UpdateCurrentScore();
		_scoringActive = true;
			combatDuration = 0;
			_noDamage = true;
			_underTime = false;
	}

	public void RestartCombat(){
		if (rankEnabled){
		currentMultiplierStage = 0;
		currentMultiplier = multiplierStages[currentMultiplierStage];
			currentRankAdd = totalRank = 0;
			combatDuration = 0;
			_noDamage = true;
			_underTime = false;
		}
	}

	public void EndCombat(){
		if (rankEnabled){
		EndCombo();
			if (goalTimeInSeconds >= combatDuration){
				_underTime = true;
			}else{
				_underTime = false;
			}
			Debug.Log(_underTime + " : " + combatDuration.ToString() + " / " + goalTimeInSeconds.ToString());
			myUI.doTimeBonus = _underTime;
			myUI.doNoDamage = _noDamage;
			endScoringAfterCount = true;
		}else{

			VerseDisplayS.V.EndVerse();
		}
	}
	public string ReturnRank(){
		string rankString = "C";
		if (rankEnabled){
		if (totalRank >= rankScoreTargets[2]){
			rankString = "S";
		}else if (totalRank >= rankScoreTargets[1]){
			rankString = "A";
		}else if (totalRank >= rankScoreTargets[0]){
			rankString = "B";
		}else{
			rankString = "C";
		}
		}
		return rankString;
	}
	public int GetRankInt(){
		int rankInt = 0;
		if (totalRank >= rankScoreTargets[2]){
			rankInt = 3;
		}else if (totalRank >= rankScoreTargets[1]){
		rankInt = 2;
		}else if (totalRank >= rankScoreTargets[0]){
			rankInt = 1;
		}
		return rankInt;
	}
	public void TakeHit(){
		if (rankEnabled){
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
		if (currentCombatID > -1){
			PlayerInventoryS.I.dManager.AddClearedCombat(currentCombatID, totalRank, ReturnRank());
		}
		delayLoad = false;
	}

	public void ScoreHit(int dmgType, float dmgAmount){
		if (rankEnabled){
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
