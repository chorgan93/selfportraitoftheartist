using UnityEngine;
using System.Collections;

public class EnemyProjectileFlashS : MonoBehaviour {

	private EnemyProjectileS myProjectileRef;

	public Color[] flashColors;
	public float flashRate = 0.083f;
	private float flashCountdown;

	private SpriteRenderer myRenderer;

	// Use this for initialization
	void Start () {
	
		myProjectileRef = GetComponentInParent<EnemyProjectileS>();
		myRenderer = GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {

		if (myProjectileRef.flashFrames <= 0){

			if (myRenderer.material.GetFloat("_FlashAmount") < 1){
				myRenderer.material.SetFloat("_FlashAmount", 1);
			}

			flashCountdown -= Time.deltaTime;
			if (flashCountdown <= 0){
				flashCountdown = flashRate;
				int colorToChoose = Mathf.RoundToInt(Random.Range(0, flashColors.Length-1));
				myRenderer.material.SetColor("_FlashColor", flashColors[colorToChoose]);
			}
		}
	
	}
}
