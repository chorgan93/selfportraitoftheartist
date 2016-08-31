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

	public Transform transformRef;
	private Vector3 followPos;


	// Update is called once per frame
	void Update () {

		followPos = transformRef.position;
		followPos.z -= 1f;
		transform.position = followPos;

		if (startSpeed > 0){


			foreach (Renderer piece in pieces){
				piece.transform.position += piece.transform.right*startSpeed*Time.deltaTime;
				if (!colorAssigned && flashFrames <= 0){
					Color newPieceCol = pieceColor;
					newPieceCol.a = 0.6f;
				piece.material.color = newPieceCol;
					//piece.material.SetTexture("_MainTex", nonFlashTexture);
				}
			}
			
			flashFrames--;
			if (flashFrames < 0){
				colorAssigned = true;
			}
			startSpeed += speedAccel*Time.deltaTime;

		}else if (flickerDelay > 0){
			flickerDelay -= Time.deltaTime;
		}
		else{
			flickerCountdown -= Time.deltaTime;
			if (flickerCountdown <= 0){
				flickerCountdown = flickerTime;
				numFlickers ++;
				if (numFlickers > flickerAmt){
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
