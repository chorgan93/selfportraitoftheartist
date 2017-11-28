using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class ControlManagerS : MonoBehaviour {

	// TODO distinguish mouse/keyboard and controller types

	private float triggerSensitivity = 0.1f;

	private string platformType;
	private string controllerType;
	private bool canSelectPS4 = false;
	public bool CanSelectPS4 { get { return canSelectPS4; } }

	public static int controlProfile = -1; // 0 = gamepad, 1 = keyboard & mouse, 2 = keyboard, 3 = PS4 on Mac/PC

	// Use this for initialization
	void Start () {

		if (controlProfile < 0){
			if (ControllerAttached()){
				if (DetermineControllerType() == 1){
					controlProfile = 3;
				}else{
				controlProfile = 0;
				}
			}else{
				controlProfile = 1;
			}
		}
		platformType = GetPlatform();
	
	}

	void Update(){

		if (Input.GetKeyDown(KeyCode.B)){
			Debug.Log("current control type: " + controlProfile); 
		}

	}

	string GetPlatform(){

		// assume pc, check for mac/linux
		string platform = "PC";

		if (controlProfile == 3){
			platform = "PS4";
			canSelectPS4 = true;
		}
		else if (Application.platform == RuntimePlatform.OSXEditor ||
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

	public int DetermineControllerType(){
		int numToReturn = 0;
		string[] joyStickNames = Input.GetJoystickNames();
		//Debug.Log(joyStickNames[0]);
		if (joyStickNames[0].Contains("Sony")){
			numToReturn = 1;
			canSelectPS4 = true;
		}
		return numToReturn;
	}


	//_________________________________________PUBLIC CONTROL CHECKS

	public float Horizontal(){

		if (GetPlatform() == "Mobile"){
			float moveHorizontal = CrossPlatformInputManager.GetAxis("Horizontal");
			
			return moveHorizontal;
		}
		else{
			if (ControllerAttached()){
				if (controlProfile == 0 || controlProfile == 3){
					return Input.GetAxis("HorizontalController");
				}else{
					return Input.GetAxis("Horizontal");
				}
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
				if (controlProfile == 0 || controlProfile == 3){
					return Input.GetAxis("VerticalController");
				}else{
					return Input.GetAxis("Vertical");
				}
			}
			else{
				return Input.GetAxis("Vertical");
			}
		}

	}

	public float HorizontalMenu(){

		// return all possible input types

		float horizontal = 0f;

		if (ControllerAttached()){
			if(Mathf.Abs(Input.GetAxis("HorizontalController")) > 0){
				horizontal = Input.GetAxis("HorizontalController");
			}else{
				horizontal = Input.GetAxis("Horizontal");
			}
		}
		else{
			horizontal = Input.GetAxis("Horizontal");
		}

		return horizontal;

	}

	public float VerticalMenu(){

		// return all possible input types

		float vertical = 0f;

			if (ControllerAttached()){
			if(Mathf.Abs(Input.GetAxis("VerticalController")) > 0){
				vertical = Input.GetAxis("VerticalController");
			}else{
				vertical = Input.GetAxis("Vertical");
			}
		}
			else{
				vertical = Input.GetAxis("Vertical");
			}

		return vertical;

	}

	public float HorizontalMovement(){

		if (GetPlatform() == "Mobile"){
			float moveHorizontal = CrossPlatformInputManager.GetAxis("Horizontal");

			return moveHorizontal;
		}
		else{
			if (ControllerAttached()){
				if (controlProfile == 0 || controlProfile == 3){
					return Input.GetAxis("HorizontalController");
				}else{
					return Input.GetAxis("HorizontalKeys");
				}
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
				if (controlProfile == 0 || controlProfile == 3){
					return Input.GetAxis("VerticalController");
				}else{
					return Input.GetAxis("VerticalKeys");
				}
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
			if (controlProfile == 0 || controlProfile == 3){
			return (Input.GetButton("ShootButton"+platformType));
			}else if (controlProfile == 1){
				return (Input.GetMouseButton(2) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
			}else{
				return (Input.GetKey(KeyCode.J));
			}
			//return (Input.GetButton("ReloadButton"+platformType));
		}else{
			//return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
			if (controlProfile == 1){
				return (Input.GetMouseButton(2) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
			}else{
				return (Input.GetKey(KeyCode.J));
			}
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
			if (controlProfile == 0 || controlProfile == 3){
			return (Input.GetButton("SwitchBuddyButton" + platformType));
			}else{
				return ( Input.GetKey(KeyCode.Space));
			}
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
			if (controlProfile == 0 || controlProfile == 3){
			return (Input.GetAxis("ShootTrigger"+platformType) > triggerSensitivity);
			}else {
				return (Input.GetKey(KeyCode.Space));
			}
			//return (Input.GetButton("DashButton"+platformType));
			
		}
		else{
			
			return (Input.GetKey(KeyCode.Space));
			
		}
		
	}
	
	public bool ShootButton(){

		if (ControllerAttached()){

			if (controlProfile == 0 || controlProfile == 3){
			return (Input.GetButton("SwitchButton"+platformType+"Alt"));
			}else if (controlProfile == 1){
				return (Input.GetMouseButton(0));
			}else{
				return (Input.GetKey(KeyCode.K));
					}

		}
		else{


			if (controlProfile == 1){
				return (Input.GetMouseButton(0));
			}else{
				return (Input.GetKey(KeyCode.K));
			}

		}
		
	}
	public bool HeavyButton(){
		
		if (ControllerAttached()){

			if (controlProfile == 0 || controlProfile == 3){
			return (Input.GetButton("ReloadButton"+platformType));
			}else if (controlProfile == 1){
				return (Input.GetMouseButton(1));
			}else{
				return (Input.GetKey(KeyCode.L));
			}
			
		}
		else{
			
			//return (Input.GetMouseButton(1));
			if (controlProfile == 1){
				return (Input.GetMouseButton(1));
			}else{
				return (Input.GetKey(KeyCode.L));
			}
			
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
			
			return (Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Keypad3));
			
		}
	}


	public bool TalkButton(){
		if (ControllerAttached()){

			if (controlProfile == 0 || controlProfile == 3){
			return (Input.GetButton("DashButton"+platformType));
			}else{
				return (Input.GetKey(KeyCode.E));
			}
			
		}
		else{

			return (Input.GetKey(KeyCode.E));
			
		}
	}

	public string CheckTalkString(){
		return ("DashButton"+platformType);
	}

	public bool SwitchButton(){



		if (ControllerAttached()){
			if (controlProfile == 0 || controlProfile == 3){
			return (Input.GetAxis("DashTrigger" + platformType) > triggerSensitivity);
			}else{
				return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
			}
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
			if (controlProfile == 0 || controlProfile == 3){
			return (Input.GetButton("StartButton"+platformType));
			}else{
				return(Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return));
			}
		}else{
			return (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return));
		}
	}
	public bool BackButton(){
		if (ControllerAttached()){
			if (controlProfile == 0 || controlProfile == 3){
			return (Input.GetButton("BackButton"+platformType));
			}else{
				return (Input.GetKey(KeyCode.Escape));
			}
		}else{
			return (Input.GetKey(KeyCode.Escape));
		}
	}

	public bool MenuSelectButton(){
		if (ControllerAttached()){
			if (controlProfile == 0 || controlProfile == 3){
			return (WeaponButtonA() || TalkButton());
			}else{
				return (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E));
			}
		}else{
			return (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E));
		}
	}

	public bool MenuSelectUp(){
		if (ControllerAttached()){
			if (controlProfile == 0 || controlProfile == 3){
			return (!WeaponButtonA() && !TalkButton());
			}else{
				return (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.E));
			}
		}else{
			return (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.E));
		}
	}

	public bool ExitButton(){
		if (ControllerAttached()){
			if (controlProfile == 0 || controlProfile == 3){
			return (WeaponButtonB() || WeaponButtonC());
			}else{
				return (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Delete)
					|| Input.GetKey(KeyCode.Q));	
			}
		}else{
			return (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Delete)
				|| Input.GetKey(KeyCode.Q));
		}
	}



	public bool ExitButtonUp(){
		if (ControllerAttached()){
			if (controlProfile == 0 || controlProfile == 3){
			return (!WeaponButtonB() && !WeaponButtonC());
			}else{
				return (!Input.GetKey(KeyCode.Escape) && !Input.GetKey(KeyCode.Backspace) && !Input.GetKey(KeyCode.Delete) && !Input.GetKey(KeyCode.Q));
			}
		}else{
			return (!Input.GetKey(KeyCode.Escape) && !Input.GetKey(KeyCode.Backspace) && !Input.GetKey(KeyCode.Delete) && !Input.GetKey(KeyCode.Q));
		}
	}

	public bool UseItemButton(){

		if (ControllerAttached()){
		
			if (controlProfile == 0 || controlProfile == 3){
			return (Input.GetButton("SwitchButton"+platformType));
			}else{
				return (Input.GetKey(KeyCode.R));
			}
		}else{
			return (Input.GetKey(KeyCode.R));
		}
	}
	public bool ToggleItemButton(){

		if (ControllerAttached()){

			if (controlProfile == 0 || controlProfile == 3){
				return (Input.GetButton("SwitchBuddyButton"+platformType));
			}else{
				return (Input.GetKey(KeyCode.Tab));
			}
		}else{
			return (Input.GetKey(KeyCode.Tab));
		}
	}
	public bool ScrollItemLeftButton(){
		//TODO add functionality for keyboard/mouse and linux
		if (ControllerAttached()){
			if (platformType == "Mac"){
				return (Input.GetButton("SwitchItemButtonLeftMac"));
			}else{
				if (controlProfile == 3){
					return (Input.GetAxis("SwitchItemAxisPS4") < -0.1f);
				}else{
				return (Input.GetAxis("SwitchItemAxisPC") < -0.1f);
				}
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
				if (controlProfile == 3){
					return (Input.GetAxis("SwitchItemAxisPS4") > 0.1f);
				}else{
				return (Input.GetAxis("SwitchItemAxisPC") > 0.1f);
				}
			}
		}else{
			return false;
		}
	}

	public void ChangeControlProfile(int dir){
		if (dir > 0){
			controlProfile ++;
			if ((controlProfile > 3 && canSelectPS4) || (controlProfile > 2 && !canSelectPS4)){
				if (ControllerAttached() && !canSelectPS4){
					controlProfile = 0;
				}else{
					controlProfile = 1;
				}
			}
		}else{
			controlProfile --;
			if (ControllerAttached() && !canSelectPS4){
				if (controlProfile < 0){
					controlProfile = 2;
				}
			}else{
				if (controlProfile < 1){
					if (canSelectPS4){
						controlProfile = 3;
					}else{
					controlProfile = 2;
					}
				}
			}
		}
	}
}
