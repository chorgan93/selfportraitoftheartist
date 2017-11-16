using UnityEngine;
using System.Collections;

public class BGMLayerS : MonoBehaviour {

	private const bool NO_MUSIC = false;

	public bool matchTimeStamp = true;
	public float startVolume;
	public float maxVolume;

	public float fadeInRate;
	public float fadeOutRate;

	private bool fadingIn = false;
	private bool fadingOut = false;

	private AudioSource mySource;
	public AudioSource sourceRef { get { return mySource; } }

	public AudioClip forceRestartOnLoop;
	private bool checkForceRestart;

	private bool destroyOnFade = false;
	private int prevData = 0;

	private float startPitch;
	private float witchTimePitch;
	private float witchTimePitchMult = 0.33f;
	private bool witchingIn = false;
	private bool witchingOut = false;
	private float witchInTime = 0.8f;
	private float witchOutTime = 0.6f;
	private float currentWitchCount;
	private float witchT;

	// Use this for initialization
	void Awake () {

		mySource = GetComponent<AudioSource>();
		mySource.volume = startVolume;
		startPitch = mySource.pitch;
		witchTimePitch = startPitch*witchTimePitchMult;

		checkForceRestart = (forceRestartOnLoop != null);

		// for recording only, delete after
		//Debug.Log("ATTN Colin delete the folowing lines:");
		//mySource.volume = 0f;
		//startVolume = maxVolume = 0f;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (witchingIn){
			currentWitchCount += Time.deltaTime;
			if (currentWitchCount >= witchInTime){
				currentWitchCount = witchInTime;
				witchingIn = false;
			}
			witchT = currentWitchCount/witchInTime;
			witchT = Mathf.Sin(witchT * Mathf.PI * 0.5f);
			mySource.pitch = Mathf.Lerp(startPitch, witchTimePitch, witchT);
		}
		if (witchingOut){
			currentWitchCount += Time.deltaTime;
			if (currentWitchCount >= witchOutTime){
				currentWitchCount = witchOutTime;
				witchingOut = false;
			}
			witchT = currentWitchCount/witchOutTime;
			witchT = Mathf.Sin(witchT * Mathf.PI * 0.5f);
			mySource.pitch = Mathf.Lerp(witchTimePitch, startPitch, witchT);
		}

		if (fadingIn){
			mySource.volume += fadeInRate*Time.unscaledDeltaTime;
			if (mySource.volume >= maxVolume*BGMHolderS.volumeMult){
				mySource.volume = maxVolume*BGMHolderS.volumeMult;
				fadingIn = false;
			}
		}

		if (fadingOut){
			mySource.volume -= fadeOutRate*Time.unscaledDeltaTime;
			if (mySource.volume <= 0f){
				mySource.volume = 0f;
				fadingOut = false;
				if (destroyOnFade){
					Destroy(gameObject);
				}
			}
		}

		if (checkForceRestart){
			if (prevData > mySource.timeSamples){
				BGMHolderS.BG.ForceReset(forceRestartOnLoop, mySource.timeSamples);
			}
			prevData = mySource.timeSamples;
		}
	
	}

	public void FadeIn(bool instant = false){

		if (!PlayerStatDisplayS.RECORD_MODE && !NO_MUSIC){
		if (!mySource.isPlaying){
			mySource.Play();
		}

		if (!instant){
			fadingIn = true;
			fadingOut = false;
		}else{
			mySource.Stop();
			mySource.Play();
			fadingIn = false;
			fadingOut = false;
			mySource.volume = maxVolume*BGMHolderS.volumeMult;
		}
		}

		destroyOnFade = false;
	}

	public void FadeOut(bool instant, bool dOnFade = false){
		destroyOnFade = dOnFade;
		if (!instant){
			fadingOut = true;
			fadingIn = false;
		}else{
			if (destroyOnFade){
				Destroy(gameObject);
			}else{
				fadingOut = false;
				fadingIn = false;
				mySource.volume = 0f;
			}
		}
	}

	public void StopLayer(){
		mySource.Stop();
	}

	public void StartWitch(){
		//mySource.pitch = witchTimePitch;
		if (!witchingIn && !witchingOut){
			currentWitchCount = 0f;
		}
		witchingIn = true;
		witchingOut = false;
	}
	public void EndWitch(){
		//mySource.pitch = startPitch;
		if (!witchingIn && !witchingOut){
			currentWitchCount = 0f;
		}
		witchingIn = false;
		witchingOut = true;
	}

	public bool isPlayingAndHeard(){
		bool iP = false;
		if (mySource.volume > 0 && mySource.isPlaying && gameObject.activeSelf){
			iP = true;
		}
		return iP;
	}

	public void UpdateBasedOnSetting(int dir){
		if (mySource.isPlaying){
			if (dir < 0 && !fadingOut){
				mySource.volume = maxVolume*BGMHolderS.volumeMult;
			}
			if (dir > 0 && !fadingIn){
	
					fadingIn = true;
			}
		}
	}

}
