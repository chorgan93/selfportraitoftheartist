using UnityEngine;
using System.Collections;

public class SFXObjS : MonoBehaviour {

	private AudioSource mySource;
	public float pitchMult = 0.1f;

	// Use this for initialization
	void Start () {

		mySource = GetComponent<AudioSource>();
		mySource.pitch += Random.insideUnitCircle.x*pitchMult;
		mySource.Play();
	
	}

	void LateUpdate () {

		if (!mySource.isPlaying){
			Destroy(gameObject);
		}
	
	}
}
