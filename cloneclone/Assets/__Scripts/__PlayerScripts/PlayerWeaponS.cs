using UnityEngine;
using System.Collections;

public class PlayerWeaponS : MonoBehaviour {

	public int weaponNum = 0;
	public float speedMult = 1f;
	public GameObject[] attackChain;
	public GameObject[] heavyChain;
	public GameObject[] chargeChain;
	public GameObject dashAttack;
	public GameObject switchSoundObj;

	public Sprite swapSprite;
	public Color swapColor;
	
}
