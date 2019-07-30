using UnityEngine;
using System.Collections;

public class BGMLayerS : MonoBehaviour
{

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
    public AudioClip loopAudio;
    private AudioClip _mainAudio;
    public AudioClip mainAudio { get { return _mainAudio; } }
    private bool playedIntro = false;
    private bool doIntroCheck = false;

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

	private bool adjustingUp = false;
	private bool adjustingDown = false;

	public bool debugLayer = false;

	// Use this for initialization
	void Awake () {

		mySource = GetComponent<AudioSource>();
		mySource.volume = startVolume;
		startPitch = mySource.pitch;
		witchTimePitch = startPitch*witchTimePitchMult;
        _mainAudio = mySource.clip;

        checkForceRestart = (forceRestartOnLoop != null);

        if (loopAudio != null){
            mySource.clip = loopAudio;
            playedIntro = false;
            mySource.loop = false;
            doIntroCheck = true;
        }

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
			if (debugLayer){
				Debug.Log("Fading in! " + mySource.volume);
			}
			if (mySource.volume >= maxVolume*BGMHolderS.volumeMult){
				mySource.volume = maxVolume*BGMHolderS.volumeMult;
				fadingIn = false;
				if (debugLayer){
					Debug.Log("reached max volume! " + maxVolume*BGMHolderS.volumeMult);
				}
			}
		}

        if (doIntroCheck){
            if (mySource.timeSamples >= loopAudio.samples){
                // intro is done, switch to main loop
                mySource.Stop();
                mySource.clip = _mainAudio;
                doIntroCheck = false;
                playedIntro = true;
                mySource.loop = true;
                mySource.Play();
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

		if (adjustingUp){
			mySource.volume += fadeInRate*Time.unscaledDeltaTime;
			if (mySource.volume >= maxVolume*BGMHolderS.volumeMult){
				mySource.volume = maxVolume*BGMHolderS.volumeMult;
				adjustingUp = false;
			}
		}
		if (adjustingDown){
			mySource.volume -= fadeOutRate*Time.unscaledDeltaTime;
			if (mySource.volume <= maxVolume*BGMHolderS.volumeMult){
				mySource.volume = maxVolume*BGMHolderS.volumeMult;
				adjustingDown = false;
			}
		}

		if (checkForceRestart){
			if (prevData > mySource.timeSamples){
				BGMHolderS.BG.ForceReset(forceRestartOnLoop, mySource.timeSamples);
			}
			prevData = mySource.timeSamples;
		}
	
	}

	public void FadeIn(bool instant = false, float maxV = -1f){

		if (!PlayerStatDisplayS.RECORD_MODE && !NO_MUSIC){
		if (!mySource.isPlaying){
			mySource.Play();
			}
		

		if (!instant){
					if (debugLayer){
						Debug.Log(gameObject.name + " is being told to fade in!");
					}
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
		if (maxV > 0){
				ChangeMaxVolume(maxV);
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

	public void ChangeMaxVolume(float newVolume){
		adjustingUp = false;
		adjustingDown = false;
		maxVolume = newVolume;
		if (mySource.volume > maxVolume*BGMHolderS.volumeMult && !fadingOut){
			adjustingDown = true;
		}
		if (mySource.volume < maxVolume*BGMHolderS.volumeMult && !fadingIn){
			adjustingUp = true;
		}
	}

}
