using UnityEngine;
using System.Collections;

public class BloodParticleEffect : MonoBehaviour {

	private SpriteRenderer mainBlood;
	public SpriteRenderer[] bloodBits;
	public SpriteRenderer rectEffect;
	//private Rigidbody[] bloodRigids;
	//private TrailRenderer[] bloodTrails;
	public float startSpeedMin = 1200;
	public float startSpeedMax = 3600;
	private float applyGravDelay = 0.5f;
	private bool appliedGrav = false;
	public float rotateAmt;

	public Color[] possColors;
	private float changeColorTime = 0.08f;
	private float changeColorCountdown;

	Vector3 currentVel = Vector3.zero;	

	// Use this for initialization
	void Start () {

		mainBlood = GetComponent<SpriteRenderer>();

		changeColorCountdown = changeColorTime;

		// bloodRigids = new Rigidbody[bloodBits.Length];
	//	bloodTrails = new TrailRenderer[bloodBits.Length];

		Vector3 rotateAmtV = Vector3.zero;
		int currentIndex = 0;
		int dir = 1;
		foreach(SpriteRenderer b in bloodBits){
			rotateAmtV.z = Random.insideUnitCircle.x * rotateAmt;
			b.transform.Rotate(rotateAmtV);
			currentVel = b.transform.up*(Random.Range(startSpeedMin, startSpeedMax))*dir;
			if (Mathf.Abs(rotateAmtV.z) < rotateAmt/2f){
				currentVel*=0.25f;
			}else{
				currentVel*=1.5f;
				Vector3 newScale = b.transform.localScale;
				newScale.x *= 1.15f;
				newScale.y *= 1.5f;
				b.transform.localScale=newScale;
			}
			b.transform.position += currentVel;
			//bloodTrails[currentIndex] = b.GetComponent<TrailRenderer>();
		//	bloodRigids[currentIndex] = b.GetComponent<Rigidbody>();
		//	bloodRigids[currentIndex].AddForce(currentVel, ForceMode.Impulse);
			currentIndex++;
			dir++;
			if (dir > 2){
				dir = -1;
			}
			if (dir == 0){
				dir = 1;
			}
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		/*if (!appliedGrav){
		applyGravDelay -= Time.deltaTime;
			if (applyGravDelay <= 0){
				foreach(Rigidbody r in bloodRigids){
					r.useGravity = true;
				}
				appliedGrav = true;
			}
		}**/

		changeColorCountdown -= Time.deltaTime;
		if (changeColorCountdown <= 0){
			changeColorCountdown = changeColorTime;
			Color newCol = possColors[Mathf.RoundToInt(Random.Range(0, possColors.Length-1))];
			newCol.a = mainBlood.color.a;
			mainBlood.color = newCol;
			int currentIndex = 0;
			foreach (SpriteRenderer b in bloodBits){
				newCol = possColors[Mathf.RoundToInt(Random.Range(0, possColors.Length-1))];
				newCol.a = b.color.a;
				b.color = newCol;
				//bloodTrails[currentIndex].material.color = newCol;
				currentIndex++;
			}
			if (rectEffect){
				rectEffect.color = newCol;
			}
		}

	
	}
}
