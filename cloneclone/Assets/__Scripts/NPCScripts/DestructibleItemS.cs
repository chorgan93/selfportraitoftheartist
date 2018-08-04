using UnityEngine;
using System.Collections;

public class DestructibleItemS : EnemyS {

	new public float maxDestructibleHealth;
	private float _currentDestructibleHealth;
	new public float currentDestructibleHealth { get { return _currentDestructibleHealth; } }
	public bool destroyed { get { return _currentDestructibleHealth <= 0; } }

	public float[] healthLevels;
	public Sprite[] destructionSprites;

	public GameObject hitBit;
	public GameObject destructionBit;
	public float destructionBitSpeedMin;
	public float destructionBitSpeedMax;
	public float yForceMin;
	public float yForceMax;
	public float destructionBitZDif;
	public int numToSpawnOnHit;
	public int numToSpawnOnDestroy;

	private int whiteFrames;
	private int whiteFramesMax = 6;
	private bool flashing = false;

	private SpriteRenderer _myDestructibleRenderer;
	//public SpriteRenderer myDestructibleRenderer { get { return _myDestructibleRenderer; } }
	private Color startCol;
	private Collider _myDestructibleCollider;

	public int onlyTakeDamageFromWeapon = -1;

	public float destroySleepTime = 0.1f;

	// Use this for initialization
	void Start () {

		_myDestructibleCollider = GetComponent<Collider>();
		_myDestructibleRenderer = GetComponent<SpriteRenderer>();
		startCol = _myDestructibleRenderer.color;
		_myDestructibleRenderer.material.SetFloat("_FlashAmount", 0f);
		_currentDestructibleHealth = maxDestructibleHealth;
	
	}

	void Update(){
		if (flashing){
			whiteFrames --;
			if (whiteFrames <= 0){
				flashing = false;
				_myDestructibleRenderer.material.SetFloat("_FlashAmount", 0f);
				_myDestructibleRenderer.color = startCol;
			}
		}
	}

	void FixedUpdate(){
		// just covering up enemy fixedupdate
	}
	void LateUpdate(){
		// just covering up enemy lateupdate
	}

	private void SpawnBits(float zReference, Vector3 hitPos){

		int numToSpawn = numToSpawnOnHit;
		if (destroyed){
			numToSpawn = numToSpawnOnDestroy;
		}

		Vector3 bitEuler = Vector3.zero;
		GameObject newBit;
		float dir = 1f;
		for (int i = 0; i < numToSpawn; i++){
			if (destroyed){
				newBit = Instantiate(destructionBit, transform.position, Quaternion.identity)
					as GameObject;
			}else{
				newBit = Instantiate(hitBit, hitPos, Quaternion.identity)
					as GameObject;
			}
			bitEuler.z = (zReference+90f)*dir+(destructionBitZDif*Random.insideUnitCircle.x);
			newBit.transform.rotation = Quaternion.Euler(bitEuler);
			//bitForce = new Vector3(Random.Range(destructionBitSpeedMin, destructionBitSpeedMax)*dir, Random.Range(yForceMin,yForceMax), 0);
			newBit.GetComponent<Rigidbody>().AddForce(Random.Range(destructionBitSpeedMin, destructionBitSpeedMax)*newBit.transform.right, ForceMode.Impulse);
			dir*=-1f;
		}

	}

	private void HitSprite(){

		int currentLv = 0;
		int updateSprite = 0;
		foreach(float f in healthLevels){
			if (_currentDestructibleHealth <= f*maxDestructibleHealth){
				updateSprite = currentLv;
			}
			currentLv++;
		}
		_myDestructibleRenderer.sprite = destructionSprites[updateSprite];
		_myDestructibleRenderer.color = new Color(_myDestructibleRenderer.color.r, _myDestructibleRenderer.color.g, _myDestructibleRenderer.color.b, 1f);
		whiteFrames = whiteFramesMax;
		flashing = true;
		_myDestructibleRenderer.material.SetFloat("_FlashAmount", 1f);

	}

	public void TakeDamage(float dmgAmt, float destructionRotation, Vector3 hitPos, int weaponNum){

		if (onlyTakeDamageFromWeapon <= -1 || (onlyTakeDamageFromWeapon > -1 && onlyTakeDamageFromWeapon == weaponNum)){
			_currentDestructibleHealth -= dmgAmt;

				if (_currentDestructibleHealth <= 0){
				_myDestructibleCollider.enabled = false;
				DestructibleDead();
					CameraShakeS.C.SmallShake();
					CameraShakeS.C.TimeSleep(destroySleepTime/2f);
					}else{
					CameraShakeS.C.MicroShake();
					//CameraShakeS.C.SmallSleep();
				}
			SpawnBits(destructionRotation, hitPos);
			HitSprite();
		}
	}
}
