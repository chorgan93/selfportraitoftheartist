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

	private bool destroyOnFade = false;

	// Use this for initialization
	void Awake () {

		mySource = GetComponent<AudioSource>();
		mySource.volume = startVolume;

		// for recording only, delete after
		//Debug.Log("ATTN Colin delete the folowing lines:");
		//mySource.volume = 0f;
		//startVolume = maxVolume = 0f;
	
	}
	
	// Update is called once per frame
	void Update () {

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
