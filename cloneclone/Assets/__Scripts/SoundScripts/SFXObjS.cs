using UnityEngine;
using System.Collections;

public class SFXObjS : MonoBehaviour {

	private AudioSource mySource;
	public float pitchMult = 0.1f;

	public static float volumeSetting = 1f;
	private const float volumeSettingChangeAmt = 0.25f;

	// Use this for initialization
	void Start () {

		mySource = GetComponent<AudioSource>();
		mySource.volume *= volumeSetting;
		mySource.pitch += Random.insideUnitCircle.x*pitchMult;
		mySource.Play();

		if (transform.parent){
			transform.parent = null;
		}
	
	}

	void LateUpdate () {

		if (!mySource.isPlaying){
			Destroy(gameObject);
		}
	
	}

	public static void SetVolumeSetting(int dir){
		if (dir>0){
			if (volumeSetting < 1f){
				volumeSetting += volumeSettingChangeAmt;
			}
		}else{
			if (volumeSetting > 0f){
				volumeSetting -= volumeSettingChangeAmt;
			}
		}
	}
}
