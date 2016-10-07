using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipMenuS : MonoBehaviour {

	public Image playerImage;
	private PlayerController pRef;

	public Image mantraMain;
	public Image mantraSub;

	public Image buddyMain;
	public Image buddyMainOutline;
	public Image buddySub;
	public Image buddySubOutline;

	private bool onMainScreen = true;

	private bool _canBeQuit = false;
	public bool canBeQuit { get { return _canBeQuit; } }

	void Start(){
		
		playerImage.enabled = false;
		mantraMain.enabled = false;
		mantraSub.enabled = false;
		buddyMain.enabled = false;
		buddySub.enabled = false;

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

	}
}
