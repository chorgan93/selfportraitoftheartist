using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FakeEnemyBuddyS : MonoBehaviour {

	public Transform targetRef;

	[Header("Movement Properties")]
	public Transform _buddyPos;
	public GameObject buddySound;

	public float followSpeed;
	public float nearPlayerMult = 0.5f;
	private Rigidbody _myRigid;
	public Rigidbody myRigid { get { return _myRigid; } }

	private bool targetInRange = false;

	[Header("Visual Properties")]
	public SpriteRenderer shadowRenderer;
	public Color shadowColor;

	private float startScale;
	public float sScale { get { return startScale; } }

	private Animator _myAnimator;
	public Animator myAnimator { get { return _myAnimator; } }


	public virtual void Initialize(){

		_myRigid = GetComponent<Rigidbody>();

		_myAnimator = GetComponent<Animator>();

		//transform.parent = null;

		startScale = transform.localScale.x;

		Color shadowCol = shadowColor;
		shadowCol.a = shadowRenderer.color.a;
		shadowRenderer.color = shadowCol;
		shadowRenderer.material.SetColor("_FlashColor", shadowColor);

		_myAnimator.SetFloat("DifficultySpeed", 1f);


	}

	void Start(){
		Initialize();
	}

	void FixedUpdate(){
		FollowEnemy();
	}

	public virtual void FollowEnemy(){

		Vector3 moveForce = Vector3.zero;
		moveForce = (_buddyPos.position-transform.position).normalized*followSpeed*Time.deltaTime;


		if (targetInRange){
			moveForce*=nearPlayerMult;
		}

		_myRigid.AddForce(moveForce, ForceMode.Acceleration);

		Vector3 fixPos = transform.position;
		if (transform.position.y < targetRef.position.y){
			fixPos.z = targetRef.position.z-1f;
		}else{
			fixPos.z = targetRef.position.z+1f;
		}
		transform.position = fixPos;


	}

	public virtual void BuddyUpdate(){
		if (targetRef){
			if (!targetRef.gameObject.activeSelf){
				gameObject.SetActive(false);
			}
		}
	}

	public virtual void FaceDirection(){

		Vector3 faceScale = transform.localScale;
		if (_myRigid.velocity.x > 0){
			faceScale.x = startScale;
		}

		if (_myRigid.velocity.x < 0){
			faceScale.x = -startScale;
		}
		transform.localScale = faceScale;
	}

	public void SetPositions(Transform upperPos, Transform lowerPos){
		_buddyPos = upperPos;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.transform == targetRef){
			targetInRange = true;
		}
	}


	void OnTriggerExit(Collider other){
		if (other.gameObject.transform == targetRef){
			targetInRange = false;
		}
	}


}
