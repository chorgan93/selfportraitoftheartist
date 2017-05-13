using UnityEngine;
using System.Collections;

public class EnemyBuddyS : MonoBehaviour {

	private EnemyS _enemyRef;
	public EnemyS enemyRef { get { return _enemyRef; } }

	public Transform _buddyPos;
	public Transform _buddyPosLower;
	public GameObject buddySound;

	public float followSpeed;
	public float nearPlayerMult = 0.5f;
	private Rigidbody _myRigid;
	public Rigidbody myRigid { get { return _myRigid; } }

	private SimpleEnemyDetectS _myDetect;
	public SimpleEnemyDetectS myDetect { get { return _myDetect; } }

	public SpriteRenderer shadowRenderer;
	public Color shadowColor;

	private float startScale;
	public float sScale { get { return startScale; } }

	public bool charging = false;
	public bool slowOnCharge = true;

	private Animator _myAnimator;
	public Animator myAnimator { get { return _myAnimator; } }

	private bool doingAction = false;
	public bool buddyDoingAction { get { return doingAction; } }

	public virtual void Initialize(){

		_enemyRef = GetComponentInParent<EnemyS>();
		_myRigid = GetComponent<Rigidbody>();
		_myDetect = GetComponentInChildren<SimpleEnemyDetectS>();

		_myDetect.SetTargetEnemy(_enemyRef);

		_myAnimator = GetComponent<Animator>();

		transform.parent = null;

		startScale = transform.localScale.x;

		Color shadowCol = shadowColor;
		shadowCol.a = shadowRenderer.color.a;
		shadowRenderer.color = shadowCol;
		shadowRenderer.material.SetColor("_FlashColor", shadowColor);


	}

	public virtual void FollowEnemy(){

		Vector3 moveForce = Vector3.zero;
		if (_enemyRef.myRigidbody.velocity.y <= -0.1f){
			moveForce = (_buddyPos.position-transform.position).normalized*followSpeed*Time.deltaTime;
		}
		else{
			moveForce = (_buddyPosLower.position-transform.position).normalized*followSpeed*Time.deltaTime;
		}

		if (_myDetect.TargetEnemyInRange()){
			moveForce*=nearPlayerMult;
		}

		_myRigid.AddForce(moveForce, ForceMode.Acceleration);

		Vector3 fixPos = transform.position;
		if (transform.position.y < _enemyRef.transform.position.y){
			fixPos.z = _enemyRef.transform.position.z-1f;
		}else{
			fixPos.z = _enemyRef.transform.position.z+1f;
		}
		transform.position = fixPos;


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
		_buddyPosLower = lowerPos;
	}

	public virtual void TriggerAction(){
		if (!doingAction){
			doingAction = true;
		}	
	}
	public virtual void EndAction(){
		doingAction = false;
	}



}
