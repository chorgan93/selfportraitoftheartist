using UnityEngine;
using System.Collections;

public class BlockDisplay3DS : MonoBehaviour {

	private Renderer myRenderer;
	private PlayerController myPlayer;
	private Quaternion startRotation;
	private Vector3 rotateRate = new Vector3(0,600,0f);
	private float rotateCountdownMax = 0.083f;
	private float rotateCountdown;

	private int flashFramesMax = 5;
	private int currentFlashFrames = 0;
	public Color colorFullPower;
	public Color colorNoPower;
	private Vector3 colorDiff;
	private Color currentColor;
	private bool isFlashing = false;

	private bool initialized = false;

	private Texture startTexture;
	public Texture flashTexture;
	public Texture hitFlashTexture;

	private bool parryEffect = false;
	public bool doingParry { get { return parryEffect; } }
	private float parryEffectTimeMax = 0.1f;
	private float parryEffectTime;
	private float parryFadeRate = 3f;
	private Vector3 parryGrowRate = new Vector3(5f, 5f, 5f);
	private Vector3 startSize;

	public GameObject parryEffectPrefab;
	public EnemyParryBehavior enemyRef;
	private bool isEnemy = false;

	// Use this for initialization
	void Start () {

		myRenderer = GetComponent<Renderer>();
		if (enemyRef){
			isEnemy = true;
			enemyRef.SetBlockRef(this);
			initialized = true;
		}else{
		myPlayer = GetComponentInParent<PlayerController>();
		myPlayer.SetBlockReference(this);
		startRotation = transform.rotation;
		}
		myRenderer.material.color = currentColor = colorFullPower;

		startSize = transform.localScale;

		SetColorDiff();

		startTexture = myRenderer.material.GetTexture("_MainTex");
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!isEnemy){
		if (!initialized){
			myPlayer.myStats.AddBlocker(this);
			initialized = true;
		}

		if (myPlayer.showBlock || parryEffect){
			if (!myRenderer.enabled){
				myRenderer.enabled = true;
				DoFlash(parryEffect);
				transform.rotation = startRotation;
				rotateCountdown = rotateCountdownMax;
			}
			ApplyRotation();
			if (parryEffect){
				if (!isFlashing){
				parryEffectTime -= Time.unscaledDeltaTime;
				if (parryEffectTime <= 0){
						currentColor = myRenderer.material.color;
						currentColor.a -= parryFadeRate*Time.unscaledDeltaTime;
						transform.localScale += parryGrowRate*Time.unscaledDeltaTime;
						if (currentColor.a <= 0){
							currentColor.a = 0;
							parryEffect = false;
						}
						myRenderer.material.color = currentColor;
					}
				}
			}

			if (isFlashing){
			currentFlashFrames--;
			if (currentFlashFrames <= 0){
					isFlashing = false;
					myRenderer.material.color = currentColor;
					myRenderer.material.SetTexture("_MainTex", startTexture);
			}
			}
		}
		else{
			if (isFlashing){
				currentFlashFrames--;
				if (currentFlashFrames <= 0){
					isFlashing = false;
					myRenderer.material.color = currentColor;
					myRenderer.material.SetTexture("_MainTex", startTexture);
					myRenderer.enabled = false;
				}
			}else{
				myRenderer.enabled = false;
			}

		}
		}else{
			if (parryEffect){
				if (!myRenderer.enabled){
					myRenderer.enabled = true;
					DoFlash(parryEffect);
					transform.rotation = startRotation;
					rotateCountdown = rotateCountdownMax;
				}
				ApplyRotation();
				if (parryEffect){
					if (!isFlashing){
						parryEffectTime -= Time.unscaledDeltaTime;
						if (parryEffectTime <= 0){
							currentColor = myRenderer.material.color;
							currentColor.a -= parryFadeRate*Time.unscaledDeltaTime;
							transform.localScale += parryGrowRate*Time.unscaledDeltaTime;
							if (currentColor.a <= 0){
								currentColor.a = 0;
								parryEffect = false;
							}
							myRenderer.material.color = currentColor;
						}
					}
				}

				if (isFlashing){
					currentFlashFrames--;
					if (currentFlashFrames <= 0){
						isFlashing = false;
						myRenderer.material.color = currentColor;
						myRenderer.material.SetTexture("_MainTex", startTexture);
					}
				}
			}
			else{
				if (isFlashing){
					currentFlashFrames--;
					if (currentFlashFrames <= 0){
						isFlashing = false;
						myRenderer.material.color = currentColor;
						myRenderer.material.SetTexture("_MainTex", startTexture);
						myRenderer.enabled = false;
					}
				}else{
					myRenderer.enabled = false;
				}
			}
		}
	
	}

	public void DoStartFlash(){
		
		isFlashing = true;
		myRenderer.material.SetTexture("_MainTex", flashTexture);
		myRenderer.material.color = Color.white;
		currentFlashFrames = flashFramesMax;
		currentColor = Color.Lerp(colorNoPower, colorFullPower, myPlayer.myStats.currentDefense/myPlayer.myStats.maxDefense);
	}

	public void DoFlash(bool extraFrames = false){

		isFlashing = true;
		myRenderer.material.SetTexture("_MainTex", hitFlashTexture);
		myRenderer.material.color = Color.white;
		if (extraFrames){
			currentFlashFrames = flashFramesMax*2;
		}else{
			currentFlashFrames = flashFramesMax;
			parryEffect = false;
		}
		if (!isEnemy){
		currentColor = Color.Lerp(colorNoPower, colorFullPower, myPlayer.myStats.currentDefense/myPlayer.myStats.maxDefense);
		}else{
			currentColor = Color.Lerp(colorNoPower, colorFullPower, 1f);
		}

	}

	public void FireParryEffect(Vector3 enemyPosition){
		transform.localScale = startSize;
		parryEffect = true;
		parryEffectTime = parryEffectTimeMax;
		if (parryEffectPrefab){
			Vector3 spawnPos = (enemyPosition + transform.position)/2f;
			spawnPos.z = +1f;
			GameObject newEffect = Instantiate(parryEffectPrefab, spawnPos, parryEffectPrefab.transform.rotation)
				as GameObject;
			if (!isEnemy){
				newEffect.GetComponent<ParryEffectS>().FireParry(transform.position, enemyPosition, myPlayer.EquippedWeapon().swapColor,
				myPlayer.EquippedWeapon().flashSubColor);
			}else{
				newEffect.GetComponent<ParryEffectS>().FireParry(transform.position, enemyPosition, enemyRef.myEnemyReference.bloodColor,
					Color.red);
			}
		}
	}

	void ApplyRotation(){
		rotateCountdown -= Time.deltaTime;
		if (rotateCountdown <= 0){
		transform.Rotate(rotateRate*Time.deltaTime);
			rotateCountdown = rotateCountdownMax;
		}
	}

	public void ChangeColors(Color newWeaponColor){

		Color newColor = newWeaponColor;
		newColor.a = colorFullPower.a;
		colorFullPower = newColor;

		SetNoPowerColor();
	
		if (!isEnemy){
		if (myPlayer.showBlock){
			DoStartFlash();
		}
		}

	}

	private void SetColorDiff(){
		colorDiff.x = colorFullPower.r - colorNoPower.r;
		colorDiff.y = colorFullPower.g - colorNoPower.g;
		colorDiff.z = colorFullPower.b - colorNoPower.b;
	}

	private void SetNoPowerColor(){
		Color newColor = colorFullPower;
		newColor.a = colorNoPower.a;
		newColor.r = colorFullPower.r-colorDiff.x;
		newColor.g = colorFullPower.g-colorDiff.y;
		newColor.b = colorFullPower.b-colorDiff.z;
		colorNoPower = newColor;
	}
}
