using UnityEngine;
using System.Collections;

public class EnemyBreakS : MonoBehaviour {

	public float rotateMaxZ = 30f;
	public float rotateMaxY = 15f;

	public float startSpeed;
	public float speedAccel;

	public float flickerDelay;
	private float flickerTime = 0.08f;
	private float flickerCountdown;

	public int flickerAmt;
	private int numFlickers;

	public Renderer[] pieces;
	public Color pieceColor;
	private bool colorAssigned = false;
	private int flashFrames = 4;
	public Texture nonFlashTexture;

	void Start () {

		transform.Rotate ( new Vector3(0, rotateMaxY*Random.insideUnitCircle.x, rotateMaxZ*Random.insideUnitCircle.y));

		foreach(Renderer piece in pieces){
			piece.material.color = Color.white;
		}

	}

	// Update is called once per frame
	void Update () {

		if (startSpeed > 0){


			foreach (Renderer piece in pieces){
				piece.transform.position += piece.transform.right*startSpeed*Time.deltaTime;
				if (!colorAssigned && flashFrames <= 0){
				piece.material.color = pieceColor;
					piece.material.SetTexture("_MainTex", nonFlashTexture);
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
