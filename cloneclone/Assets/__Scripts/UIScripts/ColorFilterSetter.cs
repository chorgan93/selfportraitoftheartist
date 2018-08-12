using UnityEngine;
using System.Collections;

public class ColorFilterSetter : MonoBehaviour {

	public Color filterColor = Color.black;
	public Color raysColor = new Color(0,1,1);
	public Color filterColorNoEffects = Color.white;

    private GameObject filterObject;

	// Use this for initialization
	void Awake () {
	
		if (filterColorNoEffects == Color.white){
			filterColorNoEffects = filterColor;
		}

		filterObject = GameObject.Find("ColorFilter");

		if (filterObject != null){
			#if UNITY_EDITOR_OSX
			if (!CameraEffectsS.cameraEffectsEnabled){

				filterObject.GetComponent<SpriteRenderer>().color = filterColorNoEffects;
			}else{

				filterObject.GetComponent<SpriteRenderer>().color = filterColor;
			}

			#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE
			if (QualitySettings.GetQualityLevel() < 1 && !CameraEffectsS.E.arcadeMode){

			filterObject.GetComponent<SpriteRenderer>().color = filterColorNoEffects;
			}else{

			filterObject.GetComponent<SpriteRenderer>().color = filterColor;
			}
			#endif
		}


	}

	void Start(){
		if (CameraEffectsS.E != null){
			
			CameraEffectsS.E.SetRaysColor(raysColor);
		}
	}

    public void RefreshFilter(){
        if (filterObject != null)
        {
            if (!CameraEffectsS.cameraEffectsEnabled)
            {

                filterObject.GetComponent<SpriteRenderer>().color = filterColorNoEffects;
            }
            else
            {

                filterObject.GetComponent<SpriteRenderer>().color = filterColor;
            }
        }
    }

	#if UNITY_EDITOR_OSX
	void Update(){

		if(Input.GetKeyUp(KeyCode.P)){
			GameObject filterObject = GameObject.Find("ColorFilter");
			if (!CameraEffectsS.cameraEffectsEnabled && filterObject != null){

				filterObject.GetComponent<SpriteRenderer>().color = filterColorNoEffects;
			}else{

				filterObject.GetComponent<SpriteRenderer>().color = filterColor;
			}
		}
	}
	#endif
}
