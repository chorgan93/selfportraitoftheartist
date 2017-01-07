using UnityEngine;
using System.Collections;

public class LockOnS : MonoBehaviour {

	private bool _lockedOn = false;
	public bool lockedOn { get { return _lockedOn; } }
	private SpriteRenderer myRenderer;

	private float zDiff = -1f;
	private Vector3 currentPos;
	private Color myColor;
	private float maxFade;

	private int flashFrameMax = 6;
	private int flashCount = 0;
	private bool flashing = false;

	private EnemyHealthUIS _enemyHealthUI;
	public EnemyHealthUIS enemyHealthUI { get { return _enemyHealthUI; } } 


	private Vector3 sizeDistortion = new Vector3(2f, 1.25f, 1f);

	private PlayerController myPlayerReference;

	private EnemyS _myEnemy;
	public EnemyS myEnemy { get { return _myEnemy; } }

	// Use this for initialization
	void Start () {

		myRenderer = GetComponent<SpriteRenderer>();
		myColor = myRenderer.color;
		maxFade = myColor.a;

		myRenderer.enabled = false;

		myPlayerReference = GetComponentInParent<PlayerController>();
		myPlayerReference.SetLockOnIndicator(this);
		transform.parent = null;

		_enemyHealthUI = GameObject.Find("Enemy Health").GetComponent<EnemyHealthUIS>();
		_enemyHealthUI.SetLockOnRef(this);
		_enemyHealthUI.TurnOff();

	
	}
	
	// Update is called once per frame
	void Update(){

		if (myRenderer.enabled){
			if (flashing){
				flashCount--;
				if (flashCount <= 0){
					flashing = false;
					if (_lockedOn){
						myRenderer.material.SetFloat("_FlashAmount", 0f);
						myColor.a = maxFade;
						myRenderer.color = myColor;
					}else{
						myRenderer.enabled = false;
					}
				}
			}
		}

	}

	void FixedUpdate () {

		CheckEnemy();

		if (_lockedOn){
			UpdatePosition();
		}
	
	}

	private void CheckEnemy(){

		if (_lockedOn){
			if (myPlayerReference.myStats.PlayerIsDead()){
				myRenderer.enabled = false;
				_lockedOn = false;
			}
			else if (_myEnemy == null){
				if (myPlayerReference.myDetect.closestEnemy != null){
					LockOn(myPlayerReference.myDetect.closestEnemy);
				}else{
					EndLockOn();
				}
			}else{
				if (_myEnemy.isDead){
					if (myPlayerReference.myDetect.closestEnemy != null){
						if (myPlayerReference.myDetect.closestEnemy != null){
							LockOn(myPlayerReference.myDetect.closestEnemy);
						}else{
							EndLockOn();
						}
					}else{
						EndLockOn();
					}
				}
			}
		}
	}

	private void UpdatePosition(){


		currentPos = _myEnemy.myShadow.transform.position;
		currentPos.x = _myEnemy.transform.position.x;
		currentPos.y -= Mathf.Abs(transform.localScale.y*0.15f);
		currentPos.z += zDiff;
		transform.position = currentPos;

	}

	public void LockOn(EnemyS newEnemy){
		_myEnemy = newEnemy;
		_enemyHealthUI.NewTarget(_myEnemy);
		SetSprite();
		myRenderer.enabled = true;
		transform.localScale = sizeDistortion*newEnemy.myRenderer.transform.localScale.x*newEnemy.transform.localScale.x;
		if (_myEnemy.transform.parent != null){
			transform.localScale*=_myEnemy.transform.parent.localScale.x;
		}
		if (transform.localScale.y < 0){
			Vector3 fixScale = transform.localScale;
			fixScale.y *= -1f;
			transform.localScale = fixScale;
		}
		UpdatePosition();
		_lockedOn = true;
		myColor.a = 1f;
		myRenderer.color = myColor;
		myRenderer.material.SetFloat("_FlashAmount", 1f);
		flashCount = flashFrameMax;
		flashing = true;
		myRenderer.enabled = true;
	}

	public void SetSprite(){
		
		myColor = myPlayerReference.getEWeapon.swapColor;
		myColor.a = maxFade;
		myRenderer.color = myColor;
		myRenderer.sprite = myPlayerReference.getEWeapon.swapSprite;
	}

	public void EndLockOn(){
		_enemyHealthUI.EndLockOn();
		myRenderer.material.SetFloat("_FlashAmount", 1f);
		flashCount = flashFrameMax;
		flashing = true;
		_lockedOn = false;
		_myEnemy = null;
	}
}
