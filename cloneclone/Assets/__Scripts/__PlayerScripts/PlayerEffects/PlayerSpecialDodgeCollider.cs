using UnityEngine;
using System.Collections;

public class PlayerSpecialDodgeCollider : MonoBehaviour {

	private PlayerController myPlayer;
	private EnemyProjectileS checkProj;
	private EnemyChargeAttackS checkCharge;

	void Start(){
		myPlayer = GetComponentInParent<PlayerController>();
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "EnemyCharge"){

			checkCharge = other.GetComponent<EnemyChargeAttackS>();
			if (checkCharge != null){
				if (!checkCharge.dontDealDamage){
				 myPlayer.CloseCallCheck(checkCharge.enemyReference);
				}
			}

		}
		else if (other.gameObject.tag == "EnemyProjectile" || other.gameObject.tag == "EnemyAttack"){
			checkProj = other.GetComponent<EnemyProjectileS>();
			if (checkProj != null){
				if (!checkProj.dontTriggerWitchTime && !checkProj.dontDealDamage){
					myPlayer.CloseCallCheck(checkProj.myEnemy);
				}
			}
		}
	}
}
