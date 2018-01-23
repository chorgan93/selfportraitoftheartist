using UnityEngine;
using System.Collections;

public class AnimObjS : MonoBehaviour {

	private SpriteRenderer mySprite;
	public Sprite[] animFrames;
	public float animRate;
	public float firstFrameDelay = 0f;
	private float maxFirstFrameDelay;
	private float animRateCountdown;
	private int currentFrame;

	private float witchMult = 0.1f;

	public bool destroyOnEnd = true;
	public bool loop = false;
	public float loopBufferTime = 0f;
	private float loopTime = 0f;
	private bool delayingLoop = false;
	public bool ignoreWitchTime = false;

	public GameObject fadeObj;
	private bool endAnim = false;
	public bool affectedByDifficulty = false;
	private float[] difficultyMults = new float[4]{0.9f, 1f, 1.05f, 1.1f};

	private EffectSpawnManagerS spawnManager;

	private bool _initialized = false;
	public bool initialize { get { return _initialized; } }

	private ReflectColorChange myReflect;

	// Use this for initialization
	void Awake(){

		mySprite = GetComponent<SpriteRenderer>();
		currentFrame = 0;
		mySprite.sprite = animFrames[currentFrame];
	}
	void Start () {

		mySprite = GetComponent<SpriteRenderer>();
		myReflect = GetComponent<ReflectColorChange>();
		currentFrame = 0;
		mySprite.sprite = animFrames[currentFrame];
		animRateCountdown = animRate+firstFrameDelay/DifficultyMult();
		maxFirstFrameDelay = firstFrameDelay/DifficultyMult();
		_initialized = true;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (delayingLoop){
			if (PlayerSlowTimeS.witchTimeActive){
				if (ignoreWitchTime){
					loopTime -= Time.deltaTime*DifficultyMult();
				}else{
					loopTime -= Time.deltaTime*DifficultyMult()*witchMult;
				}
			}else{
			loopTime -= Time.deltaTime*DifficultyMult();
			}
			if (loopTime <= 0){
				delayingLoop = false;
				currentFrame = 0;
				mySprite.sprite = animFrames[currentFrame];
			}
		}else{
		if (!endAnim){
				if (PlayerSlowTimeS.witchTimeActive){
					if (ignoreWitchTime){

						animRateCountdown -= Time.deltaTime*DifficultyMult();
					}else{

						animRateCountdown -= Time.deltaTime*DifficultyMult()*witchMult;
					}
				}else{
					animRateCountdown -= Time.deltaTime*DifficultyMult();
				}
		}

		if (animRateCountdown <= 0){

			if (currentFrame < animFrames.Length-1){
				if (fadeObj){
					Vector3 spawnpos = transform.position;
					spawnpos.z += 2f;
					//GameObject fadeObjSpawn = spawnManager.SpawnPlayerFade(spawnpos);
					GameObject fadeObjSpawn = Instantiate(fadeObj, spawnpos, Quaternion.identity)
						as GameObject;
					fadeObjSpawn.GetComponent<SpriteRenderer>().sprite = animFrames[currentFrame];
					fadeObjSpawn.transform.parent = transform;
					fadeObjSpawn.transform.localScale = Vector3.one;
					fadeObjSpawn.transform.localRotation = Quaternion.identity;
						fadeObjSpawn.transform.parent = null;
				}
			}

				animRateCountdown = animRate/DifficultyMult();
			currentFrame ++;

			if (currentFrame > animFrames.Length-1){
				if (!loop){
					currentFrame = animFrames.Length-1;
				}
				else{
					if (loopBufferTime > 0){
						loopTime = loopBufferTime;
						delayingLoop = true;
							currentFrame = animFrames.Length-1;
					}else{
						currentFrame = 0;
					}
				}

				if (destroyOnEnd){
					Destroy(gameObject);
					}
			}
			
			mySprite.sprite = animFrames[currentFrame];
		}
		}

	
	}

	public void EndAnimation(){
		endAnim = true;
	}

	public void ResetAnimation(){
		if (!mySprite){

			mySprite = GetComponent<SpriteRenderer>();
			currentFrame = 0;
			mySprite.sprite = animFrames[currentFrame];
		}
		endAnim = false;
		currentFrame = 0;
		mySprite.sprite = animFrames[currentFrame];
		firstFrameDelay = maxFirstFrameDelay/DifficultyMult();
		animRateCountdown = animRate+firstFrameDelay/DifficultyMult();
		gameObject.SetActive(true);
	}

	private float DifficultyMult(){
		if (affectedByDifficulty){
			return difficultyMults[DifficultyS.GetSinInt()];
		}else{
			return 1f;
		}
	}

	public void SetColor(Color newCol){
		Color replaceCol = newCol;
		if (!mySprite){
			mySprite = GetComponent<SpriteRenderer>();
		}
		replaceCol.a = mySprite.color.a;
		mySprite.color = replaceCol;
	} 

	public void ActivateReflect(){
		if (myReflect){
			myReflect.ActivateReflect();
		}
	}
}
