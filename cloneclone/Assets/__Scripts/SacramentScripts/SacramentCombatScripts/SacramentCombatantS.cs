using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SacramentCombatantS : MonoBehaviour {

	public enum SacramentIVID {C, K, AA, Nightmare};
	public SacramentIVID combatID = SacramentIVID.Nightmare;

	private static float allyHealth_C = 100f;
	private static float allyHealth_K = 100f;
	private static float allyHealth_AA = 100f;

	private float maxHealth = 100f;

	public string combatantName;

	public bool isEnemy = false;
	public int startPriority = -1;
	private int _currentPriority;
	public int currentPriority { get { return _currentPriority; } }

	[Header("Display Properties")]
	public Image combatantImage;
	private Color startColor;
	public Color koColor = Color.grey;
	public string[] startTurnString;
	public Image healthBar;
	public Text healthPercent;
	private Vector2 startHealthSize;

	[Header("Status Properties")]
	public float startHealth;
	private float currentHealth;
	public float returnHealth { get { return currentHealth; } }
	public bool startHiddenState = false;
	private bool _isHiding = false;
	public bool isHiding { get {return _isHiding; } }

	[Header("Combat Stats")]
	public float baseStrength = 1f;
	private float workingStrength = 1f;
	public float currentStrength {get {return workingStrength; } }
	public float baseDefense = 1f;
	private float workingDefense = 1f;
	public float currentDefense {get {return workingStrength; } }
	public float baseAccuracy = 0.8f;
	private float workingAccuracy = 0.8f;
	public float currentAccuracy { get { return workingAccuracy; } }
	public float baseEvasion = 1f;
	private float workingEvasion = 1f;
	public float currentEvasion { get { return workingEvasion; } }
	public float hiddenEvadeMult = 0.2f;

	private SacramentCombatS _myManager;
	public SacramentCombatS myManager { get { return _myManager; } }

	private SacramentCombatActionS _currentAction;
	public SacramentCombatActionS currentAction { get { return _currentAction; } }

	private SacramentCombatActionS _queuedAction;
	public SacramentCombatActionS[] possibleActions;
	public SacramentCombatActionOptionS[] possibleActionSelectors;

	private List<int> buffNums = new List<int>();
	private List<float> buffMults = new List<float>();
	private List<int> buffDecays = new List<int>();

	public List<SacramentCombatActionS> possAIActions;

	private float hideRate = 1f;
	private float revealRate = 3f;
	private Color fadeCol;

	private bool _optionsShowing = false;

	// Use this for initialization
	void Start () {

		_isHiding = startHiddenState;

		if (healthBar){
		startHealthSize = healthBar.rectTransform.sizeDelta;
		}
		if (combatID == SacramentIVID.Nightmare){ 
			currentHealth = maxHealth = startHealth;
		}else if (combatID == SacramentIVID.C){
			currentHealth = allyHealth_C;
		}else if (combatID == SacramentIVID.K){
			currentHealth = allyHealth_K;
		}else if (combatID == SacramentIVID.AA){
			currentHealth = allyHealth_AA;
		}
		_currentPriority = startPriority;

		SetHealthBar();

		startColor = combatantImage.color;
		workingEvasion = baseEvasion;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartActing(SacramentCombatS myC){
		if (!_myManager){
			_myManager = myC;
			workingAccuracy = baseAccuracy;
		}
		if (_queuedAction != null){
			_queuedAction.StartAction(this);
		}
		else if (isEnemy){
			SetAIActions();
			int actionToDo = Mathf.FloorToInt(Random.Range(0, possAIActions.Count));
			possAIActions[actionToDo].StartAction(this);
			_currentAction = possibleActions[actionToDo];
		}else{
			_myManager.combatText.AddToString(startTurnString[Mathf.FloorToInt(Random.Range(0, startTurnString.Length))],
				null, true);
		}
	}

	void SetAIActions(){
		possAIActions = new List<SacramentCombatActionS>();
		for (int i = 0; i < possibleActions.Length; i++){
			if (possibleActions[i].ValidAction()){
				possAIActions.Add(possibleActions[i]);
			}
		}
	}

	public void SetPriority (int newP){
		_currentPriority = newP;
	}
	public void SetActionQueue (SacramentCombatActionS newA){
		_queuedAction = newA;
	}

	public void SetHiding(bool newH){
		_isHiding = newH;
		if (newH){
			StartCoroutine(HideRoutine());
		}else{
			StartCoroutine(RevealRoutine());
		}
	}

	IEnumerator HideRoutine(){
		fadeCol = startColor;
		while (fadeCol.a > 0f){
			fadeCol.a -= Time.deltaTime*hideRate;
			combatantImage.color = fadeCol;
			yield return null;
		}
		fadeCol.a = 0f;
		combatantImage.color = fadeCol;
	}

	IEnumerator RevealRoutine(){
		fadeCol = startColor;
		fadeCol.a = 0f;
		while (fadeCol.a < 1f){
			fadeCol.a += Time.deltaTime*revealRate;
			combatantImage.color = fadeCol;
			yield return null;
		}
		fadeCol.a = 1f;
		combatantImage.color = fadeCol;
	}

	public void ShowOptions(bool newShow = true){
		for (int i = 0; i < possibleActionSelectors.Length; i++){
			if (newShow){ 
				possibleActionSelectors[i].Initialize(this);
			}else if (_optionsShowing){
				possibleActionSelectors[i].DeactivateOption();

			}
		}
		_optionsShowing = newShow;
	}
	public void SelectAction(int actionToChoose){
		possibleActions[actionToChoose].StartAction(this, true);
		ShowOptions(false);
	}

	public void TakeDamage(float dmgAmount){
		currentHealth -= dmgAmount;
		if (currentHealth <= 0f){
			currentHealth = 0f;

			if (healthPercent){
				healthPercent.text = "CRITICAL";
			}
		}else if (healthPercent){
			healthPercent.text = Mathf.RoundToInt(currentHealth/maxHealth*100f) + " %";
		}
		SetHealthBar();
	}

	void SetHealthBar(){
		if (healthBar){
		Vector2 newSize = startHealthSize;
		newSize.x *= currentHealth/maxHealth;
		healthBar.rectTransform.sizeDelta = newSize;
		}
	}

	public void ApplyBuffs(){
		workingStrength = baseStrength;
		workingDefense = baseDefense;
		workingAccuracy = baseAccuracy;
		workingEvasion = baseEvasion;
		for (int i = 0; i < buffNums.Count; i++){
			switch (buffNums[i]){
			case (0):
				workingStrength*=buffMults[i];
				break;
			case (1):
				workingDefense*=buffMults[i];
				break;
			case (2):
				workingAccuracy*=buffMults[i];
				break;
			case (3):
				workingEvasion*=buffMults[i];
				break;
				default:
				break;
			}
		}
	}

	public void DecayBuffs(){
		for (int i = 0; i < buffDecays.Count; i++){
			buffDecays[i]--;
		}
		RemoveBuffs();
	}

	public void RemoveBuffs(){
		for (int i = buffNums.Count; i >= 0; i--){
			if (buffDecays[i]<=0){
					switch (buffNums[i]){
					case (0):
						workingStrength/=buffMults[i];
						break;
					case (1):
						workingDefense/=buffMults[i];
						break;
					case (2):
						workingAccuracy/=buffMults[i];
						break;
					case (3):
						workingEvasion/=buffMults[i];
						break;
					default:
						break;
					}

				buffNums.RemoveAt(i);
				buffDecays.RemoveAt(i);
				buffMults.RemoveAt(i);
			}
		}
	}

}
