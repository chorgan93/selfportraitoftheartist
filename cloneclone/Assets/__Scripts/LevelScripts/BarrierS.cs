using UnityEngine;
using System.Collections;

public class BarrierS : MonoBehaviour {

	public static int activeBarriers = 0;

	public int barrierNum = -1;

	private float fixTurnOffTime = 1.4f;

	public Collider barrierCollider;
	private Renderer _myRenderer;
	public Texture flashTexture;
	private Texture startTexture;

	bool firstBarrier = false;

	public float delayTurnOffTime = 0.4f;
	public float turnOffTime = 0.9f;
	public float extraCameraTime = 0f;
	private float fadeRate = 0.8f;
	private Color myColor;

	bool turningOff = false;
	private int flashFrames = 6;
	public GameObject turnOffSound;

	private ActivateOnBarrierOffS activateObj;
	public GameObject overrideResetPOI;

	public int turnOffAtProgression = -1;

	// Use this for initialization
	void Start () {

		CheckClear();

		_myRenderer = GetComponent<Renderer>();
		myColor = _myRenderer.material.color;
		startTexture = _myRenderer.material.GetTexture("_MainTex");

		activateObj = GetComponent<ActivateOnBarrierOffS>();
	
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

							/*if (firstBarrier){
								CameraShakeS.C.SmallSleep();
							}
							CameraShakeS.C.SmallShake();**/
							if (turnOffSound){
								Instantiate(turnOffSound);
							}
						}
						myColor.a -= fadeRate*Time.deltaTime;
						if (myColor.a <= 0){
							_myRenderer.enabled = false;
							barrierCollider.enabled = false;
							activeBarriers--;
							if (activateObj){
								activateObj.OnOff();
							}
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

		if (turnOffAtProgression > -1){
			if (StoryProgressionS.storyProgress.Contains(turnOffAtProgression)){
				gameObject.SetActive(false);
				barrierCollider.enabled = false;
			}
		}

	}

	public void TurnOff(){

		if (gameObject.activeSelf){

			activeBarriers++;
			if (activeBarriers == 1){
				firstBarrier = true;
			}

			CameraFollowS.F.AddToQueue(gameObject, delayTurnOffTime+turnOffTime*fixTurnOffTime+extraCameraTime);
			if (overrideResetPOI != null){
				CameraFollowS.F.SetOverrideResetPOI(overrideResetPOI);
			}

			delayTurnOffTime = delayTurnOffTime*activeBarriers*1f + turnOffTime*fixTurnOffTime*((activeBarriers*1f)-1f);
	
			turningOff = true;

			if (barrierNum >= 0){
				PlayerInventoryS.I.AddClearedWall(barrierNum);
			}

		}


	}
}
