using UnityEngine;
using System.Collections;

public class PlayerHealEffect : MonoBehaviour {

	private SpriteRenderer _myRender;
	public SpriteRenderer[] subRenders;
	public Sprite[] animFrames;
	public Sprite[] subAnimFrames;
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
					SetSubFrames(currentFrame);
					_myRender.sprite = animFrames[currentFrame];
				}
			}
		}

	}

	public void TriggerStaminaEffect(){
		currentFrame = 0;
		animCount = animRate;
		_myRender.sprite = animFrames[currentFrame];
		SetSubFrames(currentFrame);
		//staminaColor = playerRef.EquippedWeapon().flashSubColor;
		setColor = staminaColor;
		setColor.a = startAlpha;
		SetSubColors(setColor);
		_myRender.color = setColor;
		_myRender.enabled = true;
	}

	void SetSubFrames(int newFrame){
		for (int i = 0; i < subRenders.Length; i++){
			if (newFrame < subAnimFrames.Length){
				subRenders[i].enabled = true;
				subRenders[i].sprite = subAnimFrames[newFrame];
			}else{
				subRenders[i].enabled = false;
			}
		}
	}

	void SetSubColors(Color newCol){
		for (int i = 0; i < subRenders.Length; i++){
			subRenders[i].color = newCol;

		}
	}

	public void TriggerChargeEffect(){
		currentFrame = 0;
		animCount = animRate;
		_myRender.sprite = animFrames[currentFrame];
		setColor = chargeColor;
		setColor.a = startAlpha;
		SetSubColors(setColor);
		_myRender.color = setColor;
		_myRender.enabled = true;
	}
}
