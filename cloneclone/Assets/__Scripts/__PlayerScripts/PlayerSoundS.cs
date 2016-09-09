using UnityEngine;
using System.Collections;

public class PlayerSoundS : MonoBehaviour {

	private bool _walking = false;
	private bool _running = false;

	[Header("Footstep Sounds")]
	public GameObject[] footsteps;
	public float footstepRate = 0.22f;
	public float runningMult = 1.6f;
	private float footstepCountdown;
	private int footstepToUse = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		Footsteps();
	
	}

	public void SetWalking(bool newW){
		_walking = newW;
		if (_walking == false){
			_running = false;
		}
	}
	public void SetRunning(bool newR){
		_running = newR;
	}

	private void Footsteps(){



		if (_walking){
			if (_running){
				footstepCountdown -= Time.deltaTime*runningMult;
			}else{
				footstepCountdown -= Time.deltaTime;
			}
			if (footstepCountdown <= 0){
				footstepCountdown = footstepRate;
				footstepToUse = Mathf.RoundToInt(Random.Range(0, footsteps.Length-1));
				if (footstepToUse < footsteps.Length){
					Instantiate(footsteps[footstepToUse]);
				}
			}
		}else{
			footstepCountdown = footstepRate;
		}

	}
}
