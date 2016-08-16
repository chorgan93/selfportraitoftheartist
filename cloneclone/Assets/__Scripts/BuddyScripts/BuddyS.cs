using UnityEngine;
using System.Collections;

public class BuddyS : MonoBehaviour {

	public float costPerUse = 20f;

	private PlayerController _playerRef;
	public PlayerController playerRef { get { return _playerRef; } }

	private Transform _buddyPos;
	public Transform buddyPos { get { return _buddyPos; } }

	public float followSpeed;
	public float nearPlayerMult = 0.5f;
	private Rigidbody _myRigid;
	public Rigidbody myRigid { get { return _myRigid; } }

	private PlayerDetectS _myDetect;
	public PlayerDetectS myDetect { get { return _myDetect; } }

	private SpriteRenderer shadowRenderer;
	public Color shadowColor;

	private float startScale;
	public float sScale { get { return startScale; } }

	public bool charging = false;
	public bool slowOnCharge = true;

	private Animator _myAnimator;
	public Animator myAnimator { get { return _myAnimator; } }

	public virtual void Initialize(){

		_playerRef = GetComponentInParent<PlayerController>();
		_buddyPos = GameObject.Find("BuddyPos").transform;

		_myRigid = GetComponent<Rigidbody>();
		_myDetect = GetComponentInChildren<PlayerDetectS>();

		_myAnimator = GetComponent<Animator>();

		transform.parent = null;

		startScale = transform.localScale.x;

		shadowRenderer = GetComponentInChildren<SpriteRenderer>();
		shadowRenderer.color = shadowColor;

	}

	public virtual void FollowPlayer(){

		Vector3 moveForce = (_buddyPos.position-transform.position).normalized*followSpeed*Time.deltaTime;

		if (_myDetect.PlayerInRange()){
			moveForce*=nearPlayerMult;
		}

		_myRigid.AddForce(moveForce, ForceMode.Acceleration);

		Vector3 fixPos = transform.position;
		if (transform.position.y < _playerRef.transform.position.y){
			fixPos.z = _playerRef.transform.position.z-1f;
		}else{
			fixPos.z = _playerRef.transform.position.z+1f;
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

	public virtual void Equip(){

		_playerRef.EquipBuddy(this);

	}

	public virtual void Swap(){

	}

	public virtual void Unequip(){

	}
}
