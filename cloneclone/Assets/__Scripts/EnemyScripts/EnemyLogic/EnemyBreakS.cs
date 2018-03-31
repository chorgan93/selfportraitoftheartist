using UnityEngine;
using System.Collections;

public class EnemyBreakS : MonoBehaviour {


	public float startSpeed;
	public float speedAccel;

	public float flickerDelay;
	private float flickerTime = 0.06f;
	private float flickerCountdown;

	public int flickerAmt;
	private int numFlickers;

	public Renderer[] pieces;
	public Color pieceColor;
	private bool colorAssigned = false;
	private int flashFrames = 4;
	public Texture nonFlashTexture;

	public EnemyBreakLettersS[] breakLetters;
	public EnemyBreakLettersS[] subLetters;
	public EnemyBreakLettersS[] dropShadows;
	private string breakString = "BREAK";
	private string parryString = "PARRY";

	public Transform transformRef;
	private Vector3 followPos;

	private float activateLetterTime = 0.04f;
	private float subLetterTime = 0.02f;

	public BuddySwitchEffectS breakBodyEffect;
	private bool activated = false;

	public void Activate(bool fromParry = false){

		breakBodyEffect.ChangeEffect(Color.red, transformRef);
		if (fromParry){
			breakString = parryString;
		}
		StartCoroutine(ActivateLetters());
		activated = true;

	}

	IEnumerator ActivateLetters(){
		for (int i = 0; i < breakLetters.Length; i ++){
			breakLetters[i].Activate(breakString[i].ToString());
			dropShadows[i].Activate(breakString[i].ToString());
			yield return new WaitForSeconds(subLetterTime);
			subLetters[i].Activate(breakString[i].ToString());
			yield return new WaitForSeconds(activateLetterTime);
		}
	}


	// Update is called once per frame
	void Update () {

		if (activated){
		followPos = transformRef.position;
		followPos.z -= 1f;
		transform.position = followPos;

		if (startSpeed > 0){

			foreach (Renderer piece in pieces){
				piece.transform.position += piece.transform.up*startSpeed*Time.unscaledDeltaTime;
				if (!colorAssigned && flashFrames <= 0){
					Color newPieceCol = pieceColor;
					newPieceCol.a = 0.6f;
					piece.material.color = newPieceCol;
				}
			}
			
			flashFrames--;
			if (flashFrames < 0){
				colorAssigned = true;
			}

			startSpeed += speedAccel*Time.unscaledDeltaTime;

		}else if (flickerDelay > 0){
			flickerDelay -= Time.deltaTime;
			if (!colorAssigned){
				foreach (Renderer piece in pieces){

					Color newPieceCol = pieceColor;
					newPieceCol.a = 0.6f;
					piece.material.color = newPieceCol;

				}
				colorAssigned = true;
			}
		}
		else{
			flickerCountdown -= Time.deltaTime;
			if (flickerCountdown <= 0){
				flickerCountdown = flickerTime;
				numFlickers ++;
				if (numFlickers > flickerAmt){
					Destroy(breakBodyEffect.gameObject);
					Destroy(gameObject);
				}else{
					foreach (Renderer piece in pieces){
						piece.enabled = !piece.enabled;
					}
				}
			}
		}
		}
	
	}

	public void ChangeScale (float multS){
		foreach (Renderer piece in pieces){
			piece.transform.localScale*=multS;
		}
		startSpeed*=multS;
		speedAccel*=multS;
		breakBodyEffect.transform.localScale*=multS*2f;
	}
}
