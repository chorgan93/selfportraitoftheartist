using UnityEngine;
using System.Collections;

public class WeaponSwitchFlashS : MonoBehaviour {

	private SpriteRenderer _myRenderer;
	private Color currentColor;
	public Sprite flashSprite;
	public int flashFrames = 4;
	private int flashCountdown = 0;

	public float fadeRate = 2f;
	public float growRate = 8f;
	private Vector3 startSize;

	private PlayerWeaponS currentWeapon;

	// Use this for initialization
	void Start () {

		_myRenderer = GetComponent<SpriteRenderer>();
		_myRenderer.enabled = false;
		startSize = transform.localScale;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_myRenderer.enabled){
			flashCountdown --;
			if (flashCountdown == 0){
				_myRenderer.sprite = currentWeapon.swapSprite;
				_myRenderer.color = currentWeapon.swapColor;
			}
			if (flashCountdown < 0){

				/*Vector3 currentSize = transform.localScale;
				currentSize.x += growRate*Time.deltaTime;
				currentSize.y = currentSize.x;
				transform.localScale = currentSize;*/

				currentColor = _myRenderer.color;
				currentColor.a -= Time.deltaTime*fadeRate;
				if (currentColor.a <= 0){
					currentColor.a = 0;
					_myRenderer.enabled = false;
				}
				_myRenderer.color = currentColor;
			}
		}
	
	}

	public void Flash(PlayerWeaponS newWeapon){


		currentWeapon = newWeapon;

		Instantiate(currentWeapon.switchSoundObj);

		currentColor = Color.white;
		currentColor.a = 1f;
		_myRenderer.color = currentColor;
		flashCountdown = flashFrames;
		_myRenderer.sprite = flashSprite;
		_myRenderer.enabled = true;
		transform.localScale = startSize;

	}
}
