using UnityEngine;
using System.Collections;

public class AnimObjS : MonoBehaviour {

	private SpriteRenderer mySprite;
	public Sprite[] animFrames;
	public float animRate;
	public float firstFrameDelay = 0f;
	private float animRateCountdown;
	private int currentFrame;

	public bool destroyOnEnd = true;
	public bool loop = false;
	public float loopBufferTime = 0f;
	private float loopTime = 0f;
	private bool delayingLoop = false;

	public GameObject fadeObj;
	private bool endAnim = false;

	private EffectSpawnManagerS spawnManager;

	// Use this for initialization
	void Start () {

		mySprite = GetComponent<SpriteRenderer>();
		currentFrame = 0;
		mySprite.sprite = animFrames[currentFrame];
		animRateCountdown = animRate+firstFrameDelay;
	
		if (fadeObj){
		spawnManager = GameObject.Find("EffectsManager").GetComponent<EffectSpawnManagerS>();
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (delayingLoop){
			loopTime -= Time.deltaTime;
			if (loopTime <= 0){
				delayingLoop = false;
				currentFrame = 0;
				mySprite.sprite = animFrames[currentFrame];
			}
		}else{
		if (!endAnim){
			animRateCountdown -= Time.deltaTime;
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
				}
			}

			animRateCountdown = animRate;
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
}
