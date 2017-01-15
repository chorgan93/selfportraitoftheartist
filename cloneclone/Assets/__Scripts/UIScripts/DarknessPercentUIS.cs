using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DarknessPercentUIS : MonoBehaviour {

	[Header("Text Display")]
	public Text mainTextDisplay;
	public Text redTextDisplay;
	public Text purpleTextDisplay;
	Vector2 redTextOffset;
	Vector2 purpleTextOffset;
	private float redTextShakeCount;
	private float purpleTextShakeCount;

	private float displayAmt;
	private string displayString;

	[Header("Image Display")]
	public Image mainBarDisplay;
	public Image redBarDisplay;
	public Image purpleBarDisplay;
	private Vector2 maxBarSize;
	private Vector2 currentBarSize;
	private Vector2 altBarSize;

	[Header("Death Display")]
	public Text deathTextDisplay;
	public Text deathRedTextDisplay;
	private float saveDeathAmt;

	private PlayerStatsS pStats;

	private bool deathSequence;
	private bool fadeInDeathNumbers;
	private bool fadeOutDeathNumbers;

	private bool _allowAdvance = true;
	public bool allowAdvance { get { return _allowAdvance; } }

	// Use this for initialization
	void Start () {

		maxBarSize = altBarSize = currentBarSize = mainBarDisplay.rectTransform.sizeDelta;
		pStats = GameObject.Find("Player").GetComponent<PlayerStatsS>();
		_allowAdvance = true;
	
	}
	
	// Update is called once per frame
	void Update () {
	
		SetMainSize();
		SetText();

	}

	void SetMainSize(){

		currentBarSize = maxBarSize;
		currentBarSize.x *= pStats.currentDarkness/PlayerStatsS.DARKNESS_MAX;
		mainBarDisplay.rectTransform.sizeDelta = redBarDisplay.rectTransform.sizeDelta = currentBarSize;

	}

	void SetText(){

		displayAmt = pStats.currentDarkness/PlayerStatsS.DARKNESS_MAX*100f;
		if (displayAmt < 10){
			displayString = "0" + displayAmt.ToString("F2") + "%";
		}else{
			displayString = displayAmt.ToString("F2") + "%";
		}
		mainTextDisplay.text = redTextDisplay.text = displayString;
	}

	public void ActivateDeathCountUp(){
		saveDeathAmt = pStats.currentDarkness;
		_allowAdvance = false;
		fadeInDeathNumbers = true;

		deathTextDisplay.enabled = true;
		deathRedTextDisplay.enabled = true;
		deathSequence = true;
	}

	public void EndDeathCountup(){

		fadeInDeathNumbers = false;
		fadeOutDeathNumbers = true;
	}
}
