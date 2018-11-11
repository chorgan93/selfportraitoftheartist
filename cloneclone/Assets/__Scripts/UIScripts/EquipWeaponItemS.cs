using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipWeaponItemS : MonoBehaviour {
	
	private PlayerInventoryS inventoryRef;
	private PlayerWeaponS weaponRef;

	public Image weaponImage;
	public int weaponNum;
    public int rowIndex;

	public string weaponDescriptionMain;
	public string weaponDescriptionSub;
	private bool _unlocked = false;
	public bool unlocked { get { return _unlocked; } }

    private Vector2 startPosition;
    bool setPosition = false;
    private float finalRowXOffset = 19;
    private Vector2 offsetPosition;

	public void Initialize(PlayerInventoryS i){

		inventoryRef = i;
        if (!setPosition)
        {
            startPosition = transform.parent.GetComponent<RectTransform>().anchoredPosition;
            offsetPosition = startPosition;
            offsetPosition.x -= finalRowXOffset;
        }

		bool turnOn = false;
		foreach (PlayerWeaponS w in i.unlockedWeapons){
			if (w.weaponNum == weaponNum){
				turnOn = true;
				weaponRef = w;
			}
		}

		if (!turnOn){
			weaponImage.enabled = false;
			//weaponName.enabled = false;
			_unlocked = false;
            if (weaponNum == 12){
                // turn this item off completely if player does not have secret mantra
                transform.parent.gameObject.SetActive(false);
            }
		}else{
			weaponImage.sprite = weaponRef.swapSprite;
			weaponImage.color = weaponRef.swapColor;
			weaponImage.enabled = true;

			//weaponName.color = weaponRef.swapColor;
			//weaponName.enabled = true;
			_unlocked = true;
            if (weaponNum == 12)
            {
                // turn this item on completely if player does have secret mantra
                transform.parent.gameObject.SetActive(true);
            }
        }

        if (rowIndex > 7 && rowIndex < 12){
            if (i.CheckForWeaponNum(12)){
                transform.parent.GetComponent<RectTransform>().anchoredPosition = offsetPosition;
            }else
            {
                transform.parent.GetComponent<RectTransform>().anchoredPosition = startPosition;
            }
        }

	}

	public PlayerWeaponS WeaponRefForSwitch(){
		return weaponRef;
	}
}
