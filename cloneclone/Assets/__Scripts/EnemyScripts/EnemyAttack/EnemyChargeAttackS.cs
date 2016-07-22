using UnityEngine;
using System.Collections;

public class EnemyChargeAttackS : MonoBehaviour {

	private EnemyS myEnemy;
	public Renderer _myRenderer;
	private Collider _myCollider;
	private float _animateRate = 0.033f;
	private float animateCountdown;
	private Vector2 startTiling;
	private float tilingRandomMult = 0.5f;

	private float visibleTime = 0.4f;
	private float startAlpha = 1f;
	private float fadeRate;
	private Color fadeColor;

	private float capturedChargeTime;
	public float chargeUpTime;
	public float maxChargeAlpha;
	private bool charging = false;

	public Texture startFlash;
	private Texture startTexture;
	private int flashFrames = 3;
	private int colliderFrames = 3;
	private int flashMax = 3;
	private int blackFlashFrames = 4;

	public GameObject hitObj;

	[Header("Attack Properties")]
	public float knockbackForce = 1000f;
	public float dmg = 5f;
	private Vector3 knockBackDir;
	public float knockbackTime = 0.8f;


	// Use this for initialization
	void Start () {

		//_myRenderer = GetComponent<Renderer>();
		_myCollider = GetComponent<Collider>();
		animateCountdown = _animateRate;

		myEnemy = GetComponentInParent<EnemyS>();

		startTiling = _myRenderer.material.GetTextureScale("_MainTex");
		startTexture = _myRenderer.material.GetTexture("_MainTex");
		_myRenderer.enabled = false;

		fadeRate = startAlpha/visibleTime;

		fadeColor = _myRenderer.material.color;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_myRenderer.enabled){

			if (charging){
				chargeUpTime -= Time.deltaTime;
					fadeColor.a = maxChargeAlpha*(1f-chargeUpTime/capturedChargeTime);
				_myRenderer.material.color = fadeColor;
				if (chargeUpTime <= 0){
					TriggerAttack();
				}
			}
			else{

			flashFrames--;
			if (flashFrames < 0){
				
				if (_myCollider.enabled && flashFrames < -colliderFrames){
					_myCollider.enabled = false;
				}

				if (_myRenderer.material.GetTexture("_MainTex") == startFlash){
					if (_myRenderer.material.color == Color.black){
						_myRenderer.material.color = Color.white;
						flashFrames = blackFlashFrames;
					}else{
					_myRenderer.material.SetTexture("_MainTex", startTexture);
					_myCollider.enabled = true;
					}
				}else{


			fadeColor.a -= Time.deltaTime*fadeRate;
			if (fadeColor.a <= 0){
				fadeColor.a = 0;
				_myRenderer.enabled = false;
					_myCollider.enabled = false;
			}else{
				_myRenderer.material.color = fadeColor;
			}
				animateCountdown -= Time.deltaTime;
				if (animateCountdown <= 0){
					animateCountdown = _animateRate;
					Vector2 newTiling = startTiling;
					newTiling.x += Random.insideUnitCircle.x*tilingRandomMult;
					newTiling.y += Random.insideUnitCircle.y*tilingRandomMult;
					_myRenderer.material.SetTextureScale("_MainTex", newTiling);
				}
			}
			}
			}
		}
	
	}

	public void TriggerAttack(){

		fadeColor = _myRenderer.material.color;
		fadeColor.a = startAlpha;

		_myRenderer.material.color = Color.black;

		_myRenderer.material.SetTexture("_MainTex", startFlash);
		_myRenderer.enabled = true;
		flashFrames = flashMax;

		CameraShakeS.C.TimeSleep(0.06f);
		CameraShakeS.C.SmallShake();

		charging = false;


	}

	private float FaceDirection(Vector3 direction){
		
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
		
		rotateZ += -90f+25f*Random.insideUnitCircle.x;

		return rotateZ;
	}

	void OnTriggerEnter(Collider other){
		
		if (other.gameObject.tag == "Player"){
		
			Debug.Log("yeah");
			knockBackDir = (other.transform.position-transform.position).normalized;
			knockBackDir.z = 1f;

			other.gameObject.GetComponent<PlayerController>().myStats.TakeDamage
				(myEnemy, dmg, knockBackDir*knockbackForce*Time.deltaTime, knockbackTime);

			//HitEffect(other.transform.position, other.gameObject.GetComponent<EnemyS>().bloodColor);
		}
		
	}

	/*void HitEffect(Vector3 spawnPos, Color bloodCol){

		Vector3 hitObjSpawn = spawnPos;
		GameObject newHitObj = Instantiate(hitObj, hitObjSpawn, Quaternion.identity)
			as GameObject;
		
		newHitObj.transform.Rotate(new Vector3(0,0,FaceDirection((spawnPos-transform.position).normalized)));
		
		SpriteRenderer hitRender = newHitObj.GetComponent<SpriteRenderer>();
		hitRender.color = bloodCol;

		newHitObj.transform.localScale = Vector3.one*10f;

		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/2f;
		newHitObj.transform.position = hitObjSpawn;
	}*/

	public void TurnOn(float attackWarmup){

		capturedChargeTime = chargeUpTime = attackWarmup;
		_myRenderer.material.SetTexture("_MainTex", startFlash);
		fadeColor.a = 0;
		_myRenderer.material.color = fadeColor;
		_myCollider.enabled = false;
		_myRenderer.enabled = true;
		charging = true;
		Debug.Log("I'm on!!");

	}
}
