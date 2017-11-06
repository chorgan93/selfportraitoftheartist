using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankManagerS : MonoBehaviour {

	[HideInInspector]
	public int totalRank = 0;
	private int rankCountUp = 0;
	public int currentCombo {get { return rankCountUp; } }
	private int rankAtCountStart = 0;
	private int currentRankAdd;
	private int currentMultiplier;

	private RankUIS myUI;

	[Header("Scoring Properties")]
	public int[] scoreTypeAmts;

	[Header("Multiplier Properties")]
	public int[] multiplierStages;
	public float[] dmgToAdvanceMultipliers;
	public float[] dmgAdvanceReductionPenalties; // how much currentDmgAdvance to take away on hit at current stage
	private int currentMultiplierStage = 0;
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
		}else{
			enabled  = false;
		}
	}

	// Use this for initialization
	void Start () {

		Initialized();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_scoringActive){
			if (_countingUp){
				countUpCount += Time.deltaTime;
				if (countUpCount >= countUpTime){
					countUpCount = countUpTime;
					_countingUp = false;
				}
				countUpT = countUpCount/countUpTime;
				totalRank = Mathf.RoundToInt(Mathf.Lerp(rankAtCountStart, rankCountUp, countUpT));
			}
		}
	
	}

	void Initialized() {
		if (!_initialized){
			_initialized = true;
			myUI = GetComponent<RankUIS>();
		}
	} 

	public void StartCombat(){

		currentMultiplierStage = 0;
		currentMultiplier = multiplierStages[currentMultiplierStage];
		currentRankAdd = totalRank = 0;
		
	}

	public void RestartCombat(){
		currentMultiplierStage = 0;
		currentMultiplier = multiplierStages[currentMultiplierStage];
		currentRankAdd = totalRank = 0;
	}

	public void ScoreHit(int dmgType, float dmgAmount){
		currentRankAdd += scoreTypeAmts[dmgType];
		currentDmgAdvance += dmgAmount;
		if (currentDmgAdvance >= multiplierStages[currentMultiplierStage]){
			if (currentMultiplierStage < multiplierStages.Length-1){
				currentDmgAdvance = dmgAmount-dmgToAdvanceMultipliers[currentMultiplierStage];
				currentMultiplierStage++;
			}else{
				currentDmgAdvance = dmgToAdvanceMultipliers[currentMultiplierStage];
			}
		}
		timeSinceDealingDmg = 0f;
		currentReductionState = 0;


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
}
