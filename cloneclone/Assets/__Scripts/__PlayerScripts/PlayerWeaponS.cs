using UnityEngine;
using System.Collections;

public class PlayerWeaponS : MonoBehaviour {

	public string weaponName = "L U N A";
	public int weaponNum = 0;
	public int displayNum = 0;
	public float speedMult = 1f;
	public GameObject[] attackChain;
	public GameObject[] heavyChain;
	public GameObject[] chargeChain;
	public GameObject dashAttack;
	public GameObject counterAttack;
	public GameObject counterAttackHeavy;
	public GameObject switchSoundObj;

	public Sprite swapSprite;
	public Color swapColor;
	public Color flashSubColor;
	public Color invertSwapColor;
	public Color invertSubColor;

	public GameObject attackFlashMain;
	public GameObject attackFlashSub;

	private float zRotateOffset = 20f;

	private const float _spawnRange = 1.3f;
	private float doFlip = 1f;

	public void AttackFlash(Vector3 startPos, Vector3 dir, Transform newParent, float delay, 
		Color overrideColor){

		Vector3 spawnPos = startPos;
		spawnPos -= dir.normalized*_spawnRange;

		float newAnimRate = delay;

		GameObject attackFlash1 = Instantiate(attackFlashMain, spawnPos, Quaternion.Euler(EffectDirection(dir)))
			as GameObject;
		SpriteRenderer flashRender = attackFlash1.GetComponent<SpriteRenderer>();
		Color fixCol = flashRender.color;
		if (overrideColor != null){
			fixCol = overrideColor;
		}else{
		fixCol = swapColor;
		}
		fixCol.a = flashRender.color.a;
		flashRender.color = fixCol;
		attackFlash1.transform.parent = newParent;

		Vector3 flipSize = attackFlash1.transform.localScale;
		flipSize.y *= doFlip;
		attackFlash1.transform.localScale = flipSize;

		// TODO holy shit fix this for charge attacks
		/*if (numFlashes > 0){
			if (doFlip > 0){
				attackFlash1.transform.position += attackFlash1.transform.up*1.25f;
			}else{
				
				attackFlash1.transform.position -= attackFlash1.transform.up*1.25f;
			}
		}**/

		AnimObjS animRef = attackFlash1.GetComponent<AnimObjS>();
		animRef.animRate = delay/(animRef.animFrames.Length*1f+2f);

		GameObject attackFlash2 = Instantiate(attackFlashSub, spawnPos, Quaternion.Euler(EffectDirection(dir)))
			as GameObject;
		flashRender = attackFlash2.GetComponent<SpriteRenderer>();
		fixCol = flashRender.color;
		fixCol = flashSubColor;
		fixCol.a = flashRender.color.a;
		flashRender.color = fixCol;
		attackFlash2.transform.parent = newParent;

		flipSize = attackFlash2.transform.localScale;
		flipSize.y *= doFlip;
		attackFlash2.transform.localScale = flipSize;

		// TODO holy shit fix this for charge attacks
		/*if (numFlashes > 0){
			if (doFlip > 0){
				attackFlash2.transform.position += attackFlash2.transform.up*1.25f;
			}else{
				
				attackFlash2.transform.position -= attackFlash2.transform.up*1.25f;
			}
		}**/

		animRef = attackFlash2.GetComponent<AnimObjS>();
		animRef.animRate = animRef.firstFrameDelay = delay/(animRef.animFrames.Length*1f+2f);

		doFlip *= -1f;


	}

	private Vector3 EffectDirection(Vector3 direction){
		
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

		rotateZ += zRotateOffset;
		
		return new Vector3(0,0,rotateZ);

	}
	
}
