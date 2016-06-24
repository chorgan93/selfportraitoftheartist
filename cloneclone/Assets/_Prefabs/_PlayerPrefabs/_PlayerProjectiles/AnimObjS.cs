using UnityEngine;
using System.Collections;

public class AnimObjS : MonoBehaviour {

	private SpriteRenderer mySprite;
	public Sprite[] animFrames;
	public float animRate;
	private float animRateCountdown;
	private int currentFrame;

	public bool destroyOnEnd = true;
	public bool loop = false;

	public GameObject fadeObj;
	private bool endAnim = false;

	// Use this for initialization
	void Start () {

		mySprite = GetComponent<SpriteRenderer>();
		currentFrame = 0;
		mySprite.sprite = animFrames[currentFrame];
		animRateCountdown = animRate;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!endAnim){
			animRateCountdown -= Time.deltaTime;
		}

		if (animRateCountdown <= 0){

			if (currentFrame < animFrames.Length-1){
				if (fadeObj){
					GameObject fadeObjSpawn = Instantiate(fadeObj, transform.position, transform.rotation)
						as GameObject;
					fadeObjSpawn.GetComponent<SpriteRenderer>().sprite = animFrames[currentFrame];
				}
			}

			animRateCountdown = animRate;
			currentFrame ++;

			if (currentFrame > animFrames.Length-1){
				if (!loop){
					currentFrame = animFrames.Length-1;
				}
				else{
					currentFrame = 0;
				}

				if (destroyOnEnd){
					Destroy(gameObject);
				}
			}
			
			mySprite.sprite = animFrames[currentFrame];
		}

	
	}

	public void EndAnimation(){
		endAnim = true;
	}
}
