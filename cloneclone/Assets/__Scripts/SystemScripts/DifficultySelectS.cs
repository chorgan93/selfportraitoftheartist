using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DifficultySelectS : MonoBehaviour {

	private enum SelectState { Intro, FadeIn, selectDifficulties, End };
	private SelectState currentState = SelectState.Intro;

	[Header("Intro Animation:")]
	public SpriteRenderer fallingSprite;
	public float fallTime;
	private float fallTimeCount;
	private float fallT;
	public float fallY;
	private Vector3 startPos;
	private Vector3 fallingPos;

	[Header("Sin Assignments:")]
	public Text sinTitleText;
	public Text sinChosenText;
	public Text sinDescriptionText;
	public string[] sinTitles;
	public string[] sinDescriptions;
	public Text sinSelectorLeft;
	public Text sinSelectorRight;
	private int sinSelect = 1;

	[Header("Punishment Assignments:")]
	public Text punishmentTitleText;
	public Text punishmentChosenText;
	public Text punishmentDescriptionText;
	public string[] punishmentTitles;
	public string[] punishmentDescriptions;
	public Text punishSelectorLeft;
	public Text punishSelectorRight;
	private int punishSelect = 1;

	private Color textCol;

	[Header("Effect References")]
	public Image cutToBlack;
	public FadeSpriteObjectS[] airLines;
	public float lineActivateTime = 0.1f;
	private float lineActivateCountdown;

	// Use this for initialization
	void Start () {

		currentState = SelectState.Intro;
		StartCoroutine(difficultySelectSequence());

		startPos = fallingSprite.transform.position;
		fallingSprite.transform.position = fallingPos = new Vector3(fallingSprite.transform.position.x,
		                                               fallingSprite.transform.position.y+fallY,
		                                               fallingSprite.transform.position.z);

	//	turnOffAllText();
	//	setText();

		punishSelect = sinSelect = 1;
		fallTimeCount = 0;
	
	}

	void Update(){
		if (currentState != SelectState.End){
			lineActivateCountdown -= Time.deltaTime;
			if (lineActivateCountdown <= 0){
				lineActivateCountdown = lineActivateTime;
				int lineToActivate = Mathf.FloorToInt(Random.Range(0, airLines.Length));
				airLines[lineToActivate].gameObject.SetActive(true);
				airLines[lineToActivate].Reinitialize();
			}
		}
	}

	void HandleDifficultySelect(){

		if (currentState == SelectState.Intro){
			while (fallTimeCount < fallTime){
				fallTimeCount += Time.deltaTime;
				fallT = fallTimeCount/fallTime;
			}
		}

	}

	void turnOffAllText(){
		sinTitleText.enabled = false;
		sinDescriptionText.enabled = false;
		sinChosenText.enabled = false;

		sinSelectorLeft.enabled = sinSelectorRight.enabled = false;
		
		punishmentTitleText.enabled = false;
		punishmentDescriptionText.enabled = false;
		punishmentChosenText.enabled = false;

		punishSelectorLeft.enabled = punishSelectorRight.enabled = false;

		Color textCol = sinTitleText.color;
		textCol.a = 0;
		sinTitleText.color = sinDescriptionText.color = sinChosenText.color = textCol;
		
		textCol = punishmentTitleText.color;
		textCol.a = 0;
		punishmentTitleText.color = punishmentDescriptionText.color = punishmentChosenText.color = textCol;

		textCol = punishSelectorLeft.color;
		textCol.a = 0;
		punishSelectorLeft.color = punishSelectorRight.color = sinSelectorLeft.color = sinSelectorRight.color
			= textCol;
	}

	void turnOnAllText(){
		sinTitleText.enabled = true;
		sinDescriptionText.enabled = true;
		sinChosenText.enabled = true;
		
		sinSelectorLeft.enabled = sinSelectorRight.enabled = true;
		
		punishmentTitleText.enabled = true;
		punishmentDescriptionText.enabled = true;
		punishmentChosenText.enabled = true;
		
		punishSelectorLeft.enabled = punishSelectorRight.enabled = true;
	}

	void updateSelectors(){
		
		sinSelectorLeft.enabled = sinSelectorRight.enabled = true;

		if (sinSelect <= 0){
			sinSelectorLeft.enabled = false;
		}
		if (sinSelect >= sinTitles.Length){
			sinSelectorRight.enabled = false;
		}
		
		punishSelectorLeft.enabled = punishSelectorRight.enabled = true;

		if (punishSelect <= 0){
			punishSelectorLeft.enabled = false;
		}
		if (punishSelect >= punishmentTitles.Length){
			punishSelectorRight.enabled = false;
		}
	}

	void setText(){
		
		sinTitleText.text = sinTitles[sinSelect];
		punishmentTitleText.text = punishmentTitles[punishSelect];
		
		sinDescriptionText.text = sinDescriptions[sinSelect];
		punishmentDescriptionText.text = punishmentDescriptions[punishSelect];

	}

	IEnumerator difficultySelectSequence(){

		if (currentState == SelectState.Intro){
			yield return new WaitForSeconds(2f);

			while (fallTimeCount <= fallTime){
				fallTimeCount+=Time.deltaTime;
				fallT = fallTimeCount/fallTime;
				fallT = fallT*fallT * (3f - 2f*fallT);

				fallingSprite.transform.position = Vector3.Lerp(fallingPos, startPos, fallT);
				yield return null;
			}

			currentState = SelectState.FadeIn;
		}

		if (currentState == SelectState.FadeIn){
			while (textCol.a < 1f){
				yield return null;
			}
			currentState = SelectState.selectDifficulties;
		}
		yield return null;
	}
}
