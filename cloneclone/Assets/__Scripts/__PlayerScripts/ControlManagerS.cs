using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
#if UNITY_SWITCH
using nn.hid;
#endif

public class ControlManagerS : MonoBehaviour {

	// TODO distinguish mouse/keyboard and controller types
	// adding a comment here

	private float triggerSensitivity = 0.1f;

	private string platformType;
	private string truePlatform;
	private string controllerType;
    [HideInInspector]
	public bool canSelectPS4 = false;
	public bool CanSelectPS4 { get { return canSelectPS4; } }

	public static int controlProfile = -1; // 0 = gamepad, 1 = keyboard & mouse, 2 = keyboard, 3 = PS4 on Mac/PC
	public static List<int> savedGamepadControls;
    public static List<int> savedKeyboardControls;
    public static List<int> savedKeyboardandMouseControls;
    public static List<int> defaultGamepadControls = new List<int>(14){0,1,2,3,4,5,6,7,8,9,10,11,12,13};
    public static List<int> defaultKeyAndMouseControls = new List<int>(14) { 14, 15, 16, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

    bool useSwitch = false;

#if UNITY_SWITCH
    const int numberOfControllers = 2; //1 player + handheld
    NpadState[] npadStates = new NpadState[numberOfControllers];
    static bool switchInitialized = false;
    public TextMesh debugControl;
    private const float analogMax = 32750f; // was 32760f, but that felt too high and made movement sluggish
#endif

    // Use this for initialization
    void Start () {

#if UNITY_SWITCH
        useSwitch = true;
        InitializeSwitchControllers();
        controlProfile = 0;
        if (debugControl != null) {
            debugControl.gameObject.SetActive(true);
        }
#endif

        if (controlProfile < 0 && !useSwitch){
			if (ControllerAttached()){
				// Debug.Log("Controller attached !! " + Input.GetJoystickNames()[0]);
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
		truePlatform = GetTruePlatform();

		if (savedGamepadControls == null){
			savedGamepadControls = new List<int>(14){0,1,2,3,4,5,6,7,8,9,10,11,12,13};
		}else{
			if (savedGamepadControls.Count < defaultGamepadControls.Count){
				savedGamepadControls.Clear();
				for (int i = 0; i < defaultGamepadControls.Count; i++){
					savedGamepadControls.Add(defaultGamepadControls[i]);
				}
			}
		}

        if (savedKeyboardControls == null)
        {
            savedKeyboardControls = new List<int>(14) { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        }
        else
        {
            if (savedKeyboardControls.Count < defaultGamepadControls.Count)
            {
                savedKeyboardControls.Clear();
                for (int i = 0; i < defaultGamepadControls.Count; i++)
                {
                    savedKeyboardControls.Add(defaultGamepadControls[i]);
                }
            }
        }

        if (savedKeyboardandMouseControls == null)
        {
            savedKeyboardandMouseControls = new List<int>(14) { 14, 15, 16, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        }
        else
        {
            if (savedKeyboardandMouseControls.Count < defaultKeyAndMouseControls.Count)
            {
                savedKeyboardandMouseControls.Clear();
                for (int i = 0; i < defaultKeyAndMouseControls.Count; i++)
                {
                    savedKeyboardandMouseControls.Add(defaultKeyAndMouseControls[i]);
                }
            }
        }


	}

#if UNITY_SWITCH
    /*private void Update()
    {
        if (debugControl != null && switchInitialized) {
            debugControl.text = "H: " + SwitchHorizontal() + " V: " + SwitchVertical();
        }
    }**/
    void InitializeSwitchControllers() {
        if (switchInitialized) {
            return;
        }
        // init the controller states
        for (int i = 0; i < numberOfControllers; i++)
        {
            npadStates[i] = new NpadState();
        }

        Npad.Initialize();

        // Set supported styles
        // FullKey = Pro Controller
        // JoyDual = Two Joy-Con used as one controller
        // see nn::hid::SetSupportedNpadStyleSet (NpadStyleSet style) in the SDK docs for API details
        Npad.SetSupportedStyleSet(NpadStyle.FullKey | NpadStyle.JoyDual | NpadStyle.Handheld);

        // NpadJoy.SetHoldType only affects how controllers behave when using a system applet.
        // NpadJoyHoldType.Vertical is the default setting, but most games would want to use NpadJoyHoldType.Horizontal as it can support all controller styles
        // If you use NpadJoyHoldType.Vertical,  Npad.SetSupportedStyleSet must only list controller types supported by NpadJoyHoldType.Vertical.
        // If you don't, the controller applet will fail to load.
        // Supported types for NpadJoyHoldType.Vertical are: NpadStyle.JoyLeft, NpadStyle.JoyRight
        NpadJoy.SetHoldType(NpadJoyHoldType.Horizontal);

        NpadJoy.SetHandheldActivationMode(NpadHandheldActivationMode.Dual); //both controllers must be docked for handheld mode.

        //You must call Npad.SetSupportedIdType for all supported controllers.
        //Your game may run if you don't call this, but not calling this may lead to crashes in some circumstances.
        NpadId[] npadIds = { NpadId.Handheld, NpadId.No1};
        Npad.SetSupportedIdType(npadIds);

        callControllerApplet();
        switchInitialized = true;
    }

    void callControllerApplet()
    {
        #if !UNITY_EDITOR
        // set the arguments for the applet
        // see nn::hid::ControllerSupportArg::SetDefault () in the SDK documentation for details
        ControllerSupportArg controllerSupportArgs = new ControllerSupportArg();
        controllerSupportArgs.SetDefault();
            
        controllerSupportArgs.enableSingleMode = true;
        

        nn.hid.ControllerSupport.Show(controllerSupportArgs);
        #endif
    }

    float HandleSwitchInputAxis(int axis) {

        // axis determines which axis to check, 0 = horizontal, 1 = vertical, 2 = r horizontal, 3 = r vertical
        float axisValue = 0f;
        for (int i = 0; i < numberOfControllers; i++) {
            NpadStyle npadStyle;
            if (i == 0)
            {
                npadStyle = Npad.GetStyleSet(NpadId.Handheld);
                Npad.GetState(ref npadStates[i], NpadId.Handheld, npadStyle);
                if (npadStyle != NpadStyle.None)
                {



                    // now get appropriate axis reading
                    switch (axis)
                    {
                        case 0:
                            axisValue = npadStates[i].analogStickL.x;
                            break;
                        case 1:
                            axisValue = npadStates[i].analogStickL.y;
                            break;
                        case 3:
                            axisValue = npadStates[i].analogStickR.x;
                            break;
                        case 4:
                            axisValue = npadStates[i].analogStickR.y;
                            break;
                        default:
                            axisValue = 0f;
                            break;
                    }
                }
            }
            else
            {
                npadStyle = Npad.GetStyleSet(NpadId.No1);
                Npad.GetState(ref npadStates[i], NpadId.No1, npadStyle);
                if (npadStyle != NpadStyle.None)
                {


                    // now get appropriate axis reading
                    switch (axis)
                    {
                        case 0:
                            axisValue = npadStates[i].analogStickL.x;
                            break;
                        case 1:
                            axisValue = npadStates[i].analogStickL.y;
                            break;
                        case 3:
                            axisValue = npadStates[i].analogStickR.x;
                            break;
                        case 4:
                            axisValue = npadStates[i].analogStickR.y;
                            break;
                        default:
                            axisValue = 0f;
                            break;
                    }
                }
            }
        }

        return axisValue/analogMax;

    }

    bool HandleSwitchInputButton(int button)
    {

        // axis determines which button to check, 0 = A, 1 = B, 2 = X, 3 = Y
        // 4 = +, 5 = -, 6 = Up, 7 = Down, 8 = Left, 9 = Right, 
        // 10 = R, 11 = ZR, 12 = L, 13 = ZL
        bool buttonDown = false;
        for (int i = 0; i < numberOfControllers; i++)
        {
            NpadStyle npadStyle;
            if (i == 0)
            {
                npadStyle = Npad.GetStyleSet(NpadId.Handheld);
                Npad.GetState(ref npadStates[i], NpadId.Handheld, npadStyle);
                if (npadStyle != NpadStyle.None)
                {

                    // now get appropriate axis reading
                    switch (button)
                    {
                        case 0:
                            buttonDown = npadStates[i].GetButton(NpadButton.A);
                            break;
                        case 1:
                            buttonDown = npadStates[i].GetButton(NpadButton.B);
                            break;
                        case 2:
                            buttonDown = npadStates[i].GetButton(NpadButton.X);
                            break;
                        case 3:
                            buttonDown = npadStates[i].GetButton(NpadButton.Y);
                            break;
                        case 4:
                            buttonDown = npadStates[i].GetButton(NpadButton.Plus);
                            break;
                        case 5:
                            buttonDown = npadStates[i].GetButton(NpadButton.Minus);
                            break;
                        case 6:
                            buttonDown = npadStates[i].GetButton(NpadButton.Up);
                            break;
                        case 7:
                            buttonDown = npadStates[i].GetButton(NpadButton.Down);
                            break;

                        case 8:
                            buttonDown = npadStates[i].GetButton(NpadButton.Left);
                            break;

                        case 9:
                            buttonDown = npadStates[i].GetButton(NpadButton.Right);
                            break;
                        case 10:
                            buttonDown = npadStates[i].GetButton(NpadButton.R);
                            break;
                        case 11:
                            buttonDown = npadStates[i].GetButton(NpadButton.ZR);
                            break;

                        case 12:
                            buttonDown = npadStates[i].GetButton(NpadButton.L);
                            break;

                        case 13:
                            buttonDown = npadStates[i].GetButton(NpadButton.ZL);
                            break;
                        default:
                            buttonDown = false;
                            break;
                    }
                }
            }
            else
            {
                npadStyle = Npad.GetStyleSet(NpadId.No1);
                Npad.GetState(ref npadStates[i], NpadId.No1, npadStyle);
                if (npadStyle != NpadStyle.None)
                {


                    // now get appropriate axis reading
                    switch (button)
                    {
                        case 0:
                            buttonDown = npadStates[i].GetButton(NpadButton.A);
                            break;
                        case 1:
                            buttonDown = npadStates[i].GetButton(NpadButton.B);
                            break;
                        case 2:
                            buttonDown = npadStates[i].GetButton(NpadButton.X);
                            break;
                        case 3:
                            buttonDown = npadStates[i].GetButton(NpadButton.Y);
                            break;
                        case 4:
                            buttonDown = npadStates[i].GetButton(NpadButton.Plus);
                            break;
                        case 5:
                            buttonDown = npadStates[i].GetButton(NpadButton.Minus);
                            break;
                        case 6:
                            buttonDown = npadStates[i].GetButton(NpadButton.Up);
                            break;
                        case 7:
                            buttonDown = npadStates[i].GetButton(NpadButton.Down);
                            break;

                        case 8:
                            buttonDown = npadStates[i].GetButton(NpadButton.Left);
                            break;

                        case 9:
                            buttonDown = npadStates[i].GetButton(NpadButton.Right);
                            break;
                        case 10:
                            buttonDown = npadStates[i].GetButton(NpadButton.R);
                            break;
                        case 11:
                            buttonDown = npadStates[i].GetButton(NpadButton.ZR);
                            break;

                        case 12:
                            buttonDown = npadStates[i].GetButton(NpadButton.L);
                            break;

                        case 13:
                            buttonDown = npadStates[i].GetButton(NpadButton.ZL);
                            break;
                        default:
                            buttonDown = false;
                            break;
                    }
                }
            }
        }

        return buttonDown;

    }
#endif

    public static void LoadControls(List<int> keyboardControls, List<int> gamepadControls, List<int> mouseControls){
        savedKeyboardControls = keyboardControls;
        savedGamepadControls = gamepadControls;
        savedKeyboardandMouseControls = mouseControls;
    }

	string GetPlatform(){

#if UNITY_SWITCH
        return "Switch";
#endif

        // assume pc, check for mac/linux
        string platform = "PC";

		if (controlProfile == 3){
			platform = "PS4";
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

	string GetTruePlatform()
    {
#if UNITY_SWITCH
        return "Switch";
#endif
        string platform = "PC";
		if (Application.platform == RuntimePlatform.OSXEditor ||
			Application.platform == RuntimePlatform.OSXPlayer){

			platform = "Mac";

		}
		else if (Application.platform == RuntimePlatform.LinuxPlayer){

			platform = "Linux";

		}
		return platform;
	}

	//_________________________________________CONTROLLER CHECK

	public bool ControllerAttached()
    {
#if UNITY_SWITCH
        return true;
#endif

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
        if ((joyStickNames[0].Contains("Sony") || joyStickNames[0].Contains("Unknown") || joyStickNames[0] == "Wireless Controller")
            && !joyStickNames[0].Contains("Xbox") && !joyStickNames[0].Contains("XBox") && !joyStickNames[0].Contains("XBOX")){
			numToReturn = 1;
			canSelectPS4 = true;
		}
		return numToReturn;
	}


	//_________________________________________PUBLIC CONTROL CHECKS

	public float Horizontal(){

#if UNITY_SWITCH
        return SwitchHorizontal();
#endif

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

#if UNITY_SWITCH
    float SwitchHorizontal() {
        return HandleSwitchInputAxis(0);
        }
        float SwitchVertical()
        {
            return HandleSwitchInputAxis(1);
        }
        float SwitchHorizontalRight()
        {
            return HandleSwitchInputAxis(2);
        }
        float SwitchVerticalRight()
        {
            return HandleSwitchInputAxis(3);
        }
#endif

        public float Vertical()
        {

#if UNITY_SWITCH
            return SwitchVertical();
#endif

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
#if UNITY_SWITCH
            return SwitchHorizontal();
#endif

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
#if UNITY_SWITCH
            return SwitchVertical();
#endif

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


#if UNITY_SWITCH
            return SwitchHorizontal();
#endif

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


#if UNITY_SWITCH
            return SwitchVertical();
#endif

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

#if UNITY_SWITCH
            return SwitchHorizontalRight();
#endif
            if (controlProfile == 3){
			if (truePlatform == "PC"){
				return Input.GetAxis("RightHorizontalController" + platformType + "PC");
			}else{
		return Input.GetAxis("RightHorizontalController" + platformType);
			}
		}else{
			return Input.GetAxis("RightHorizontalController" + platformType);
		}
		
	}
	
	public float RightVertical(){

#if UNITY_SWITCH
            return SwitchVerticalRight();
#endif
            if (controlProfile == 3){
			if (truePlatform == "PC"){
				return Input.GetAxis("RightVerticalController" + platformType + "PC");
			}else{
				return Input.GetAxis("RightVerticalController" + platformType);
			}
		}else{
		return Input.GetAxis("RightVerticalController" + platformType);
		}
		
	}

	public bool TransformButton(){
#if UNITY_SWITCH
        return Mathf.Abs(HandleSwitchInputAxis(2)) > 0.1f ||
                    (Mathf.Abs(HandleSwitchInputAxis(3)) > 0.1f);
#endif
        if (controlProfile == 3){
			if (truePlatform == "PC"){
				return (Mathf.Abs(Input.GetAxis("RightVerticalController" + platformType + "PC")) > 0.1f ||
					(Mathf.Abs(Input.GetAxis("RightHorizontalController" + platformType + "PC")) > 0.1f));
			}else{
				return (Mathf.Abs(Input.GetAxis("RightVerticalController" + platformType)) > 0.1f ||
					(Mathf.Abs(Input.GetAxis("RightHorizontalController" + platformType)) > 0.1f));
			}
		}else if (controlProfile == 0){
			return (Mathf.Abs(Input.GetAxis("RightVerticalController" + platformType)) > 0.1f ||
				(Mathf.Abs(Input.GetAxis("RightHorizontalController" + platformType)) > 0.1f));
		}else{
			return (Input.GetKey(KeyCode.F));
		}
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

	public bool GetCustomInput(int inputIndex){
        if (controlProfile == 0 || controlProfile == 3)
        {
            return (GetInputPressed(savedGamepadControls[inputIndex]));
        }
        else if (controlProfile == 1)
        {
            return (GetInputPressed(savedKeyboardandMouseControls[inputIndex]));
        }
        else
        {
            return (GetInputPressed(savedKeyboardControls[inputIndex]));
        }
	}

    public void SetCustomInput(int actionBeingSet, int newButton){
        if (controlProfile == 0 || controlProfile == 3)
        {
            // replace gamepad
            int buttonBeingReplaced = savedGamepadControls[actionBeingSet];
            savedGamepadControls[actionBeingSet] = savedGamepadControls[newButton];
            savedGamepadControls[newButton] = buttonBeingReplaced;
        }else if (controlProfile == 1){
            // set mouse & key control

            int buttonBeingReplaced = savedKeyboardandMouseControls[actionBeingSet];
            if (savedKeyboardandMouseControls.Contains(newButton))
            {
                savedKeyboardandMouseControls[savedKeyboardandMouseControls.IndexOf(newButton)] = buttonBeingReplaced;

            }
                savedKeyboardandMouseControls[actionBeingSet] = newButton;
        }else if (controlProfile == 2){
            // set key only control

            int buttonBeingReplaced = savedKeyboardControls[actionBeingSet];
            if (savedKeyboardControls.Contains(newButton))
            {
                savedKeyboardControls[savedKeyboardControls.IndexOf(newButton)] = buttonBeingReplaced;
               
            }

            savedKeyboardControls[actionBeingSet] = newButton;
        }
    }

	private bool GetInputPressed(int inputValue, bool overrideToKey = false){
		bool inputPressed = false;
        if (((controlProfile == 0 || controlProfile == 3) && !overrideToKey) || useSwitch)
        {
            switch (inputValue)
            {
                case (0):
                    // return default light attack
                    inputPressed = ShootButton();
                    break;
                case (1):
                    // return default heavy attack
                    inputPressed = HeavyButton();
                    break;
                case (2):
                    // return default familiar attack
                    inputPressed = FamiliarControl();
                    break;
                case (3):
                    // return default talk button
                    inputPressed = TalkButton();
                    break;
                case (4):
                    // return default dodge input
                    inputPressed = DashTrigger();
                    break;
                case (5):
                    // return default shift input
                    inputPressed = SwitchButton();
                    break;
                case (6):
                    // return default use item button
                    inputPressed = UseItemButton();
                    break;
                case (7):
                    // return default switch item button
                    inputPressed = ToggleItemButton();
                    break;
                case (8):
                    // return default taunt input
                    inputPressed = TauntButton();
                    break;
                case (9):
                    // return default transform input 
                    inputPressed = TransformButton();
                    break;
                case (10):
                    // return default equip menu button
                    inputPressed = StartButton();
                    break;
                case (11):
                    // return default game menu button
                    inputPressed = BackButton();
                    break;
                case (12):
                    // return default menu select button (on Switch, these will be swapped w/ExitButton)
                    inputPressed = MenuSelectButton();
                    break;
                case (13):
                    // return default menu cancel button (on Switch, these will be swapped w/TalkButton)
                    inputPressed = ExitButton();
                    break;


                default:
                    inputPressed = false;
                    break;
            }
        }else{
            switch (inputValue)
            {
                case (0):
                    // return default light attack
                    inputPressed = Input.GetKey(KeyCode.K);

                    break;
                case (1):
                    // return default heavy attack
                    inputPressed = Input.GetKey(KeyCode.L);
                    break;
                case (2):
                    // return default familiar attack
                    inputPressed = Input.GetKey(KeyCode.J);
                    break;
                case (3):
                    // return default talk button
                    inputPressed = Input.GetKey(KeyCode.E);
                    break;
                case (4):
                    // return default dodge input
                    inputPressed = Input.GetKey(KeyCode.Space);
                    break;
                case (5):
                    // return default shift input
                    inputPressed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                    break;
                case (6):
                    // return default use item button
                    inputPressed = Input.GetKey(KeyCode.R);
                    break;
                case (7):
                    // return default switch item button
                    inputPressed = Input.GetKey(KeyCode.Tab);
                    break;
                case (8):
                    // return default taunt input (Q)
                    inputPressed = Input.GetKey(KeyCode.Q);
                    break;
                case (9):
                    // return default transform input 
                    inputPressed = Input.GetKey(KeyCode.F);
                    break;
                case (10):
                    // return default equip menu button
                    inputPressed = (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter));
                    break;
                case (11):
                    // return default game menu button
                    inputPressed = Input.GetKey(KeyCode.Escape);
                    break;
                case (12):
                    // return default menu select button (E)
                    inputPressed = Input.GetKey(KeyCode.E);
                    break;
                case (13):
                    // return default menu cancel button (Q)
                    inputPressed = Input.GetKey(KeyCode.Q);
                    break;
                case (14):
                    // return default light attack (mouse)
                    inputPressed = Input.GetMouseButton(0);

                    break;
                case (15):
                    // return default heavy attack (mouse)
                    inputPressed = Input.GetMouseButton(1);
                    break;
                case (16):
                    // return default familiar attack (mouse)
                    inputPressed = Input.GetMouseButton(2);
                    break;
                case(17):
                    // return T Key
                    inputPressed = Input.GetKey(KeyCode.T);
                    break;
                case (18):
                    // return Y Key
                    inputPressed = Input.GetKey(KeyCode.Y);
                    break;
                case (19):
                    // return U Key
                    inputPressed = Input.GetKey(KeyCode.U);
                    break;
                case (20):
                    // return I Key
                    inputPressed = Input.GetKey(KeyCode.I);
                    break;
                case (21):
                    // return O Key
                    inputPressed = Input.GetKey(KeyCode.O);
                    break;
                case (22):
                    // return P Key
                    inputPressed = Input.GetKey(KeyCode.P);
                    break;
                case (23):
                    // return [ Key
                    inputPressed = Input.GetKey(KeyCode.LeftBracket);
                    break;
                case (24):
                    // return ] Key
                    inputPressed = Input.GetKey(KeyCode.RightBracket);
                    break;
                case (25):
                    // return \ Key
                    inputPressed = Input.GetKey(KeyCode.Backslash);
                    break;
                case (26):
                    // return G Key
                    inputPressed = Input.GetKey(KeyCode.G);
                    break;
                case (27):
                    // return H Key
                    inputPressed = Input.GetKey(KeyCode.H);
                    break;
                case (28):
                    // return ; Key
                    inputPressed = Input.GetKey(KeyCode.Semicolon);
                    break;
                case (29):
                    // return ' Key
                    inputPressed = Input.GetKey(KeyCode.Quote);
                    break;
                case (30):
                    // return Z Key
                    inputPressed = Input.GetKey(KeyCode.Z);
                    break;
                case (31):
                    // return X Key
                    inputPressed = Input.GetKey(KeyCode.X);
                    break;
                case (32):
                    // return C Key
                    inputPressed = Input.GetKey(KeyCode.C);
                    break;
                case (33):
                    // return V Key
                    inputPressed = Input.GetKey(KeyCode.V);
                    break;
                case (34):
                    // return B Key
                    inputPressed = Input.GetKey(KeyCode.B);
                    break;
                case (35):
                    // return N Key
                    inputPressed = Input.GetKey(KeyCode.N);
                    break;
                case (36):
                    // return M Key
                    inputPressed = Input.GetKey(KeyCode.M);
                    break;
                case (37):
                    // return , Key
                    inputPressed = Input.GetKey(KeyCode.Comma);
                    break;
                case (38):
                    // return . Key
                    inputPressed = (Input.GetKey(KeyCode.Period) || Input.GetKey(KeyCode.KeypadPeriod));
                    break;
                case(39):
                    // return / key
                    inputPressed = Input.GetKey(KeyCode.Slash);
                    break;
                case (40):
                    // return control key
                    inputPressed = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
                    break;
                case (41):
                    // return alt key
                    inputPressed = (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
                    break;
                case (42):
                    // return cmd key
                    inputPressed = (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand));
                    break;
                case (43):
                    // return ` key
                    inputPressed = Input.GetKey(KeyCode.BackQuote);
                    break;
                case (44):
                    // return 1 key
                    inputPressed = (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1));
                    break;
                case (45):
                    // return 2 key
                    inputPressed = (Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2));
                    break;
                case (46):
                    // return 3 key
                    inputPressed = (Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Keypad3));
                    break;
                case (47):
                    // return 4 key
                    inputPressed = (Input.GetKey(KeyCode.Alpha4) || Input.GetKey(KeyCode.Keypad4));
                    break;
                case (48):
                    // return 5 key
                    inputPressed = (Input.GetKey(KeyCode.Alpha5) || Input.GetKey(KeyCode.Keypad5));
                    break;
                case (49):
                    // return 6 key
                    inputPressed = (Input.GetKey(KeyCode.Alpha6) || Input.GetKey(KeyCode.Keypad6));
                    break;
                case (50):
                    // return 7 key
                    inputPressed = (Input.GetKey(KeyCode.Alpha7) || Input.GetKey(KeyCode.Keypad7));
                    break;
                case (51):
                    // return 8 key
                    inputPressed = (Input.GetKey(KeyCode.Alpha8) || Input.GetKey(KeyCode.Keypad8));
                    break;
                case (52):
                    // return 9 key
                    inputPressed = (Input.GetKey(KeyCode.Alpha9) || Input.GetKey(KeyCode.Keypad9));
                    break;
                case (53):
                    // return 0 key
                    inputPressed = (Input.GetKey(KeyCode.Alpha0) || Input.GetKey(KeyCode.Keypad0));
                    break;
                case (54):
                    // return - key
                    inputPressed = (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus));
                    break;
                case (55):
                    // return = key
                    inputPressed = (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadEquals));
                    break;
                case (56):
                    // return delete key
                    inputPressed = (Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Delete));
                    break;


                default:
                    inputPressed = false;
                    break;
            }
        }
		return inputPressed;
	}

    public int CheckForKeyPress(bool includeMenu = false){
        int returnKey = -1;
        for (int i = 0; i <= 56; i++){

        // dont check for menu cancel/select keys
            if (i >= 12 && i <= 13 && !includeMenu){
                continue;
            }
            if (GetInputPressed(i, true)){
                returnKey = i;
            }
        }
        return returnKey;
    }
    public int CheckForButtonPress(bool includeMenu = false){
        int returnKey = -1;
        for (int i = 0; i <= 13; i++)
        {

            // dont check for menu cancel/select keys
            if (i >= 12 && !includeMenu)
            {
                continue;
            }
            if (GetInputPressed(i))
            {
                returnKey = i;
            }
        }
        return returnKey;
    }

	public bool FamiliarControl(){
#if UNITY_SWITCH
        return HandleSwitchInputButton(3);
#endif
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

#if UNITY_SWITCH
        return HandleSwitchInputButton(11);
#endif

        if (ControllerAttached()){
			
			//return (Input.GetAxis("DashTrigger"+platformType) > triggerSensitivity);
			if (controlProfile == 0){
			return (Input.GetAxis("ShootTrigger"+platformType) > triggerSensitivity);
			}else if (controlProfile == 3){
				if (truePlatform == "PC"){
					return (Input.GetAxis("ShootTrigger"+platformType+"PC") > triggerSensitivity);}
				else{
					return (Input.GetAxis("ShootTrigger"+platformType) > triggerSensitivity);
				}
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

#if UNITY_SWITCH
        return HandleSwitchInputButton(2);
#endif
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

#if UNITY_SWITCH
        return HandleSwitchInputButton(0);
#endif

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

#if UNITY_SWITCH
        return HandleSwitchInputButton(1);
#endif
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


#if UNITY_SWITCH
        return HandleSwitchInputButton(13);
#endif

        if (ControllerAttached()){
			if (controlProfile == 0 ){
			return (Input.GetAxis("DashTrigger" + platformType) > triggerSensitivity);
			}else if (controlProfile == 3){
				if (truePlatform == "PC"){
					return (Input.GetAxis("DashTrigger" + platformType + "PC") > triggerSensitivity);
				}else{
				return (Input.GetAxis("DashTrigger" + platformType) > triggerSensitivity);
				}
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

	public bool StartButton()
    {
#if UNITY_SWITCH
        return HandleSwitchInputButton(4);
#endif
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
	public bool BackButton()
    {
#if UNITY_SWITCH
        return HandleSwitchInputButton(5);
#endif
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

#if UNITY_SWITCH
        return HeavyButton();
#endif
        if (ControllerAttached()){
			if (controlProfile == 0 || controlProfile == 3){
			return (TalkButton());
			}else{
				return (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E));
			}
		}else{
			return (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E));
		}
	}

	public bool MenuSelectUp()
    {
#if UNITY_SWITCH
        return !HeavyButton();
#endif
        if (ControllerAttached()){
			if (controlProfile == 0 || controlProfile == 3){
                return (!TalkButton());
			}else{
				return (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.E));
			}
		}else{
			return (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.E));
		}
	}

	public bool ExitButton()
    {
#if UNITY_SWITCH
        return TalkButton();
#endif
        if (ControllerAttached()){
			if (controlProfile == 0 || controlProfile == 3){
                return (HeavyButton());
			}else{
				return (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Delete)
					|| Input.GetKey(KeyCode.Q));	
			}
		}else{
			return (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Delete)
				|| Input.GetKey(KeyCode.Q));
		}
	}



	public bool ExitButtonUp()
    {
#if UNITY_SWITCH
        return !TalkButton();
#endif
        if (ControllerAttached()){
			if (controlProfile == 0 || controlProfile == 3){
                return (!HeavyButton());
			}else{
				return (!Input.GetKey(KeyCode.Escape) && !Input.GetKey(KeyCode.Backspace) && !Input.GetKey(KeyCode.Delete) && !Input.GetKey(KeyCode.Q));
			}
		}else{
			return (!Input.GetKey(KeyCode.Escape) && !Input.GetKey(KeyCode.Backspace) && !Input.GetKey(KeyCode.Delete) && !Input.GetKey(KeyCode.Q));
		}
	}

	public bool UseItemButton(){

#if UNITY_SWITCH
        return HandleSwitchInputButton(10);
#endif

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
	public bool ToggleItemButton()
    {
#if UNITY_SWITCH
        return HandleSwitchInputButton(12);
#endif

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
	public bool TauntButton()
    {
#if UNITY_SWITCH
        return HandleSwitchInputButton(6) || HandleSwitchInputButton(7) || HandleSwitchInputButton(8) || HandleSwitchInputButton(9);
#endif
        //TODO test this please, add rest of dpad and add keyboard default
        if (ControllerAttached()){
			if (platformType == "Mac"){
				return (Input.GetButton("SwitchItemButtonLeftMac") || Input.GetButton("SwitchItemButtonRightMac")
					|| Input.GetButton("SwitchItemButtonUpMac") || Input.GetButton("SwitchItemButtonUpMac"));
            }else if (platformType == "Linux"){
                return (Mathf.Abs(Input.GetAxis("SwitchItemAxisLinux")) > 0.1f || Mathf.Abs(Input.GetAxis("UseItemAxisLinux")) > 0.1f
                        || Input.GetButton("TauntButtonUpLinux") || Input.GetButton("TauntButtonDownLinux")
                        || Input.GetButton("TauntButtonLeftLinux") || Input.GetButton("TauntButtonRightLinux"));
            }
            else{
				if (controlProfile == 3){
                    return (Mathf.Abs(Input.GetAxis("SwitchItemAxisPS4PC")) > 0.1f || Mathf.Abs(Input.GetAxis("UseItemAxisPS4PC")) > 0.1f);
				}else{
                    return (Mathf.Abs(Input.GetAxis("SwitchItemAxisPC")) > 0.1f || Mathf.Abs(Input.GetAxis("UseItemAxisPC")) > 0.1f);
				}
			}
		}else{
			return (Input.GetKey(KeyCode.Q));
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

	public bool ToggleMapButton(){

#if UNITY_SWITCH
        return HandleSwitchInputButton(10) || HandleSwitchInputButton(12);
#endif
        if (ControllerAttached()){

			if (controlProfile == 0 || controlProfile == 3){
				return (Input.GetButton("SwitchButton"+platformType) || Input.GetButton("SwitchBuddyButton"+platformType));
			}else{
				return (Input.GetKey(KeyCode.Tab));
			}
		}else{
			return (Input.GetKey(KeyCode.Tab));
		}
	}

	public void ChangeControlProfile(int dir){
#if UNITY_SWITCH
        controlProfile = 0;
        return;
#endif
        if (dir > 0){
			controlProfile ++;
			if (controlProfile > 3){
				controlProfile = 1;
			}
			if (controlProfile > 2 && !canSelectPS4){
				if (ControllerAttached()){
					controlProfile = 0;
				}else{
					controlProfile = 1;
				}
			}
		}else{
			controlProfile --;
			if (canSelectPS4){
				if (controlProfile < 1){
					controlProfile = 3;
				}
			}else{
				if (ControllerAttached()){
					if (controlProfile < 0){
						controlProfile = 2;
					}
				}else{
					if (controlProfile < 1){
						controlProfile = 3;
					}
				}
			}
		}
	}

    public void RestoreDefaults(){

        if (controlProfile == 1){
            // restore keyboard/mouse
            if (savedKeyboardandMouseControls == null)
            {
                savedKeyboardandMouseControls = new List<int>(14) { 14, 15, 16, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            }
            else
            {
                
                    savedKeyboardandMouseControls.Clear();
                    for (int i = 0; i < defaultKeyAndMouseControls.Count; i++)
                    {
                        savedKeyboardandMouseControls.Add(defaultKeyAndMouseControls[i]);
                    }

            }
        }else if (controlProfile == 2){
            // restore key only
            if (savedKeyboardControls == null)
            {
                savedKeyboardControls = new List<int>(14) { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            }
            else
            {
                
                    savedKeyboardControls.Clear();
                    for (int i = 0; i < defaultGamepadControls.Count; i++)
                    {
                        savedKeyboardControls.Add(defaultGamepadControls[i]);
                    }

            }

        }else{
            // restore gamepad
            if (savedGamepadControls == null)
            {
                savedGamepadControls = new List<int>(14) { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            }
            else
            {
                
                    savedGamepadControls.Clear();
                    for (int i = 0; i < defaultGamepadControls.Count; i++)
                    {
                        savedGamepadControls.Add(defaultGamepadControls[i]);
                    }

            }
        }

    }
}
