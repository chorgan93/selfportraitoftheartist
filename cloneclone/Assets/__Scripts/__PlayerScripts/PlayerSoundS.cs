using UnityEngine;
using System.Collections;

public class PlayerSoundS : MonoBehaviour {

	private bool _walking = false;
	private bool _running = false;
	private PlayerController pRef;

	[Header("Footstep Sounds")]
	public GameObject[] footsteps;
	public float footstepRate = 0.22f;
	public float runningMult = 1.6f;
	private float footstepCountdown;
	private int footstepToUse = 0;
	public GameObject footstepObj;
	private float footstepY = -0.6f;
	private float footstepX = 0.22f;
	private float xMult = 1f;

	[Header("Action Sounds")]
	public GameObject shieldSound;
	public GameObject rollSound;
	public GameObject chargeSound;

	[Header("Damange Sounds")]
	public GameObject damageSound;
	public GameObject deathSound;
	public GameObject slowSound;

	// Use this for initialization
	void Start () {

		pRef = GetComponent<PlayerController>();
	
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
					PlaceFootstep();
				}
			}
		}else{
			footstepCountdown = footstepRate;
		}

	}

	void PlaceFootstep(){

		if (footstepObj){
			Vector3 spawnPos = transform.position;
			spawnPos.y += footstepY;
			spawnPos.x += footstepX*xMult;
			GameObject newFootstep = Instantiate(footstepObj, spawnPos, Quaternion.identity)
					as GameObject;
			if (pRef.myRenderer.transform.localScale.x < 0){
				Vector3 flipSize = newFootstep.transform.localScale;
				flipSize.x *= -1f;
				newFootstep.transform.localScale = flipSize;
			}
			newFootstep.GetComponent<SpriteRenderer>().color = pRef.myRenderer.color;
			newFootstep.transform.GetChild(0).GetComponent<SpriteRenderer>().color = pRef.myRenderer.color;
			xMult *= -1f;
		}
	}

	public void PlayRollSound(){
		Instantiate(rollSound);
	}

	public void PlayShieldSound(){
		Instantiate(shieldSound);
	}

	public void PlayHurtSound(){
		Instantiate(damageSound);
	}

	public void PlayDeathSound(){
		Instantiate(deathSound);
	}

	public void PlayChargeSound(){
		Instantiate(chargeSound);

	}

	public void PlaySlowSound(){
		Instantiate(slowSound);
	}
}
