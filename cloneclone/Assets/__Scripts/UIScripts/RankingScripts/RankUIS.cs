using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class RankUIS : MonoBehaviour {



	[Header("UI Setup")]
	public RankUIItemS scoreObjRef;
	private List<RankUIItemS> activeScoreObjs;
	private List<RankUIItemS> inactiveScoreObjs;
	public RankUIItemS addScoreItem00;
	public RankUIItemS addScoreItem01;
	private int currentAddScoreItem = 0;
	public int maxScoreObjs = 3;

	private RankManagerS myRankManager;

	[Header("VisualProperties")]
	public Image[] totalRankBorders;
	private List<float> imageMaxAlphas = new List<float>();
	public Text totalRankText;
	public Text currentComboText;
	public Text currentMultiplierText;
	public Text rankLabelText;
	private float rankTextMaxAlpha;
	public float fadeInTime;
	public float showAfterRankTime;
	private float showAfterRankCountdown;
	public float fadeOutTime;

	private Color fadeCol;
	private bool fadingIn;
	private bool fadingOut;
	private float fadeT;
	private float fadeCount;


	[Header("Placement Properties")]
	public Vector2 topAnchoredPosition;
	public float yItemSeparation;



	private bool _initialized = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if (_initialized){
			if (myRankManager.scoringActive){
				
			}
		}
	
	}

				void Initialize(RankManagerS myRank){
		if (!_initialized){
						myRankManager = myRank;
			_initialized = true;
			scoreObjRef.gameObject.SetActive(false);
			activeScoreObjs = new List<RankUIItemS>();
			inactiveScoreObjs = new List<RankUIItemS>();
			for (int i = 0; i < totalRankBorders.Length; i++){
				imageMaxAlphas.Add(totalRankBorders[i].color.a);
				fadeCol = totalRankBorders[i].color;
				fadeCol.a = 0;
				totalRankBorders[i].color = fadeCol;
			}
			rankTextMaxAlpha = totalRankText.color.a;
			fadeCol = totalRankText.color;
			totalRankText.color = rankLabelText.color = fadeCol; 
		}
	}

	public void AddScoreItem(int dmgType, float dmgAmount){
		// ui management
		if (inactiveScoreObjs.Count > 0){
			inactiveScoreObjs[0].TurnOn(dmgType, dmgAmount);
			activeScoreObjs.Add(inactiveScoreObjs[0]);
			inactiveScoreObjs.RemoveAt(0);
		}else{
			RankUIItemS newItem = Instantiate(scoreObjRef, scoreObjRef.transform.position, Quaternion.identity)
				as RankUIItemS;
			newItem.TurnOn(dmgType, dmgAmount);
			activeScoreObjs.Add(newItem);
		}
	}

	public void StartCombat(){
		
	}

	public void UpdateCurrentCombo(){
		currentComboText.text = "+ " + myRankManager.currentCombo + " " + myRankManager.CurrentMultiplierDisplay();
	}
	public void UpdateCurrentScore(){
		totalRankText.text = myRankManager.totalRank.ToString();
	}

	public void EndCombat(){
		StartCoroutine(EndCombatDisplay());
	}

	IEnumerator EndCombatDisplay(){
		yield return null;
	}

	public void StartCountUp(int countAmt){
		currentAddScoreItem++;
		if (currentAddScoreItem > 1){
			currentAddScoreItem = 0;
		}
		if (currentAddScoreItem == 0){
			addScoreItem00.TurnOn(-1, countAmt);
		}else{
			addScoreItem01.TurnOn(-1, countAmt);
		}
	}
}
