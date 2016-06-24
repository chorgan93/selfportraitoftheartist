using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BorderResizeS : MonoBehaviour {

	public Image leftBorder;
	public Image rightBorder;
	public Image statHolder;
	private Vector2 borderSizeDelta; 

	private float playerUIOffset = 5f;

	private float desiredWidth;
	private float borderWidth;

	// Use this for initialization
	void Start () {

		playerUIOffset = statHolder.rectTransform.anchoredPosition.x;
		UpdateScreen();
	
	}
	
	// Update is called once per frame
	void Update () {

		UpdateScreen();
	
	}

	private void UpdateScreen(){

		desiredWidth = Screen.height*4f/3f;

		borderWidth = (Screen.width-desiredWidth)/2f;

		if (borderWidth < 0){
			borderWidth = 0;
		}

		borderSizeDelta = Vector2.zero;
		borderSizeDelta.x = borderWidth;
		leftBorder.rectTransform.sizeDelta = rightBorder.rectTransform.sizeDelta = borderSizeDelta;

		borderSizeDelta = statHolder.rectTransform.anchoredPosition;
		//borderSizeDelta.x = borderWidth + playerUIOffset;
		borderSizeDelta.x =  playerUIOffset;
		statHolder.rectTransform.anchoredPosition = borderSizeDelta;

	}
}
