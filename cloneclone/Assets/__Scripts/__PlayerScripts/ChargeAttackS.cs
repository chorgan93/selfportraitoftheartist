using UnityEngine;
using System.Collections;

public class ChargeAttackS : MonoBehaviour {

	private const float damageVariance = 0.1f;

	private Renderer _myRenderer;
	private Collider _myCollider;
	private float _animateRate = 0.033f;
	private float animateCountdown;
	private Vector2 startTiling;
	private float tilingRandomMult = 0.5f;

	private float visibleTime = 0.4f;
	private float startAlpha = 1f;
	private float fadeRate;
	private Color fadeColor;

	public Texture startFlash;
	private Texture startTexture;
	private int flashFrames = 3;
	private int colliderFrames = 3;
	private int flashMax = 3;
	private int blackFlashFrames = 4;

	public GameObject soundObj;
	public GameObject hitObj;

	private SpriteRenderer effectRender;

	private PlayerController myPlayer;

	[Header("Attack Properties")]
	public float spawnRange = 1f;
	public float knockbackForce = 1000f;
	public float dmg = 5f;
	public float killAtLessThan = 2f;
	public float stunMult = 2f;
	public float absorbPercent = 0.1f;
	private Vector3 knockBackDir;
	public int shakeAmt = 1;
	public float hitStopTime = 0.2f;

	private bool ignoreDefenses = false;

	private bool _firstSpawned = false;

	private bool standAlone = false;

	private FlashEffectS flashEffect;

	[Header("Fos Charge Properties")]
	public bool isFosCharge = false;
	private bool fosFired = false;
	private float floatRadius = 3f;
	public float fosVisibleTime = 6f;
	public float shootLifetime = 3f;
	public float shootSpeed = 1000f;
	private Rigidbody _myRigid;
	private Vector3 currentFosPos = Vector3.zero;
	private float floatAxisA = 3f;
	private float floatAxisB = 1.75f;
	private float currentAngle = 0f;
	private float angleRate = 8f;
	private int maxHits = 1;
	private int currentHit = 0;
	private float startRotate = -1;
	private float fosFadeRate = 2f;
	private float fosFiredFade = 5f;
	public float punchMult = 1f;
	public float slowTime = 0.2f;
	public float hangTime = 0.3f;
	private Vector3 savedFireDirection = Vector3.zero;

	private bool saveEnraged = false;
	private bool saveCritical = false;

	// Use this for initialization
	void Start () {

		_myRenderer = GetComponentInChildren<Renderer>();
		_myCollider = GetComponentInChildren<Collider>();
		if (transform.parent != null){
			myPlayer = GetComponentInParent<PlayerController>();
		}else{
			standAlone = true;
		}
		effectRender = GetComponentInChildren<SpriteRenderer>();
		animateCountdown = _animateRate;

		startTiling = _myRenderer.material.GetTextureScale("_MainTex");
		startTexture = _myRenderer.material.GetTexture("_MainTex");
		_myRenderer.enabled = false;

		if (isFosCharge){
			visibleTime = fosVisibleTime;
			startRotate = transform.rotation.eulerAngles.z;
			fadeRate = fosFadeRate;
			_myRigid = GetComponent<Rigidbody>();
			myPlayer.AddFosCharge(this);
		}else{
			fadeRate = startAlpha/visibleTime;
		}

		fadeColor = _myRenderer.material.color;

		flashEffect = CameraEffectsS.E.specialFlash;

		if (standAlone){
			TriggerAttack(transform.position, Vector3.zero, myPlayer.myRenderer.color);
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (transform.parent){
			transform.parent = null;
		}

		if (isFosCharge){
			FosMovement();
		}

		if (_myRenderer.enabled){
		

			flashFrames--;
			if (flashFrames < 0){
				
				if (_myCollider.enabled && flashFrames < -colliderFrames && (!isFosCharge || (isFosCharge && currentHit >= maxHits))){
					_myCollider.enabled = false;
					if (isFosCharge && fosFired){
						_myRigid.velocity = Vector3.zero;
						visibleTime = 0f;
					}
					myPlayer.RemoveFosCharge(this);
				}

				if (_myRenderer.material.GetTexture("_MainTex") == startFlash){
					if (_myRenderer.material.color == Color.black){
						_myRenderer.material.color = Color.white;
						flashFrames = blackFlashFrames;
					}else{
					_myRenderer.material.SetTexture("_MainTex", startTexture);
						_myCollider.enabled = true;
						effectRender.color = fadeColor;
						if (_firstSpawned){
							flashEffect.NewColor(fadeColor);
							flashEffect.Flash();
						}
					}
				}else{

					if (!isFosCharge || (isFosCharge && visibleTime <= 0)){
						fadeColor.a -= Time.deltaTime*fadeRate;
					}
			if (fadeColor.a <= 0){
						if (!standAlone){
				fadeColor.a = 0;
				_myRenderer.enabled = false;
					_myCollider.enabled = false;
						}else{
							Destroy(gameObject);
						}
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

	void FosMovement(){
		if (myPlayer && visibleTime > 0 && visibleTime < 10 && !fosFired){
							if (currentHit >= maxHits){
								visibleTime = 0f;
			}else{
							visibleTime -= Time.deltaTime;
				if (visibleTime <= 0){
					FosEndFire();
				}
			}
			currentAngle += angleRate*Time.deltaTime;
			currentFosPos.x = floatAxisA * Mathf.Cos(currentAngle);
			currentFosPos.y = floatAxisB * Mathf.Sin(currentAngle);
			transform.position = myPlayer.transform.position + currentFosPos;
			DoRotation();
		}
	}

	public void FosPause(Vector3 newDir){
		float rotateZ = 0;

		newDir = savedFireDirection = newDir.normalized;

			if(newDir.x == 0){
				if (newDir.y > 0){
				rotateZ = 90;
			}
			else{
				rotateZ = -90;
			}
		}
		else{
				rotateZ = Mathf.Rad2Deg*Mathf.Atan((newDir.y/newDir.x));
		}	


				if (newDir.x < 0){
			rotateZ += 180;
		}


		_myCollider.enabled = false;
		transform.rotation = Quaternion.Euler(new Vector3(0,0,rotateZ+startRotate+90f));
		visibleTime = 9999f;
		if (currentHit < maxHits){
			currentHit = maxHits - 1;
		}
	}

	public void FosDirectedFire(bool first = false){
		Vector3 shootForce = savedFireDirection;
		shootForce.z = 0f;
		shootForce = shootForce*shootSpeed*Time.unscaledDeltaTime;
		_myRigid.AddForce(shootForce, ForceMode.Impulse);
		visibleTime = shootLifetime;
		fosFired = true;
		_myCollider.enabled = true;

		CameraShakeS.C.SmallShake();
		if (first){
			CameraShakeS.C.SmallSleep();
			CameraShakeS.C.SloAndPunch(slowTime,punchMult,hangTime);
		}
		if (currentHit < maxHits){
			currentHit = maxHits - 1;
		}
		fadeRate = fosFiredFade;
	}

	public void FosEndFire(bool fromPlayer = false){
		
		Vector3 shootForce = myPlayer.transform.position-transform.position;
		shootForce.z = 0f;
		shootForce = shootForce.normalized*shootSpeed*Time.unscaledDeltaTime;
		_myRigid.AddForce(shootForce, ForceMode.Impulse);
		visibleTime = shootLifetime;
		fosFired = true;
		_myCollider.enabled = true;
		if (currentHit < maxHits){
			currentHit = maxHits - 1;
		}

		if (!fromPlayer){
			CameraShakeS.C.SmallShake();
			CameraShakeS.C.SmallSleep();
			CameraShakeS.C.SloAndPunch(slowTime,punchMult,hangTime);
		myPlayer.TriggerAllFos(this);
		}
		fadeRate = fosFiredFade;

	}

	void DoRotation(){
		float rotateZ = 0;

		Vector3 targetDir = (transform.position-myPlayer.transform.position).normalized;

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



		transform.rotation = Quaternion.Euler(new Vector3(0,0,rotateZ+startRotate+90f));

	}

	public void TriggerAttack(Vector3 startPos, Vector3 attackDir, Color fadeCol){

		Vector3 spawnPos = attackDir.normalized;
		spawnPos*=spawnRange;
		spawnPos.z = transform.position.z;
		transform.position = startPos + spawnPos;

		fadeColor = fadeCol;
		fadeColor.a = startAlpha;

		_myRenderer.material.color = Color.black;
		effectRender.color = Color.white;

		_myRenderer.material.SetTexture("_MainTex", startFlash);
		_myRenderer.enabled = true;
		flashFrames = flashMax;
		ignoreDefenses = myPlayer.playerAug.solAug;

		dmg*=myPlayer.playerAug.GetParanoidMult();
		if (myPlayer.isTransformed){
			dmg*=myPlayer.transformedDamageMult;
			myPlayer.myStats.TranformedDarknessAttackAdd();
		}

		if (shakeAmt == 1){
		CameraShakeS.C.TimeSleep(0.1f);
		CameraShakeS.C.LargeShake();
		}
		else if (shakeAmt == 0){
			CameraShakeS.C.SpecialAttackShake();
			CameraShakeS.C.TimeSleep(0.04f);
		}else{
			CameraShakeS.C.SmallShake();
		}

		
		if (soundObj){
			Instantiate(soundObj);
		}


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

		if (other.gameObject.tag == "Destructible"){
			DestructibleItemS destructible = other.gameObject.GetComponent<DestructibleItemS>();
			destructible.TakeDamage(dmg,transform.rotation.z,(transform.position+other.transform.position)/2f, -1);
			HitEffectDestructible(destructible.myRenderer, other.transform.position);
			if (isFosCharge){
				currentHit++;
			}
		}
		
		if (other.gameObject.tag == "Enemy"){

			EnemyS hitEnemy = other.gameObject.GetComponent<EnemyS>();
			if (!hitEnemy){
				hitEnemy = other.GetComponentInParent<EnemyS>();
			}

			if (!hitEnemy.isDead && !hitEnemy.isFriendly
				&& !hitEnemy.invulnerable){
		
			knockBackDir = (other.transform.position-transform.position).normalized;
			knockBackDir.z = 1f;

				float actingDmg = dmg*myPlayer.myStats.strengthAmt();

				actingDmg *= Random.Range(1f-damageVariance, 1f+damageVariance);

				if (myPlayer.playerAug.enragedAug){
					actingDmg*=myPlayer.playerAug.GetEnragedMult();
				}
				if (myPlayer.adaptiveAugBonus){
					actingDmg*=PlayerAugmentsS.ADAPTIVE_DAMAGE_BOOST;
				}
				actingDmg*=BiosAugMult();

				saveEnraged = hitEnemy.isEnraged;
				saveCritical = hitEnemy.isCritical;
				float dmgDealt = hitEnemy.TakeDamage
					(other.transform, knockBackDir*knockbackForce*Time.deltaTime, 
						actingDmg, stunMult, 2f, myPlayer.playerAug.solAug, hitStopTime, 0f, false, 
						myPlayer.playerAug.aquaAug, killAtLessThan*DeterminedMult());
				myPlayer.AnimationStop(hitStopTime);

				RankManagerS.R.ScoreHit(2, dmgDealt, saveEnraged, saveCritical);


				myPlayer.myStats.DesperateRecover(dmgDealt);

				/*if (myPlayer.playerAug.lunaAug){
					myPlayer.myStats.RecoverCharge(absorbPercent*PlayerAugmentsS.lunaAugAmt);
				}else{
					myPlayer.myStats.RecoverCharge(absorbPercent);
				}**/

				HitEffect(other.transform.position, hitEnemy.bloodColor);
				myPlayer.ExtendWitchTime();
				if (isFosCharge){
					currentHit++;
				}
			}
		}
		
	}

	void HitEffect(Vector3 spawnPos, Color bloodCol){

		Vector3 hitObjSpawn = spawnPos;
		GameObject newHitObj = Instantiate(hitObj, hitObjSpawn, Quaternion.identity)
			as GameObject;
		
		newHitObj.transform.Rotate(new Vector3(0,0,FaceDirection((spawnPos-transform.position).normalized)));
		
		SpriteRenderer hitRender = newHitObj.GetComponent<SpriteRenderer>();
		hitRender.color = bloodCol;

		newHitObj.transform.localScale = Vector3.one*10f;

		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/2f;
		newHitObj.transform.position = hitObjSpawn;
	}

	void HitEffectDestructible(SpriteRenderer renderRef, Vector3 spawnPos,bool bigBlood = false){
		Vector3 hitObjSpawn = spawnPos;
		hitObjSpawn.z -= 1f;
		GameObject newHitObj = Instantiate(hitObj, hitObjSpawn, transform.rotation)
			as GameObject;
		
		newHitObj.transform.Rotate(new Vector3(0,0,Random.Range(-20f, 20f)));
		if (transform.localScale.y < 0){
			newHitObj.transform.Rotate(new Vector3(0,0,180f));
		}
		
		
		
		if (bigBlood){
			newHitObj.transform.localScale = _myRenderer.transform.localScale*transform.localScale.x*2.25f;
		}else{
			newHitObj.transform.localScale = _myRenderer.transform.localScale*transform.localScale.x*1.75f;
		}
		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/3f;
		newHitObj.transform.position = hitObjSpawn;
	}

	public void SetPlayer(PlayerController pRef){
		myPlayer = pRef;
	}

	public void SetFirstSpawned(){
		_firstSpawned = true;
	}

	public void TurnOffStun(){
		stunMult = 0f;
	}
	float BiosAugMult(){
		if (myPlayer.playerAug.biosAug){
			return PlayerAugmentsS.BIOS_MULT;
		}else{
			return 1f;
		}
	}

	float DeterminedMult(){
		if (myPlayer.playerAug.determinedAug){
			return 1.4f;
		}else{
			return 1f;
		}
	}
}
