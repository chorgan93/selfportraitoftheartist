using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipMenuS : MonoBehaviour {

	public Image playerImage;
	private PlayerController pRef;

	private bool controlStickMoved = false;

	public Image mantraMain;
	public Image mantraSub;

	public Image buddyMain;
	public Image buddyMainOutline;
	public Image buddySub;
	public Image buddySubOutline;

	public RectTransform selector;
	public RectTransform[] selectorPositions;
	public Image[] selectorElements;
	private int currentPos = 0;

	private float startElementAlpha;

	private bool onMainScreen = true;

	private bool _canBeQuit = false;
	public bool canBeQuit { get { return _canBeQuit; } }

	void Start(){
		
		playerImage.enabled = false;
		mantraMain.enabled = false;
		mantraSub.enabled = false;
		buddyMain.enabled = false;
		buddySub.enabled = false;
		
		startElementAlpha = selectorElements[0].color.a;
		SetSelector(currentPos);

	}

	// Update is called once per frame
	void Update () {

		if (!pRef){
			pRef = GetComponentInParent<InGameMenuManagerS>().pRef;
			
			
			playerImage.color = pRef.myRenderer.color;
			playerImage.sprite = pRef.myRenderer.sprite;
			playerImage.enabled = true;
			
			mantraMain.color = pRef.EquippedWeapon().swapColor;
			mantraMain.sprite = pRef.EquippedWeapon().swapSprite;
			mantraMain.enabled = true;

			mantraSub.color = pRef.SubWeapon().swapColor;
			mantraSub.sprite = pRef.SubWeapon().swapSprite;
			mantraSub.enabled = true;
			
			buddyMain.color = buddyMainOutline.color = pRef.EquippedBuddy().shadowColor;
			buddyMain.sprite = pRef.EquippedBuddy().buddyMenuSprite;
			buddyMain.enabled = true;

			buddySub.color = buddySubOutline.color = pRef.SubBuddy().shadowColor;
			buddySub.sprite = pRef.SubBuddy().buddyMenuSprite;
			buddySub.enabled = true;
		}

		if (onMainScreen){

			playerImage.color = pRef.myRenderer.color;
			playerImage.sprite = pRef.myRenderer.sprite;
		
			_canBeQuit = true;

		}

		if (!controlStickMoved){
			if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
				controlStickMoved = true;
				int targetPos = currentPos+1;
				if (targetPos > selectorPositions.Length-1){
					targetPos = 0;
				}
				SetSelector(targetPos);
				controlStickMoved = true;
			}
			if (pRef.myControl.Horizontal() <= -0.1f || pRef.myControl.Vertical() >= 0.1f){
				controlStickMoved = true;
				int targetPos = currentPos-1;
				if (targetPos < 0){
					targetPos = selectorPositions.Length-1;
				}
				SetSelector(targetPos);
				controlStickMoved = true;
			}
		}

		if (Mathf.Abs(pRef.myControl.Horizontal()) < 0.1f && Mathf.Abs(pRef.myControl.Vertical()) < 0.1f){
			controlStickMoved = false;
		}

	}

	public void SetSelector(int newPos){

		Color changeCols = selectorElements[currentPos].color;
		changeCols.a = startElementAlpha;
		selectorElements[currentPos].color = changeCols;

		changeCols.a = 1f;
		selectorElements[newPos].color = changeCols;
		
		currentPos = newPos;
		selector.anchoredPosition = selectorPositions[currentPos].anchoredPosition;
	}
}
