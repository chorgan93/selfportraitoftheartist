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
	public int maxScoreObjs = 4;

	private RankManagerS myRankManager;

	[Header("VisualProperties")]
	public Image[] totalRankBorders;
	public Image multiplierBar;
	private RectTransform multBarTransform;
	private Vector2 multBarSize;
	private float maxBarSize;
	public Color[] multiplierColors;
	public string[] multiplierColorStrings;
	private List<float> imageMaxAlphas = new List<float>();
	public Text totalRankText;
	public Text currentComboText;
	public GameObject currentComboObj;
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
	public Vector2 endComboPosition;
	public float yItemSeparation;
	public float xItemMoveDist = 50f;
	private Vector2 scoreAddStartPos;
	private Vector2 scoreAddEndPos;



	private bool _initialized = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if (_initialized){
			if (myRankManager.scoringActive){
				if (fadingIn || fadingOut){
					fadeCount += Time.deltaTime;
					if (fadingIn){
					if (fadeCount >= fadeInTime){
							fadeCount = fadeInTime;
					}
					fadeT = fadeCount/fadeInTime;
					}else if (fadingOut){
						if (fadeCount >= fadeOutTime){
							fadeCount = fadeOutTime;
						}
						fadeT = fadeCount/fadeOutTime;
					}
					fadeT = Mathf.Sin(fadeT * Mathf.PI * 0.5f);
					for (int i = 0; i < totalRankBorders.Length; i++){
						fadeCol = totalRankBorders[i].color;
						if (fadingIn){
							fadeCol.a = imageMaxAlphas[i]*fadeT;
						}else if (fadingOut){
							fadeCol.a = imageMaxAlphas[i]/fadeT;
						}
						totalRankBorders[i].color = fadeCol;
					}
					fadeCol = multiplierColors[myRankManager.currentMultStage];
					if (fadingIn){
						fadeCol.a = fadeT;
					}else if (fadingOut){
						fadeCol.a = 1f/fadeT;
					}
					multiplierBar.color = fadeCol;
					fadeCol = totalRankText.color;
					if (fadingIn){
						fadeCol.a = fadeT*rankTextMaxAlpha;
					}else if (fadingOut){
						fadeCol.a = 1f/fadeT*rankTextMaxAlpha;
					}
					if (fadeT >= 1f){
						fadingIn = false;
						fadingOut = false;
					}
				}
			}
		}
	
	}

				
	public void Initialize(RankManagerS myRank){
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
				scoreAddStartPos = addScoreItem00.GetComponent<RectTransform>().anchoredPosition;
				scoreAddEndPos = scoreAddStartPos;
				scoreAddEndPos.y = 0f;

			}
			multBarTransform = multiplierBar.GetComponent<RectTransform>();
			maxBarSize = multBarTransform.sizeDelta.x;
			multBarSize = multBarTransform.sizeDelta;
			multBarSize.x = 0;
			rankTextMaxAlpha = totalRankText.color.a;
			fadeCol = totalRankText.color;
			totalRankText.color = fadeCol; 
		}
	}

	public void AddScoreItem(int dmgType, float dmgAmount){
		// ui management
		if (activeScoreObjs.Count >= maxScoreObjs){
			MoveTopItemOff();
		}
		if (inactiveScoreObjs.Count > 0){
			inactiveScoreObjs[0].TurnOn(dmgType, dmgAmount, this);
			activeScoreObjs.Add(inactiveScoreObjs[0]);
			inactiveScoreObjs.RemoveAt(0);
		}else{
			RankUIItemS newItem = Instantiate(scoreObjRef, scoreObjRef.transform.position, Quaternion.identity)
				as RankUIItemS;
			newItem.TurnOn(dmgType, dmgAmount, this);
			activeScoreObjs.Add(newItem);
		}

		if (activeScoreObjs.Count > 1){
		Vector2 scoreItemTargetPos = topAnchoredPosition;
		scoreItemTargetPos.y -= yItemSeparation*(activeScoreObjs.Count-1);
		Vector2 scoreItemStartPos = scoreItemTargetPos;
		scoreItemStartPos.y -= xItemMoveDist/3f;
		activeScoreObjs[activeScoreObjs.Count-1].transform.parent = scoreObjRef.transform.parent;
		activeScoreObjs[activeScoreObjs.Count-1].SetPosition(scoreItemStartPos);
		activeScoreObjs[activeScoreObjs.Count-1].SetNewPos(true, false, scoreItemTargetPos);
		}else{
			activeScoreObjs[0].transform.parent = scoreObjRef.transform.parent;
			activeScoreObjs[0].SetPosition(topAnchoredPosition);
			activeScoreObjs[0].SetFade(true, false);
			}

	}

	void MoveTopItemOff(){
		Vector2 scoreTargetPos = Vector2.zero;
		for (int i = 0; i < activeScoreObjs.Count; i++){
			scoreTargetPos = topAnchoredPosition;
			if (i == 0){
				scoreTargetPos.x += xItemMoveDist;
				activeScoreObjs[i].SetNewPos(false, true, scoreTargetPos);
			}else{
				scoreTargetPos.y -= yItemSeparation*(i-1);
				activeScoreObjs[i].MoveTo(scoreTargetPos, 0.3f);
			}
		}
		activeScoreObjs.RemoveAt(0);
	}

	void MoveAllItemsToCombo(){
		Vector2 scoreTargetPos = endComboPosition;
		for (int i = 0; i < activeScoreObjs.Count; i++){
				activeScoreObjs[i].SetNewPos(false, true, scoreTargetPos);
			
		}
		activeScoreObjs.Clear();
	}

	public void StartCombat(){

		fadeCount = 0;
		fadingIn = true;
		UpdateCurrentCombo();
		UpdateCurrentScore();
		UpdateMultBar();
		
	}

	public void UpdateMultBar(){
		if (!fadingIn && !fadingOut){
			fadeCol = multiplierColors[myRankManager.currentMultStage];
		}
		multBarSize.x = maxBarSize*myRankManager.CurrentMultSize();
		multBarTransform.sizeDelta = multBarSize;
	}

	public void UpdateCurrentCombo(){
		if (myRankManager.currentCombo > 0){
		currentComboText.text = "+ " + myRankManager.currentCombo + " <color=" + multiplierColorStrings[myRankManager.currentMultStage].ToString() + ">" 
			+ myRankManager.CurrentMultiplierDisplay() + "</color>";
		currentComboObj.gameObject.SetActive(true);
		}else{
			currentComboObj.gameObject.SetActive(false);
		}
	}
	public void UpdateCurrentScore(){
		totalRankText.text = myRankManager.totalRank.ToString();
	}

	public void EndCurrentCombo(){
		currentComboText.text = "";
		currentComboObj.gameObject.SetActive(false);
		MoveAllItemsToCombo();
	}

	public void EndCombat(){
		StartCoroutine(EndCombatDisplay());
	}

	IEnumerator EndCombatDisplay(){
		yield return null;
	}

	public void StartCountUp(int countAmt){
		EndCurrentCombo();
		currentAddScoreItem++;
		fadingOut = true;
		fadeCount = 0f;
		if (currentAddScoreItem > 1){
			currentAddScoreItem = 0;
		}
		if (currentAddScoreItem == 0){
			addScoreItem00.TurnOn(-1, countAmt, this);
			addScoreItem00.SetPosition(scoreAddStartPos);
			addScoreItem00.SetMaxAlpha();
			addScoreItem00.SetNewPos(false, true, Vector2.zero, 0.8f);
		}else{
			addScoreItem01.TurnOn(-1, countAmt, this);
			addScoreItem01.SetPosition(scoreAddStartPos);
			addScoreItem01.SetMaxAlpha();
			addScoreItem01.SetNewPos(false, true, Vector2.zero, 0.8f);
		}
	}

	public void AddOffItem(RankUIItemS newItem){
		inactiveScoreObjs.Add(newItem);
	}
}
