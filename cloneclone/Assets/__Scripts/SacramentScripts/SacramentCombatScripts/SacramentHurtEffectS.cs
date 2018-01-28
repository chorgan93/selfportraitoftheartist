using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SacramentHurtEffectS : MonoBehaviour {

	public Color enemyHurt;
	public Color playerHurt;
	private int numFlashes;
	private int currentFlash;
	public float flashTimeOn;
	public float flashTimeOff;
	private Image myImage;
	public GameObject[] enemySoundObj;
	public GameObject[] playerSoundObj;

	// Use this for initialization
	void Start () {

		myImage = GetComponent<Image>();
		myImage.enabled = false;
	
	}

	public void StartFlashing(bool isPlayer, int nFlash) {
		
		if (isPlayer){
			myImage.color = playerHurt;
			Instantiate(playerSoundObj[Mathf.FloorToInt(Random.Range(0, playerSoundObj.Length))]);
		}else{
			myImage.color = enemyHurt;
			Instantiate(enemySoundObj[Mathf.FloorToInt(Random.Range(0, enemySoundObj.Length))]);
		}
		numFlashes = nFlash;
		currentFlash = 0;
		StartCoroutine(Flash());
	}

	IEnumerator Flash(){
		while (currentFlash < numFlashes){
			myImage.enabled = true;
			yield return new WaitForSeconds(flashTimeOn);
			myImage.enabled = false;
			yield return new WaitForSeconds(flashTimeOff);
			currentFlash++;
			yield return null;
		}
	}
}
