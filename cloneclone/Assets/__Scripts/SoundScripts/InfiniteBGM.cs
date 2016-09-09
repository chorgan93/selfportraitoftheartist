using UnityEngine;
using System.Collections;

public class InfiniteBGM : MonoBehaviour {

	private AudioSource mySource;
	public float fadeInRate = 0.5f;
	public float fadeOutRate = 2f;

	private bool fadingIn = false;
	private bool fadingOut = false;
	public float maxVolume = 1f;

	private static GameObject instance;

	private AudioClip queuedTrack;

	public bool notInfinite = false; // delete after demo
	private bool destroyOnFade = false;

	// Use this for initialization
	void Awake(){

		if (!notInfinite){
		if (!instance){
			DontDestroyOnLoad(gameObject);
			instance = gameObject;
		}else{
			Destroy(gameObject);
		}
		}

	}

	void Start () {

		mySource = GetComponent<AudioSource>();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (fadingOut){
			mySource.volume -= Time.deltaTime*fadeOutRate;
			if (mySource.volume <= 0){
				if (destroyOnFade){
					Destroy(gameObject);
				}else{
					mySource.volume = 0;
					fadingOut = false;
					mySource.Stop();
					if (queuedTrack != null){
						mySource.clip = queuedTrack;
						queuedTrack = null;
						fadingIn = true;
						mySource.Play();
					}
				}
			}

		}
		if (fadingIn){
			mySource.volume += Time.deltaTime*fadeInRate;
			if (mySource.volume >= maxVolume){
				mySource.volume = maxVolume;
				fadingIn = false;
			}
		}
	
	}

	public void FadeOut(bool destroyOnOut = false){

		fadingIn = false;
		fadingOut = true;
		destroyOnFade = destroyOnOut;

	}
	public void FadeIn(){

		if (!fadingOut){
			fadingIn = true;
			fadingOut = false;
		}

	}
	public void NewTrack(AudioClip newT){

		if (!fadingOut){
		mySource.Stop();
		mySource.clip = newT;
		mySource.Play();
		}else{
			queuedTrack = newT;
		}

	}

}
