using UnityEngine;
using System.Collections;

public class ProjectileS : MonoBehaviour {

	private Rigidbody _myRigid;
	private bool _initialized = false;

	public float shootForce = 1000f;

	public void Fire(Vector3 fireDirection, float accuracy){
	
		if (!_initialized){
			_myRigid = GetComponent<Rigidbody>();
			_initialized = true;
		}

		FaceDirection(fireDirection, accuracy);
		_myRigid.AddForce(transform.right*shootForce*Time.unscaledDeltaTime, ForceMode.Impulse);

	}

	void FaceDirection(Vector3 dir, float acc){
		float rotateZ = 0;

		Vector3 targetDir = dir.normalized;

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

		rotateZ += acc*Random.insideUnitCircle.x;


		transform.rotation = Quaternion.Euler(new Vector3(0,0,rotateZ));

	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Wall"){
			Destroy(gameObject);
		}
	}
}
