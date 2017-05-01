using UnityEngine;
using System.Collections;

public class ParryEffectS : MonoBehaviour {

	public SpriteRenderer topSprite;
	public SpriteRenderer altSprite;
	private Vector3 topSpriteSize;
	public float randomZMult = 20f;

	public float sizeDistortionXMax = 0.75f;
	public float sizeDistortionYMax = 0.3f;
	private Vector3 currentSpriteSize;
	public float distortRate = 0.08f;
	private float distortCountdown;

	void Start(){
		topSpriteSize = topSprite.transform.localScale;
	}

	void Update(){
		distortCountdown -= Time.deltaTime;
		if (distortCountdown <= 0){
			distortCountdown = distortRate;
			currentSpriteSize = topSpriteSize;
			currentSpriteSize.x += sizeDistortionXMax*Random.insideUnitCircle.x;
			currentSpriteSize.y += sizeDistortionYMax*Random.insideUnitCircle.y;
			topSprite.transform.localScale = currentSpriteSize;
		}
	}

	public void FireParry(Vector3 playerPos, Vector3 enemyPos, Color colorOne, Color colorTwo){
		topSprite.color = colorOne;
		altSprite.color = colorTwo;

		transform.Rotate(FaceDirection((enemyPos-playerPos).normalized));
	}

	private Vector3 FaceDirection(Vector3 direction){

		float rotateZ = 0;

		Vector3 targetDir = direction.normalized;

		if(targetDir.x == 0){
			if (targetDir.y > 0){
				rotateZ = 90;
			}
			else{
				rotateZ = -90;
			}
		}
		else{
			rotateZ = Mathf.Rad2Deg*Mathf.Atan((targetDir.y/targetDir.x));
		}	


		if (targetDir.x < 0){
			rotateZ += 180;
		}

		rotateZ += randomZMult*Random.insideUnitCircle.x;


		return new Vector3(0,0,rotateZ);


	}
}
