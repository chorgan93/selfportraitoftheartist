using UnityEngine;
using System.Collections;

public class EnemyProjectileS : MonoBehaviour {


	private Rigidbody _rigidbody;
	private SpriteRenderer _myRenderer;
	private EnemyS _myEnemy;

	[Header("Attack Properties")]
	public float range;
	public float shotSpeed;
	public float selfKnockbackMult;
	public float attackSpawnDistance;
	public float accuracyMult = 0f;
	public bool stopEnemy = false;

	[Header("Player Interaction")]
	public float damage;
	public float knockbackTime;
	public float playerKnockbackMult;
	
	[Header("Effect Properties")]
	public int shakeAmt = 0;
	
	private float fadeThreshold = 0.1f;
	private Color fadeColor;


	// Update is called once per frame
	void FixedUpdate () {
		
		range -= Time.deltaTime;
		if (range < fadeThreshold){
			fadeColor = _myRenderer.color;
			fadeColor.a = range/fadeThreshold;
			_myRenderer.color = fadeColor;
		}
		
		if (range <= 0){
			Destroy(gameObject);
		}
		
	}
	
	public void Fire(Vector3 aimDirection, EnemyS enemyReference){
		
		_rigidbody = GetComponent<Rigidbody>();
		_myRenderer = GetComponentInChildren<SpriteRenderer>();
		_myEnemy = enemyReference;
		
		FaceDirection((aimDirection).normalized);
		
		Vector3 shootForce = transform.right * shotSpeed * Time.deltaTime;
		
		_rigidbody.AddForce(shootForce, ForceMode.Impulse);

		Vector3 knockbackForce = -(aimDirection).normalized * shotSpeed * selfKnockbackMult *Time.deltaTime;

		
		if (stopEnemy){
			_myEnemy.myRigidbody.velocity = Vector3.zero;
		}

		_myEnemy.AttackKnockback(knockbackForce);
			
		
		DoShake();
		
		
	}

	private void DoShake(){
		
		switch (shakeAmt){
			
		default:
			CameraShakeS.C.MicroShake();
			break;
		case(0):
			break;
		case(1):
			CameraShakeS.C.SmallShake();
			break;
		case(2):
			CameraShakeS.C.LargeShake();
			break;
			
		}
		
	}
	
	private void FaceDirection(Vector3 direction){
		
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
		
		rotateZ += accuracyMult*Random.insideUnitCircle.x;
		
		
		transform.rotation = Quaternion.Euler(new Vector3(0,0,rotateZ));

		
	}

	
	void OnTriggerEnter(Collider other){
		
		if (other.gameObject.tag == "Player"){

			other.gameObject.GetComponent<PlayerStatsS>().
				TakeDamage(damage, _rigidbody.velocity.normalized*playerKnockbackMult*Time.deltaTime, knockbackTime);	

			
		}
		
	}
}
