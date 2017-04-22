using UnityEngine;
using System.Collections;

public class InstructionFloatS : MonoBehaviour {

	public Vector3 InstructionOffset = Vector3.zero;
	Transform followTransform;
	Vector3 currentPos;
	bool isShowing = false;

	public SpriteRenderer buttonSprite;
	private Color currentSpriteCol;
	public TextMesh examineString;
	public TextMesh buttonString;
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

	// Use this for initialization
	void Start () {

		currentSpriteCol = buttonSprite.color;
		currentSpriteCol.a = 0f;
		buttonSprite.color = currentSpriteCol;

		currentTextCol = examineString.color;
		currentTextCol.a = 0f;
		examineString.color = buttonString.color = currentTextCol;
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
				currentTextCol = examineString.color;
				if (fadingIn){

					currentSpriteCol.a += Time.deltaTime*fadeInRate;
					currentTextCol.a += Time.deltaTime*fadeInRate;
					if (currentSpriteCol.a >= 1){
						currentSpriteCol.a = currentTextCol.a = 1f;
						fadingIn = false;
					}
					
				}
				if (fadingOut){
					currentSpriteCol.a -= Time.deltaTime*fadeOutRate;
					currentTextCol.a -= Time.deltaTime*fadeOutRate;
					if (currentSpriteCol.a <= 0){
						currentSpriteCol.a = currentTextCol.a = 0f;
						fadingOut = false;
						isShowing = false;
					}
				}

				buttonSprite.color = currentSpriteCol;
				examineString.color = buttonString.color = currentTextCol;
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
		gameObject.SetActive(true);

	}
	public void HideInstruction(){
		fadingIn = false;
		fadingOut = true;
	
	}
}
