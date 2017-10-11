using UnityEngine;
using System.Collections;

public class RankManagerS : MonoBehaviour {

	[HideInInspector]
	public int totalRank = 0;
	private int currentRankAdd;
	private int currentMultiplier;


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
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
}
