using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerLvDisplayS : MonoBehaviour {

	private PlayerStatsS playerStatRef;

	public Text hpStat;
	public Text stStat;
	public Text mnStat;
	public Text dfStat;
	public Text enStat;
	public Text abStat;

	private Color textStartColor;
	private Color highlightColor = Color.white;

	private bool _initialized = false;


	// Use this for initialization
	void Start () {

		Initialize();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void Initialize(bool doUpdate = true){
		if (!_initialized){
			_initialized = true;
			playerStatRef = GameObject.Find("Player").GetComponent<PlayerStatsS>();
			textStartColor = hpStat.color;
		}
		if (doUpdate){
			UpdateDisplays();
		}
	}

	public void UpdateDisplays(){

		if (!_initialized){
			Initialize(false);
		}

		hpStat.color = stStat.color = mnStat.color = dfStat.color = enStat.color = abStat.color = textStartColor;
		hpStat.text = playerStatRef.maxHealth.ToString();
		stStat.text = playerStatRef.maxMana.ToString();
		mnStat.text = (playerStatRef.maxCharge/10f).ToString();
		dfStat.text = playerStatRef.maxDefense.ToString();
		enStat.text = playerStatRef.currentRecoverRateLv.ToString();
		abStat.text = playerStatRef.currentChargeRecoverLv.ToString();

	}

	public void HighlightStat(int statToEdit){
		UpdateDisplays();
		switch (statToEdit){
		default: 
			break;
		case (0):
			hpStat.text = (playerStatRef.maxHealth+1).ToString();
			hpStat.color = highlightColor;
			break;
		case (1):
			stStat.text = (playerStatRef.maxMana+1).ToString();
			stStat.color = highlightColor;
			break;
		case (2):
			mnStat.text = (playerStatRef.maxCharge/10f+1).ToString();
			mnStat.color = highlightColor;
			break;
		case (3):
			dfStat.text = (playerStatRef.maxDefense+1).ToString();
			dfStat.color = highlightColor;
			break;
		case (4):
			enStat.text = (playerStatRef.currentRecoverRateLv+1).ToString();
			enStat.color = highlightColor;
			break;
		case (5):
			abStat.text = (playerStatRef.currentChargeRecoverLv+1).ToString();
			abStat.color = highlightColor;
			break;
		}
	}
}
