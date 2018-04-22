using UnityEngine;
using System.Collections;

public class TauntDialogueS : MonoBehaviour {

	public TextMesh[] texts;
	private Color[] textCols;
	private Vector3[] textPos;

	private bool colsSet = false;

	private string tauntString = "COME ON !!";
	public float flashOnTime = 0.2f;
	private bool flashingOn = false, flashingOff = false;
	private float flashTime = 0f;
	public float flashOffTime = 0.1f;

	public float shakeTime= 0.5f;
	private float shakeCount = 0f;
	public float shakeOffsetX = 2f;
	public float shakeOffsetY = 0.4f;

	public float showTime= 0.4f;
	private float showCount = 0f;

	private Vector3 currentOffset = Vector3.zero;

	private bool _doingEffect = false;

	private Transform startParent;
	private Vector3 posOffset;
	private Vector3 posOffsetReverse;
	private Vector3 posRandomize = Vector3.zero;
	public float posRandomizeMult = 0.5f;

	// Use this for initialization
	void Start () {
	
		textPos = new Vector3[texts.Length];
		for (int i = 0; i < texts.Length; i++){
			texts[i].text = "";
			textPos[i] = texts[i].transform.localPosition;
		}

		SetEffect(Color.cyan, 1f, false);

		posOffset = transform.localPosition;
		posOffsetReverse = posOffset;
		posOffsetReverse.x *= -2f;
		startParent = transform.parent;
		transform.parent = null;


	}
	
	// Update is called once per frame
	void Update () {

		if (_doingEffect){
			if (flashingOn || flashingOff){
				flashTime -= Time.deltaTime;
				if (flashTime <= 0){
					if (flashingOff){
						SetEffect(Color.cyan, 1f, false);
					}
					if (flashingOn){
						for (int i  = 0; i < texts.Length; i++){
							texts[i].color = textCols[i];
						}
					}
					flashingOn = flashingOff = false;
				}
			}else if (shakeCount > 0){
				shakeCount -= Time.deltaTime;
				if (shakeCount <= 0){
					shakeCount = 0;
				}
				for (int i = 0; i < texts.Length; i++){
					currentOffset = Random.insideUnitSphere*shakeCount/shakeTime;
					currentOffset.z = 0f;
					currentOffset.x *= shakeOffsetX;
					currentOffset.y *= shakeOffsetY;
					texts[i].transform.localPosition = textPos[i]+currentOffset;
				}
			}else{
				showCount -= Time.deltaTime;
				if (showCount <= 0){
					flashingOff = true;
					flashTime = flashOffTime;
					for (int i  = 0; i < texts.Length; i++){
						texts[i].color = Color.white;
					}
				}
			}
		}
	
	}

	public void SetEffect(Color playerCol, float dir = 1f, bool doEffect = true){
		
		_doingEffect = doEffect;

		if (!_doingEffect){
			gameObject.SetActive(false);
		}
		flashingOn = true;
		flashingOff = false;
		flashTime = flashOnTime;
		shakeCount = shakeTime;
		showCount = showTime;
		if (!colsSet){
			textCols = new Color[texts.Length];
		}
		for (int i  = 0; i < texts.Length; i++){
			if (!colsSet){
				textCols[i] = texts[i].color;
			}
			if (i == 0){
				textCols[i] = playerCol;
			}
			texts[i].color = Color.white;
			texts[i].text = tauntString;
			texts[i].transform.localPosition = textPos[i];
		}
		colsSet = true;
		if (_doingEffect){
			posRandomize = Random.insideUnitSphere*posRandomizeMult;
			posRandomize.z = 0f;
			if (dir > 0){
				transform.position = startParent.position+(posOffset+posRandomize);
			}else{
				transform.position = startParent.position+(posOffsetReverse-posRandomize);
			}
			gameObject.SetActive(true);
		}

	}
}
