using UnityEngine;
using System.Collections;

public class InstructionFloatS : MonoBehaviour {

	public Vector3 InstructionOffset = Vector3.zero;
	Transform followTransform;
	Vector3 currentPos;
	bool isShowing = false;

	public SpriteRenderer buttonSprite;
	public SpriteRenderer buttonSpritePS4;
	public SpriteRenderer keySprite;
	public SpriteRenderer mouseSprite;
	private Color currentSpriteCol;
	private Color currentSpritePS4Col;
	private Color currentKeyCol;
	public TextMesh examineString;
	public TextMesh buttonString;
	public TextMesh buttonStringPS4;
	private bool useButtonStringPS4;
	private Color currentTextCol;

	private bool fadingIn = false;
	private bool fadingOut = false;
	private float fadeInRate = 1f;
	private float fadeOutRate = 1f;

	Vector3 wanderPos = Vector3.zero;
	Vector3 currentOffset = Vector3.zero;
	private float wanderMultX = 0.2f;
	private float wanderMultY = 0.4f;
	private float wanderSpeed = 0.25f;
	private float wanderCount;
	private float wanderChangeMin = 0.5f;
	private float wanderChangeMax = 1f;

	[Header("Really niche fix for start/select buttons")]
	public TextMesh subString;
	public string changeSubKeyString = "";
	public string changeSubMouseString = "";

	// Use this for initialization
	void Start () {

		if (ControlManagerS.controlProfile == 3 && buttonStringPS4 != null){
			useButtonStringPS4 = true;
			buttonString.gameObject.SetActive(false);
		}else{
			if (buttonStringPS4){
				buttonStringPS4.gameObject.SetActive(false);
			}
		}

		currentSpriteCol = buttonSprite.color;
		currentSpriteCol.a = 0f;
		buttonSprite.color = currentSpriteCol;

		currentSpritePS4Col = buttonSpritePS4.color;
		currentSpritePS4Col.a = 0f;
		buttonSpritePS4.color = currentSpritePS4Col;

		currentKeyCol = keySprite.color;
		currentKeyCol.a = 0f;
		mouseSprite.color = keySprite.color = currentKeyCol;

		currentTextCol = examineString.color;
		currentTextCol.a = 0f;
		if (useButtonStringPS4){
			examineString.color = buttonStringPS4.color = currentTextCol;
		}else{
		examineString.color = buttonString.color = currentTextCol;
		}

		changeSubKeyString = changeSubKeyString.Replace("NEWLINE", "\n");
		changeSubMouseString = changeSubMouseString.Replace("NEWLINE", "\n");



	}
	
	// Update is called once per frame
	void Update () {

		if (isShowing){

			wanderCount -= Time.deltaTime;
			if (wanderCount <= 0){
				wanderPos = Random.insideUnitSphere;
				wanderPos.z = 0f;
				wanderPos.x *= wanderMultX;
				wanderPos.y *= wanderMultY;
				wanderCount = Random.Range(wanderChangeMin, wanderChangeMax);
			}

			currentOffset += (wanderPos-currentOffset).normalized*wanderSpeed*Time.deltaTime;

			currentPos = followTransform.position+InstructionOffset+currentOffset;
			transform.position = currentPos;

			if (fadingIn || fadingOut){
				currentSpriteCol = buttonSprite.color;
				currentSpritePS4Col = buttonSpritePS4.color;
				currentKeyCol = keySprite.color;
				currentTextCol = examineString.color;
				if (fadingIn){

					currentSpriteCol.a += Time.deltaTime*fadeInRate;
					currentSpritePS4Col.a += Time.deltaTime*fadeInRate;
					currentKeyCol.a += Time.deltaTime*fadeInRate;
					currentTextCol.a += Time.deltaTime*fadeInRate;
					if (currentSpriteCol.a >= 1){
						currentSpriteCol.a = currentSpritePS4Col.a = currentKeyCol.a = currentTextCol.a = 1f;
						fadingIn = false;
					}
					
				}
				if (fadingOut){
					currentSpriteCol.a -= Time.deltaTime*fadeOutRate;
					currentSpritePS4Col.a -= Time.deltaTime*fadeOutRate;
					currentTextCol.a -= Time.deltaTime*fadeOutRate;
					currentKeyCol.a -= Time.deltaTime*fadeOutRate;
					if (currentSpriteCol.a <= 0){
						currentSpriteCol.a = currentSpritePS4Col.a = currentTextCol.a = currentKeyCol.a = 0f;
						fadingOut = false;
						isShowing = false;
					}
				}

				buttonSprite.color = currentSpriteCol;
				buttonSpritePS4.color = currentSpritePS4Col;
				mouseSprite.color = keySprite.color = currentKeyCol;
				if (useButtonStringPS4){
					examineString.color = buttonStringPS4.color = currentTextCol;
				}else{
				examineString.color = buttonString.color = currentTextCol;
				}
			}
		}
	
	}

	public void ShowInstruction(Transform newFollow, bool useController = true){
		
		followTransform = newFollow;
		isShowing = true;
		fadingOut = false;

		if (currentTextCol.a < 1f){
			fadingIn = true;
			transform.position = currentPos = followTransform.position + InstructionOffset;
		}

		wanderPos = Random.insideUnitSphere;
		wanderPos.z = 0f;
		wanderPos.x *= wanderMultX;
		wanderPos.y *= wanderMultY;
		wanderCount = Random.Range(wanderChangeMin, wanderChangeMax);

		currentPos = followTransform.position+InstructionOffset;
		if (ControlManagerS.controlProfile == 3){

			buttonSpritePS4.gameObject.SetActive(true);

			buttonSprite.gameObject.SetActive(false);
			mouseSprite.gameObject.SetActive(false);
			keySprite.gameObject.SetActive(false);
		}
		else if (useController && ControlManagerS.controlProfile == 0){
			buttonSprite.gameObject.SetActive(true);
			keySprite.gameObject.SetActive(false);
			mouseSprite.gameObject.SetActive(false);
			buttonSpritePS4.gameObject.SetActive(false);
		}else{
			buttonSprite.gameObject.SetActive(false);
			mouseSprite.gameObject.SetActive(false);
			keySprite.gameObject.SetActive(false);
			buttonSpritePS4.gameObject.SetActive(false);
			if (ControlManagerS.controlProfile == 1){
				mouseSprite.gameObject.SetActive(true);
				if (changeSubMouseString != ""){
					subString.text = changeSubMouseString;
				}
			}else{
				keySprite.gameObject.SetActive(true);
				if (changeSubKeyString != ""){
					subString.text = changeSubKeyString;
				}
			}
		}
		gameObject.SetActive(true);

	}
	public void HideInstruction(){
		fadingIn = false;
		fadingOut = true;
	
	}
}
