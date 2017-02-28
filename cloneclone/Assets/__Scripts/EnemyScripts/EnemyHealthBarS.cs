using UnityEngine;
using System.Collections;

public class EnemyHealthBarS : MonoBehaviour {

	private int spawnCode = 2;
	private EffectSpawnManagerS mySpawner;
	private EnemyS currentEnemy;
	private Transform playerTransform;

	private Vector3 offsetPos;

	private Vector3 startParentSize = Vector3.zero;
	private Vector3 currentParentSize;

	private Vector3 currentFullSize;
	private Vector3 currentFillSize;
	private Vector3 currentFillPos;

	public Transform parentObj;
	public SpriteRenderer barBG;
	public SpriteRenderer barFill;

	public SpriteRenderer bottomBit;
	private Vector3 bottomPosStart;
	private Vector3 currentBottomPos;

	private Vector3 startBottomSize;
	private Vector3 currentBottomSize;

	private Vector3 currentPos;

	void Update(){

		if (currentEnemy != null){
			if (!playerTransform && currentEnemy.GetPlayerReference()){
				playerTransform = currentEnemy.GetPlayerReference().transform;
			}
			if (playerTransform){
				offsetPos = currentEnemy.transform.position;
				if (playerTransform.position.x - currentEnemy.transform.position.x > 0){
					offsetPos.x -= currentEnemy.healthBarOffset.x;
					offsetPos.y += currentEnemy.healthBarOffset.y;
					offsetPos.z += currentEnemy.healthBarOffset.z;
					currentBottomSize.x = -startBottomSize.x;
				}else{
					offsetPos += currentEnemy.healthBarOffset;
					currentBottomSize.x = startBottomSize.x;
				}
				bottomBit.transform.localScale = currentBottomSize;
				transform.position = offsetPos;
			}
		}

	}

	public void SetEnemyHealthBar(EffectSpawnManagerS nSpawner, int nSpawnCode, EnemyS newEnemy){

		transform.parent = null;
		if (startParentSize == Vector3.zero){
			startParentSize = transform.localScale;
			bottomPosStart = bottomBit.transform.localPosition;

			startBottomSize = currentBottomSize = bottomBit.transform.localScale;
		}

		mySpawner = nSpawner;
		spawnCode = nSpawnCode;
		currentEnemy = newEnemy;

		offsetPos = currentEnemy.healthBarOffset;

		currentEnemy.SetHealthBar(this);

		currentParentSize = startParentSize;
		currentParentSize.x = currentEnemy.healthBarXSize;
		parentObj.transform.localScale = currentParentSize;

		currentFullSize = barBG.transform.localScale;
		barBG.transform.localScale = currentFullSize;

		currentFillSize = currentFullSize;
		barFill.transform.localScale = currentFillSize;

		currentBottomPos = bottomPosStart;
		currentBottomPos.y = -0.13f*currentEnemy.healthBarXSize;
		bottomBit.transform.localPosition = currentBottomPos;

		ResizeForDamage();
	}

	public void ResizeForDamage(){
		currentFillSize.x = currentFullSize.x * (currentEnemy.currentHealth/currentEnemy.maxHealth);
		if (currentFillSize.x <= 0f){
			currentEnemy.SetHealthBar(null);
			mySpawner.Despawn(gameObject, spawnCode);
		}else{
			barFill.transform.localScale = currentFillSize;
			currentFillPos = barBG.transform.localPosition;
			currentFillPos.x -= 0.16f * (1f-(currentEnemy.currentHealth/currentEnemy.maxHealth));
			currentFillPos.z -= 1f;
			barFill.transform.localPosition = currentFillPos;
		}
	}
}
