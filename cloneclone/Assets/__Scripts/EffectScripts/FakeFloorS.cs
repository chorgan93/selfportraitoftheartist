using UnityEngine;
using System.Collections;

public class FakeFloorS : MonoBehaviour {
	
	private float turnOffGravityY;
	public float gravityOnDelayMin = 0.1f;
	public float gravityOnDelayMax = 0.4f;
	private float gravityOnDelay = 0.5f;
	private bool gravityOn = false;
	public float yVariance = 1f;
	private bool gravityOff = false;

	private Rigidbody myRigid;

	// Use this for initialization
	void Start () {
		myRigid = GetComponent<Rigidbody>();
		myRigid.useGravity = false;
		gravityOnDelay = Random.Range(gravityOnDelayMin, gravityOnDelayMax);
	}
	
	// Update is called once per frame
	void Update () {

		if (!gravityOn){
			gravityOnDelay -= Time.deltaTime;
			if (gravityOnDelay <= 0){
				gravityOn = true;
				turnOffGravityY = transform.position.y-Random.Range(0,1f)*yVariance;
				myRigid.useGravity = true;
			}
		}else{

		if (transform.position.y < turnOffGravityY && !gravityOff){
			myRigid.useGravity = false;
			myRigid.velocity = Vector3.zero;
			gravityOff = true;
		}
		}
	}
}
