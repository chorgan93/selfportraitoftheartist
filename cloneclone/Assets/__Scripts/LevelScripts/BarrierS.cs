using UnityEngine;
using System.Collections;

public class BarrierS : MonoBehaviour {

	public static int activeBarriers = 0;

	public int barrierNum = -1;

	public Collider barrierCollider;
	private Renderer _myRenderer;
	public Texture flashTexture;
	private Texture startTexture;

	public float delayTurnOffTime = 0.4f;
	public float turnOffTime = 0.9f;
	private float fadeRate = 0.8f;
	private Color myColor;

	bool turningOff = false;
	private int flashFrames = 6;

	// Use this for initialization
	void Start () {

		CheckClear();

		_myRenderer = GetComponent<Renderer>();
		myColor = _myRenderer.material.color;
		startTexture = _myRenderer.material.GetTexture("_MainTex");
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_myRenderer.enabled){
			if (turningOff){
				delayTurnOffTime -= Time.deltaTime;
				if (delayTurnOffTime <= 0){
					if (flashFrames >= 6){
					_myRenderer.material.SetTexture("_MainTex", flashTexture);
					_myRenderer.material.color = Color.white;
					}
					flashFrames--;
					if (flashFrames <= 0){
						if (_myRenderer.material.color == Color.white){
							_myRenderer.material.SetTexture("_MainTex", startTexture);
							_myRenderer.material.color = myColor;
						}
						myColor.a -= fadeRate*Time.deltaTime;
						if (myColor.a <= 0){
							_myRenderer.enabled = false;
							barrierCollider.enabled = false;
							activeBarriers--;
							}else{
							_myRenderer.material.color = myColor;
						}
					}
				}
			}
		}
	
	}

	private void CheckClear(){

		if (PlayerInventoryS.I.clearedWalls.Count > 0 && barrierNum >= 0){
			foreach(int i in PlayerInventoryS.I.clearedWalls){
				if (i == barrierNum){
					gameObject.SetActive(false);
					barrierCollider.enabled = false;
				}
			}
		}

	}

	public void TurnOff(){

		if (gameObject.activeSelf){

			activeBarriers++;

			CameraFollowS.F.AddToQueue(gameObject, delayTurnOffTime+turnOffTime);


			delayTurnOffTime = delayTurnOffTime*activeBarriers*1f + turnOffTime*((activeBarriers*1f)-1f);
	
			turningOff = true;

			if (barrierNum >= 0){
				PlayerInventoryS.I.AddClearedWall(barrierNum);
			}

		}


	}
}
