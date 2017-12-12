using UnityEngine;
using System.Collections;

public class SpriteDistortionPlayerS : MonoBehaviour {

	private SpriteRenderer mySprite;
	private SpriteRenderer parentSprite;

	public float changeRate = 0.08f;
	private float changeCountdown = 0f;
	public float changeSizeAmt = 0.2f;
	public float changePosAmtX = 0.05f;
	public float changePosAmtY = 0.03f;
	private Vector3 currentPos;

	[Header("Bios Properties")]
	public bool biosDistortion = false;
	public Color overrideColor = Color.white;
	public Vector3 distortionSizeAdd = Vector3.zero;


	private PlayerStatsS playerReference;

	// Use this for initialization
	void Start () {

		playerReference = transform.GetComponentInParent<PlayerStatsS>();

		mySprite = GetComponent<SpriteRenderer>();
		parentSprite = transform.parent.GetComponent<SpriteRenderer>();
		if (biosDistortion){
			mySprite.material.SetColor("_FlashColor", overrideColor);
		}else{
			mySprite.material.SetColor("_FlashColor", parentSprite.color);
		}
		mySprite.sprite = parentSprite.sprite;
		ChangeSize();
	}
	
	// Update is called once per frame
	void Update () {

		if (playerReference.PlayerIsDead()){
			mySprite.enabled = false;
		}

		if (mySprite.enabled){
			if (!biosDistortion){
			if (parentSprite.color != mySprite.material.GetColor("_FlashColor")){
				mySprite.material.SetColor("_FlashColor", parentSprite.color);
			}
			}
			mySprite.sprite = parentSprite.sprite;
	
			changeCountdown -= Time.deltaTime;
			if (changeCountdown <= 0){
				ChangeSize();
			}
		}
	
	}

	private void ChangeSize(){
		transform.localScale = (Vector3.one + distortionSizeAdd) +Random.insideUnitSphere*changeSizeAmt;
		changeCountdown = changeRate;
		currentPos = Vector3.zero;
		currentPos.x += changePosAmtX*Random.insideUnitCircle.x;
		currentPos.y += changePosAmtY*Random.insideUnitCircle.y;
		currentPos.z = transform.localPosition.z;
		transform.localPosition = currentPos;
	}
}
