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

	private bool _initialized = false;

	public static RankManagerS R;

	public static bool rankEnabled = false;

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

		if (_scoringActive){
			if (delayCountUp > 0){
				delayCountUp -= Time.deltaTime;
			}
			else if (_countingUp){
				countUpCount += Time.deltaTime;
				if (countUpCount >= countUpTime){
					countUpCount = countUpTime;
					_countingUp = false;
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
			myUI.Initialize(this);
			_initialized = true;
		}
	} 

	public void StartCombat(){

		if (rankEnabled){
		currentMultiplierStage = 0;
		currentMultiplier = multiplierStages[currentMultiplierStage];
		currentRankAdd = totalRank = 0;
			myUI.StartCombat();
			myUI.UpdateCurrentScore();
		_scoringActive = true;
		}
		
	}

	public void RestartCombat(){
		if (rankEnabled){
		currentMultiplierStage = 0;
		currentMultiplier = multiplierStages[currentMultiplierStage];
		currentRankAdd = totalRank = 0;
		}
	}

	public void EndCombat(){
		if (rankEnabled){
		EndCombo();
		_scoringActive = false;
		}
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

	public int CombatRank(){
		int combatRank = 0;
		if (rankEnabled){
			combatRank = Mathf.RoundToInt(currentRankAdd*currentMultiplier);
			currentRankAdd = currentMultiplier = 0;
		}else{
			combatRank = -1;
		}
		return combatRank;
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
