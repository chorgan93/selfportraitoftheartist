using UnityEngine;
using System.Collections;

public class EnemyChaseAttackS : EnemyBehaviorS {

	public GameObject attackPrefab;
	private GameObject currentAttackPrefab;
	private EnemyProjectileS currentAttackS;
	
	[Header("Behavior Duration")]
	public float chaseTimeFixed = -1f;
	public float chaseTimeMin;
	public float chaseTimeMax;
	public PlayerDetectS rangeCheck;
	
	[Header("Movement Variables")]
	public float chaseDragAmt = -1f;
	public float chaseSpeedFixed = -1f;
	public float chaseSpeedMin;
	public float chaseSpeedMax;
	public float recenterMin;
	public float recenterMax;
	private Vector3 recenterTarget;
	private float recenterCountdown;
	
	private float currentchaseSpeed;
	
	private float chaseTimeCountdown;
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (BehaviorActing()){
			
			
			DoMovement();
			
			chaseTimeCountdown -= Time.deltaTime;
			
		
			
			if (chaseTimeCountdown <= 0){
				EndAction();
			}
		}
		
	}
	
	private void InitializeAction(){

		if (AttackInRange()){
			
			myEnemyReference.myAnimator.SetTrigger(animationKey);
			if (signalObj != null){
				Vector3 signalPos =  transform.position;
				signalPos.z = transform.position.z+1f;
				GameObject signal = Instantiate(signalObj, signalPos, Quaternion.identity)
					as GameObject;
				signal.transform.parent = myEnemyReference.transform;
			}
		
		if (chaseTimeFixed > 0){
			chaseTimeCountdown = chaseTimeFixed;
		}
		else{
			chaseTimeCountdown = Random.Range(chaseTimeMin, chaseTimeMax);
		}
		
		if (chaseSpeedFixed > 0){
			currentchaseSpeed = chaseSpeedFixed;
		}
		else{
			currentchaseSpeed = Random.Range(chaseSpeedMin, chaseSpeedMax);
		}

		if (recenterMin > 0){
			recenterTarget = myEnemyReference.GetTargetReference().transform.position;
			recenterCountdown = Random.Range(recenterMin,recenterMax);
		}

		currentAttackPrefab = Instantiate(attackPrefab, transform.position, Quaternion.identity)
			as GameObject;
		currentAttackS = currentAttackPrefab.GetComponent<EnemyProjectileS>();
		currentAttackS.AllowMultiHit();
		currentAttackS.Fire((myEnemyReference.GetTargetReference().transform.position
		                     -transform.position).normalized, myEnemyReference);
		currentAttackPrefab.transform.localPosition = Vector3.zero;
		
		if (chaseDragAmt > 0){
			myEnemyReference.myRigidbody.drag = chaseDragAmt;
		}
		}
		else{
			myEnemyReference.myAnimator.SetTrigger("Idle");
			EndAction();
		}

		
	}

	private bool AttackInRange(){
		
		bool canContinue = true;
		
		if (rangeCheck != null){
			if (!rangeCheck.PlayerInRange()){
				canContinue = false;
			}
		}
		
		return canContinue;
		
	}
	
	private void DoMovement(){
		
		if (!myEnemyReference.hitStunned){
			if (recenterMin > 0){
				recenterCountdown -= Time.deltaTime;
				if (recenterCountdown <= 0){
					recenterTarget = myEnemyReference.GetTargetReference().transform.position;
					recenterCountdown = Random.Range(recenterMin,recenterMax);
				}
				myEnemyReference.myRigidbody.AddForce((recenterTarget
				                                       -transform.position).normalized*currentchaseSpeed*Time.deltaTime);
			}else{
			myEnemyReference.myRigidbody.AddForce((myEnemyReference.GetTargetReference().transform.position
			                                       -transform.position).normalized*currentchaseSpeed*Time.deltaTime);
			}
		}
		
	}
	
	public override void StartAction (bool setAnimTrigger = true)
	{
		base.StartAction (false);
		
		InitializeAction();
	}
	
	public override void EndAction (bool doNextAction = true)
	{
		if (currentAttackPrefab){
			Destroy(currentAttackPrefab);
		}
		base.EndAction (doNextAction);
	}
}
