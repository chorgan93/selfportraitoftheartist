using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class ControlManagerS : MonoBehaviour {

	// TODO distinguish mouse/keyboard and controller types

	private float triggerSensitivity = 0.1f;

	private string platformType;

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

	public float HorizontalMovement(){

		if (GetPlatform() == "Mobile"){
			float moveHorizontal = CrossPlatformInputManager.GetAxis("Horizontal");

			return moveHorizontal;
		}
		else{
			if (ControllerAttached()){
				return Input.GetAxis("HorizontalController");
			}
			else{
				return Input.GetAxis("HorizontalKeys");
			}
		}

	}

	public float VerticalMovement(){

		if (GetPlatform() == "Mobile"){
			float moveVertical = CrossPlatformInputManager.GetAxis("Vertical");

			return moveVertical;
		}
		else{
			if (ControllerAttached()){
				return Input.GetAxis("VerticalController");
			}
			else{
				return Input.GetAxis("VerticalKeys");
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

		/*if (ControllerAttached()){
			//return (Input.GetButton("SwitchBuddyButton" + platformType));
			return (Input.GetAxis("DashTrigger"+platformType) > triggerSensitivity);
		}
		else{
			return ( Input.GetKey(KeyCode.Space));
		}**/
		return false;

	}

	public bool FamiliarControl(){

		if (ControllerAttached()){
			//return (Input.GetAxis("ShootTrigger"+platformType) > triggerSensitivity);
			return (Input.GetButton("ShootButton"+platformType));
			//return (Input.GetButton("ReloadButton"+platformType));
		}else{
			//return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
			return (Input.GetKey(KeyCode.J));
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

		/*if (ControllerAttached()){
			//return (Input.GetButton("SwitchBuddyButton" + platformType));
			return (Input.GetAxis("DashTrigger" + platformType) > triggerSensitivity);
		}
		else{
			return ( Input.GetKey(KeyCode.Space));
		}**/

		return false;

	}

	public bool DashKey(){

		if (ControllerAttached()){
			//return (Input.GetAxis("DashTrigger" + platformType) > triggerSensitivity);
			return (Input.GetButton("SwitchBuddyButton" + platformType));
		}
		else{
			return ( Input.GetKey(KeyCode.Space));
		}
		

	}

	public bool ShootTrigger(){

		if (ControllerAttached()){
			
			return (Input.GetAxis("ShootTrigger"+platformType) > triggerSensitivity);

		}
		else{
			
			return (Input.GetMouseButton(0));

		}
		
	}
	public bool DashTrigger(){
		
		if (ControllerAttached()){
			
			//return (Input.GetAxis("DashTrigger"+platformType) > triggerSensitivity);
			return (Input.GetAxis("ShootTrigger"+platformType) > triggerSensitivity);
			//return (Input.GetButton("DashButton"+platformType));
			
		}
		else{
			
			return (Input.GetKey(KeyCode.Space));
			
		}
		
	}
	
	public bool ShootButton(){

		if (ControllerAttached()){
			
			return (Input.GetButton("SwitchButton"+platformType+"Alt"));

		}
		else{
			
			//return (Input.GetMouseButton(0));
			return (Input.GetKey(KeyCode.K));

		}
		
	}
	public bool HeavyButton(){
		
		if (ControllerAttached()){
			
			return (Input.GetButton("ReloadButton"+platformType));
			
		}
		else{
			
			//return (Input.GetMouseButton(1));

			return (Input.GetKey(KeyCode.L));
			
		}
		
	}

	public bool WeaponButtonA(){
		if (ControllerAttached()){
			
			return (Input.GetButton("ShootButton"+platformType));
			
		}
		else{
			
			return (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1) || (Input.GetKey(KeyCode.E)));
			
		}
	}

	public bool WeaponButtonB(){
		if (ControllerAttached()){
			
			return (Input.GetButton("SwitchButton"+platformType+"Alt"));
			
		}
		else{
			
			return (Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2));
			
		}
	}

	public bool WeaponButtonC(){
		if (ControllerAttached()){
			
			return (Input.GetButton("ReloadButton"+platformType));
			
		}
		else{
			
			return (Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Keypad3) || (Input.GetKey(KeyCode.Q)));
			
		}
	}


	public bool TalkButton(){
		if (ControllerAttached()){
			
			return (Input.GetButton("DashButton"+platformType));
			
		}
		else{

			return (Input.GetKey(KeyCode.E));
			
		}
	}

	public bool SwitchButton(){



		if (ControllerAttached()){
			return (Input.GetAxis("DashTrigger" + platformType) > triggerSensitivity);
			//return (Input.GetButton("SwitchButton"+platformType));
		}
		else{
			//return(Input.GetKey(KeyCode.Q));
			return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
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

	public bool LockOnButton(){
		if (ControllerAttached()){
			return (Input.GetButton("SwitchButton"+platformType));
			//return (Input.GetAxis("DashTrigger"+platformType) > triggerSensitivity);
		}else{
			//Debug.Log("Pressing Lock on!");
			return (Input.GetKey(KeyCode.X));
		}
	}

	public bool ChangeLockTargetKeyRight(){
		if (!ControllerAttached()){
			return (Input.GetKey(KeyCode.C));
		}else{
			return 
				false;
		}
	}

	public bool ChangeLockTargetKeyLeft(){
		if (!ControllerAttached()){
			return (Input.GetKey(KeyCode.Z));
		}else{
			return 
				false;
		}
	}

	public bool StartButton(){
		if (ControllerAttached()){
			return (Input.GetButton("StartButton"+platformType));
		}else{
			return (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return));
		}
	}
	public bool BackButton(){
		if (ControllerAttached()){
			return (Input.GetButton("BackButton"+platformType));
		}else{
			return (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Q));
		}
	}

	public bool MenuSelectButton(){
		if (ControllerAttached()){
			return (WeaponButtonA() || TalkButton());
		}else{
			return (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E));
		}
	}

	public bool MenuSelectUp(){
		if (ControllerAttached()){
			return (!WeaponButtonA() && !TalkButton());
		}else{
			return (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.E));
		}
	}

	public bool ExitButton(){
		if (ControllerAttached()){
			return (WeaponButtonB() || WeaponButtonC());
		}else{
			return (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Delete));
		}
	}



	public bool ExitButtonUp(){
		if (ControllerAttached()){
			return (!WeaponButtonB() && !WeaponButtonC());
		}else{
			return (!Input.GetKey(KeyCode.Escape) && !Input.GetKey(KeyCode.Backspace) && !Input.GetKey(KeyCode.Delete));
		}
	}

	public bool UseItemButton(){

		if (ControllerAttached()){
		
			
			return (Input.GetButton("SwitchBuddyButton"+platformType));
		}else{
			return (Input.GetKey(KeyCode.R));
		}
	}
	public bool ScrollItemLeftButton(){
		//TODO add functionality for keyboard/mouse and linux
		if (ControllerAttached()){
			if (platformType == "Mac"){
				return (Input.GetButton("SwitchItemButtonLeftMac"));
			}else{
				return (Input.GetAxis("SwitchItemAxisPC") < -0.1f);
			}
		}else{
			return false;
		}
	}
	public bool ScrollItemRightButton(){
		//TODO add functionality for keyboard/mouse and linux
		if (ControllerAttached()){
			if (platformType == "Mac"){
				return (Input.GetButton("SwitchItemButtonRightMac"));
			}else{
				return (Input.GetAxis("SwitchItemAxisPC") > 0.1f);
			}
		}else{
			return false;
		}
	}
}
