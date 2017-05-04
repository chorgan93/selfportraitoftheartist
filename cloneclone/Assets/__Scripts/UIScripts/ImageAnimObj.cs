using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageAnimObj : MonoBehaviour {

	public Sprite[] animFrames;
	public float animRate;
	private float animCount;
	private int currentSprite;
	private Image myImage;


	// Use this for initialization
	void Start () {

		myImage = GetComponent<Image>();
		myImage.sprite = animFrames[0];
		animCount = animRate;
		currentSprite = 0;
	
	}
	
	// Update is called once per frame
	void Update () {

		animCount -= Time.deltaTime;
		if (animCount <= 0){
			currentSprite ++;
			if (currentSprite > animFrames.Length-1){
				currentSprite = 0;
			}
			myImage.sprite = animFrames[currentSprite];
			animCount = animRate;
		}
	}
}
