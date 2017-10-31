using UnityEngine;
using System.Collections;

public class SacramentStepS : MonoBehaviour {

	private SacramentHandlerS _myHandler;
	public SacramentHandlerS myHandler { get { return _myHandler; } }
	private bool _initialized = false;

	private bool _stepActive = false;
	public bool stepActive { get { return _stepActive; } }

	[Header("Item Properties")]
	public SacramentTextS[] sacramentTexts;
	public SacramentOptionS[] sacramentOptions;
	public SacramentImageS[] sacramentImages;

	[Header("Sound Properties")]
	public GameObject onSound;
	public GameObject offSound;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	


	}

	public void ActivateStep(){
		_stepActive = true;
		SetUpOptions();
		gameObject.SetActive(true);
		if (onSound){
			Instantiate(onSound);
		}
	}

	public void DeactivateStep(){
		_stepActive = false;
		gameObject.SetActive(false);
		if (offSound){
			Instantiate(offSound);
		}
	}

	public void Initialize(SacramentHandlerS handler){
		if (!_initialized){
			_myHandler = handler;
			_initialized = true;
		}
	}

	void SetUpTexts(){
		
	}

	void SetUpOptions(){
		if (sacramentOptions.Length > 0){
			for (int i = 0; i < sacramentOptions.Length; i++){
				sacramentOptions[i].Initialize(_myHandler);
			}
		}
	}

	void SetUpImages(){
		
	}
}
