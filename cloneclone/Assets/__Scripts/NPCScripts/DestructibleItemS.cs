using UnityEngine;
using System.Collections;

public class DestructibleItemS : MonoBehaviour {

	public float maxHealth;
	private float _currentHealth;
	public float currentHealth { get { return _currentHealth; } }
	public bool destroyed { get { return _currentHealth <= 0; } }

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

	private SpriteRenderer _myRenderer;
	public SpriteRenderer myRenderer { get { return _myRenderer; } }
	private Color startCol;
	private Collider myCollider;

	public float destroySleepTime = 0.1f;

	// Use this for initialization
	void Start () {

		myCollider = GetComponent<Collider>();
		_myRenderer = GetComponent<SpriteRenderer>();
		startCol = _myRenderer.color;
		_myRenderer.material.SetFloat("_FlashAmount", 0f);
		_currentHealth = maxHealth;
	
	}

	void Update(){
		if (flashing){
			whiteFrames --;
			if (whiteFrames <= 0){
				flashing = false;
				_myRenderer.material.SetFloat("_FlashAmount", 0f);
				_myRenderer.color = startCol;
			}
		}
	}

	private void SpawnBits(float zReference, Vector3 hitPos){

		int numToSpawn = numToSpawnOnHit;
		if (destroyed){
			numToSpawn = numToSpawnOnDestroy;
		}

		Vector3 bitForce = Vector3.zero;
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
			if (_currentHealth <= f*maxHealth){
				updateSprite = currentLv;
			}
			currentLv++;
		}
		_myRenderer.sprite = destructionSprites[updateSprite];
		_myRenderer.color = new Color(_myRenderer.color.r, _myRenderer.color.g, _myRenderer.color.b, 1f);
		whiteFrames = whiteFramesMax;
		flashing = true;
		_myRenderer.material.SetFloat("_FlashAmount", 1f);

	}

	public void TakeDamage(float dmgAmt, float destructionRotation, Vector3 hitPos){
		_currentHealth -= dmgAmt;
		if (_currentHealth <= 0){
			myCollider.enabled = false;
					CameraShakeS.C.SmallShake();
			CameraShakeS.C.TimeSleep(destroySleepTime);
				}else{
					CameraShakeS.C.MicroShake();
			CameraShakeS.C.SmallSleep();
				}
		SpawnBits(destructionRotation, hitPos);
		HitSprite();
	}
}
