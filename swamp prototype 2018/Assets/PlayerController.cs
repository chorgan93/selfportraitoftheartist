using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float moveSpeed = 50f;
	private Rigidbody _myRigid;

	private Vector3 moveForce = Vector3.zero;

	private Camera mainCam;
	public GameObject bulletPrefab;
	private Vector3 aimDir;
	private GameObject fireBullet;
	private ProjectileS bulletReference;

	public float accuracyMult = 2f;

	// Use this for initialization
	void Start () {
	
		_myRigid = GetComponent<Rigidbody>();
		mainCam = Camera.main;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		MovementControl();
		ShootControl();

	}

	void MovementControl(){
	
		moveForce.x = Input.GetAxis("Horizontal");
		moveForce.y = Input.GetAxis("Vertical");
		moveForce*=moveSpeed*Time.deltaTime;
		_myRigid.AddForce(moveForce, ForceMode.Force);

	}

	void ShootControl(){

		if (Input.GetMouseButtonDown(0)){
		aimDir = mainCam.ScreenToWorldPoint(Input.mousePosition);
		aimDir -= transform.position;
		aimDir.z = 0f;
		aimDir = aimDir.normalized;

		fireBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity)
			as GameObject;
		bulletReference = fireBullet.GetComponent<ProjectileS>();
		bulletReference.Fire(aimDir, accuracyMult);
		}
		
	}
}
