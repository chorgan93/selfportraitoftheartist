using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class ControlManagerS : MonoBehaviour {

	// TODO distinguish mouse/keyboard and controller types

	private float triggerSensitivity = 0.5f;

	private string platformType;

	public TouchPad touchInput;

	// Use this for initialization
	void Start () {

		platformType = GetPlatform();
	
	}
	
	// Update is called once per frame
	void Update () {

	
	}

	string GetPlatform(){

		// assume pc, check for mac/linux
		string platform = "PC";

		if (Application.platform == RuntimePlatform.OSXEditor ||
		    Application.platform == RuntimePlatform.OSXPlayer){

			platform = "Mac";

		}
		else if (Application.platform == RuntimePlatform.LinuxPlayer){
			
			platform = "Linux";
			
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android){

			platform = "Mobile";

		}

		return platform;

	}

	//_________________________________________CONTROLLER CHECK

	public bool ControllerAttached(){

		if (GetPlatform() == "Mobile"){
			return false;
		}
		else if (Input.GetJoystickNames().Length > 0){
			return true;
		}
		else{
			return false;
		}

	}


	//_________________________________________PUBLIC CONTROL CHECKS

	public float Horizontal(){

		if (GetPlatform() == "Mobile"){
			float moveHorizontal = CrossPlatformInputManager.GetAxis("Horizontal");
			
			return moveHorizontal;
		}
		else{
			if (ControllerAttached()){
				return Input.GetAxis("HorizontalController");
			}
			else{
				return Input.GetAxis("Horizontal");
			}
		}

	}

	public float Vertical(){

		if (GetPlatform() == "Mobile"){
			float moveVertical = CrossPlatformInputManager.GetAxis("Vertical");
			
			return moveVertical;
		}
		else{
			if (ControllerAttached()){
				return Input.GetAxis("VerticalController");
			}
			else{
				return Input.GetAxis("Vertical");
			}
		}

	}

	public float RightHorizontal(){
		
		return Input.GetAxis("RightHorizontalController" + platformType);
		
	}
	
	public float RightVertical(){
		
		return Input.GetAxis("RightVerticalController" + platformType);
		
	}

	public bool BlockTrigger(){

		if (ControllerAttached()){
			return (Input.GetAxis("DashTrigger"+platformType) > triggerSensitivity);
		}
		else{
			return (Input.GetMouseButton(1));
		}

	}

	public bool AimTrigger(){
		if (ControllerAttached()){
			return (Input.GetAxis("DashTrigger"+platformType) > triggerSensitivity);
		}
		else{
			return (Input.GetKey(KeyCode.LeftShift));
		}
	}

	public bool BlockButton(){

		if (ControllerAttached()){
			return (Input.GetButton("DashButton"+platformType));
		}
		else{
			return (Input.GetMouseButton(1));
		}

	}

	public bool DashKey(){

		return (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
		

	}

	public bool ShootTrigger(){

		if (ControllerAttached()){

			return (Input.GetAxis("ShootTrigger"+platformType) > triggerSensitivity);

		}
		else{
			
			return (Input.GetMouseButton(0));

		}
		
	}
	
	public bool ShootButton(){

		if (ControllerAttached()){
		
			return (Input.GetButton("ShootButton"+platformType));

		}
		else{
			
			return (Input.GetMouseButton(0));

		}
		
	}

	public bool SwitchButton(){

		if (ControllerAttached()){
			return (Input.GetButton("SwitchButton"+platformType+"AltAlt")
			        || Input.GetButton("SwitchButton"+platformType+"Alt")
			        || Input.GetButton("SwitchButton"+platformType));
		}
		else{
			return(Input.GetMouseButton(1));
		}
		
	}

	public bool ReloadButton(){

		if (ControllerAttached()){
			return (Input.GetButton("ReloadButton"+platformType));
		}
		else{
			return(Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.A));
		}

	}
}
