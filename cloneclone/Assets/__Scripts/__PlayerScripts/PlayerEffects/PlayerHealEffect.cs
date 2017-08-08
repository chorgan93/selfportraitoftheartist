using UnityEngine;
using System.Collections;

public class PlayerHealEffect : MonoBehaviour {

	private SpriteRenderer _myRender;
	public Sprite[] animFrames;
	public float animRate = 0.1f;
	private float animCount;
	private int currentFrame = 0;

	private Color setColor;

	private float startAlpha = 0.75f;
	public Color staminaColor;
	public Color healthColor;
	public Color chargeColor;

	private PlayerController playerRef;



	// Use this for initialization
	void Start () {
		playerRef = GetComponentInParent<PlayerController>();
		_myRender = GetComponent<SpriteRenderer>();
		_myRender.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (_myRender.enabled){
			animCount -= Time.deltaTime;
			if (animCount <= 0){
				animCount = animRate;
				currentFrame++;
				if (currentFrame > animFrames.Length - 1){
					_myRender.enabled = false;
				}else{
					_myRender.sprite = animFrames[currentFrame];
				}
			}
		}

	}

	public void TriggerStaminaEffect(){
		currentFrame = 0;
		animCount = animRate;
		_myRender.sprite = animFrames[currentFrame];
		staminaColor = playerRef.EquippedWeapon().flashSubColor;
		setColor = staminaColor;
		setColor.a = startAlpha;
		_myRender.color = setColor;
		_myRender.enabled = true;
	}
}
