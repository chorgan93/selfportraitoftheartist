using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIDistortionS : MonoBehaviour {

	private Image mySprite;
	public Image parentSprite;

	public float changeRate = 0.08f;
	private float changeCountdown = 0f;
	public float changeSizeAmt = 0.2f;

	public float changePosAmtX = 0.015f;
	public float changePosAmtY = 0.0075f;
	private Vector2 currentPos;
	private Vector2 startPos;
	private RectTransform myTransform;

	public bool matchColor = false;

	// Use this for initialization
	void Start () {

		myTransform = GetComponent<RectTransform>();
		startPos = currentPos = myTransform.anchoredPosition;

		mySprite = GetComponent<Image>();
		if(matchColor){
			mySprite.color = parentSprite.color;
		}
	//	mySprite.sprite = parentSprite.sprite;
		ChangeSize();

	}
	
	// Update is called once per frame
	void Update () {


		if (mySprite.enabled){
			//mySprite.sprite = parentSprite.sprite;

			if (matchColor){
				mySprite.color = parentSprite.color;
			}
	

				changeCountdown -= Time.deltaTime;

			if (changeCountdown <= 0){
				ChangeSize();
			}
		}
	
	}

	private void ChangeSize(){
	
		myTransform.localScale = Vector3.one+Random.insideUnitSphere*changeSizeAmt;

		changeCountdown = changeRate;
		currentPos = startPos;
		currentPos.x += changePosAmtX*Random.insideUnitCircle.x;
		currentPos.y += changePosAmtY*Random.insideUnitCircle.y;
		myTransform.anchoredPosition = currentPos;
	}
}
