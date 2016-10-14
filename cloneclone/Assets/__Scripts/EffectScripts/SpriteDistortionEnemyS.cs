using UnityEngine;
using System.Collections;

public class SpriteDistortionEnemyS : MonoBehaviour {

	private SpriteRenderer mySprite;
	private SpriteRenderer parentSprite;

	public float changeRate = 0.12f;
	private float changeCountdown = 0f;
	public float changeSizeAmt = 0.2f;
	public float changeSizeCritMult = 2f;
	private float changeRateCritMult = 3f;

	private float deadChangeRate;

	private EnemyS enemyReference;

	// Use this for initialization
	void Start () {

		enemyReference = transform.GetComponentInParent<EnemyS>();

		mySprite = GetComponent<SpriteRenderer>();
		parentSprite = transform.parent.GetComponent<SpriteRenderer>();
		mySprite.material.SetColor("_FlashColor", enemyReference.bloodColor);
		mySprite.sprite = parentSprite.sprite;
		ChangeSize();

		deadChangeRate = changeRate*2f;
	}
	
	// Update is called once per frame
	void Update () {

		if (enemyReference.isDead){
			changeRate = deadChangeRate;
		}

		if (mySprite.enabled){
			mySprite.sprite = parentSprite.sprite;
	
			if (enemyReference.isCritical && !enemyReference.isDead){
				changeCountdown -= Time.deltaTime*changeRateCritMult;
			}else{
				changeCountdown -= Time.deltaTime;
			}
			if (changeCountdown <= 0){
				ChangeSize();
			}
		}
	
	}

	private void ChangeSize(){
		if (enemyReference.isCritical && !enemyReference.isDead){
			transform.localScale = Vector3.one+Random.insideUnitSphere*changeSizeCritMult;
		}else{
			transform.localScale = Vector3.one+Random.insideUnitSphere*changeSizeAmt;
		}
		changeCountdown = changeRate;
	}
}
