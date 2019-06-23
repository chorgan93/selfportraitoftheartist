using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextInputUIS : MonoBehaviour {

	public static string playerName = "LUCAH";
	public GameObject inputUI;
	public RectTransform[] selectorPos;
	public RectTransform selector;
	public Text[] textFields;
	public Text[] textFieldsUnder;
	private int[] chosenLetters;
	private int currentLetter = 0;
	private int currentPos = 0;

	private string fullInput = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";

	[Header("Input Types")]
	public bool playerNameInput;

	private ControlManagerS myControl;
	private InGameCinemaTextS myCinema;


	private bool stickReset = false;
	private bool selectButtonDown = false;
	private bool backButtonDown = false;
	private bool startButtonDown = false;

	private bool canEnter = false;

	private float delayInput;

    private bool[] inputsUp = new bool[28];


	// Use this for initialization
	void Start () {
	
		GameObject findPlayer = GameObject.Find("Player");
		if (findPlayer){
			myControl = findPlayer.GetComponent<ControlManagerS>();
		}else{
			myControl = GetComponent<ControlManagerS>();
		}

		chosenLetters = new int[8]{0,fullInput.Length-1,fullInput.Length-1,fullInput.Length-1,fullInput.Length-1,
			fullInput.Length-1,fullInput.Length-1,fullInput.Length-1};
		RefreshTexts();
		inputUI.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (delayInput > 0 && inputUI.activeSelf){
			delayInput -= Time.deltaTime;
		}
		else if (inputUI.activeSelf){
            // gamepad version of input
            if (ControlManagerS.controlProfile == 0 || ControlManagerS.controlProfile == 3)
            {
#if UNITY_SWITCH
                if (stickReset && Mathf.Abs(myControl.HorizontalMenu()) > 0.45f)
#else
                if (stickReset && Mathf.Abs(myControl.HorizontalMenu()) > 0.1f)
#endif
                {
                    stickReset = false;
                    if (myControl.HorizontalMenu() < 0)
                    {
                        ChangeCurrentLetter(-1);
                    }
                    else
                    {
                        ChangeCurrentLetter(1);
                    }
                }
#if UNITY_SWITCH
                else if (stickReset && Mathf.Abs(myControl.VerticalMenu()) > 0.45f)
#else
                if (stickReset && Mathf.Abs(myControl.VerticalMenu()) > 0.1f)
#endif
                {
                    stickReset = false;
                    if (myControl.VerticalMenu() < 0)
                    {
                        ChangeCurrentLetter(-1);
                    }
                    else
                    {
                        ChangeCurrentLetter(1);
                    }
                }
                if (!stickReset && Mathf.Abs(myControl.HorizontalMenu()) < 0.1f && Mathf.Abs(myControl.VerticalMenu()) < 0.1f)
                {
                    stickReset = true;
                }

                if (!selectButtonDown && myControl.GetCustomInput(3))
                {
                    selectButtonDown = true;
                    SetLetter(true);
                }
                if (selectButtonDown && !myControl.GetCustomInput(3))
                {
                    selectButtonDown = false;
                }

                if (!backButtonDown && myControl.GetCustomInput(1))
                {
                    backButtonDown = true;
                    PressDel();
                }
                if (backButtonDown && !myControl.GetCustomInput(1))
                {
                    backButtonDown = false;
                }

                if (!startButtonDown && myControl.GetCustomInput(10))
                {
                    startButtonDown = true;
                    if (canEnter)
                    {
                        FinishInput();
                    }
                }
                if (startButtonDown && !myControl.GetCustomInput(10))
                {
                    startButtonDown = false;
                }
            }else{
                // keyboard version of input
                UpdateKeysUp();
                CheckForInput();

                if (!startButtonDown && (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return)))
                {
                    startButtonDown = true;
                    if (canEnter)
                    {
                        FinishInput();
                    }
                }
                if (startButtonDown && (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return)))
                {
                    startButtonDown = false;
                }
            }
		}
	}

	public void Activate(InGameCinemaTextS newCinema){
		canEnter = false;
		currentLetter = 0;
		currentPos = 0;
		selector.anchoredPosition = selectorPos[currentPos].anchoredPosition;
		SetLetter(false);
		delayInput = 0.3f;
		myCinema = newCinema;
		inputUI.SetActive(true);
	}

	void RefreshTexts(){
		for (int i = 0; i < textFields.Length; i++){
			textFields[i].text = textFieldsUnder[i].text = " ";
		}
	}

	void ChangeCurrentLetter(int dir){
		if (dir > 0){
			currentLetter++;
			if (currentLetter >= fullInput.Length){
				currentLetter = 0;
			}
		}else{
			currentLetter--;
			if (currentLetter < 0){
				currentLetter = fullInput.Length-1;
			}
		}
		SetLetter(false);
	}

	void SetLetter(bool advance = false, bool allowEnter = true){
		textFields[currentPos].text = textFieldsUnder[currentPos].text = fullInput[currentLetter].ToString();
		chosenLetters[currentPos] = currentLetter;
		if (currentLetter < fullInput.Length-1){
			canEnter = true;
		}
		if (advance){
		if (currentPos < textFields.Length-1){
				currentPos++;
				selector.anchoredPosition = selectorPos[currentPos].anchoredPosition;
				currentLetter = fullInput.Length-1;
				SetLetter(false);
            }else if (allowEnter){
				FinishInput();
			}
		}
	}

	void PressDel(bool keyboard = false){

		if (currentPos > 0){
			textFields[currentPos].text = textFieldsUnder[currentPos].text = fullInput[fullInput.Length-1].ToString();
			currentPos--;
			selector.anchoredPosition = selectorPos[currentPos].anchoredPosition;
			currentLetter = chosenLetters[currentPos];
		}else{
			currentLetter = chosenLetters[0] = 0;
		}
        if (keyboard)
        {
            textFields[currentPos].text = fullInput[fullInput.Length - 1].ToString();
            chosenLetters[currentPos] = currentLetter = fullInput.Length - 1;
        }
            
        SetLetter(false);

	}

	void FinishInput(){
		if (playerNameInput){
			playerName = "";
			for (int i = 0; i < textFields.Length; i++){
				playerName += textFields[i].text;
			}
			bool foundLetter = false;
			for (int j = playerName.Length-1; j >= 0; j--){
				if (playerName[j] == ' ' && !foundLetter){
					playerName = playerName.Remove(j, 1);
				}else{
					foundLetter = true;
				}
			}
		}
		myCinema.SetInputComplete();
		inputUI.SetActive(false);
	}

    void UpdateKeysUp(){
        for (int i = 0; i < inputsUp.Length; i++){
            if (!inputsUp[i] && !CheckForLetter(i))
            inputsUp[i] = true;
        }
    }

    bool CheckForLetter(int letterNum)
    {
        bool returnLetterPress = false;
        

        if (letterNum == 0){
            returnLetterPress = Input.GetKey(KeyCode.A);
        }
    if (letterNum == 1)
        {
            returnLetterPress = Input.GetKey(KeyCode.B);
        }
    if (letterNum == 2)
        {
            returnLetterPress = Input.GetKey(KeyCode.C);
        }
        if (letterNum == 3)
        {
            returnLetterPress = Input.GetKey(KeyCode.D);
        }
        if (letterNum == 4)
        {
            returnLetterPress = Input.GetKey(KeyCode.E);
        }
        if (letterNum == 5)
        {
            returnLetterPress = Input.GetKey(KeyCode.F);
        }
        if (letterNum == 6)
        {
            returnLetterPress = Input.GetKey(KeyCode.G);
        }
        if (letterNum == 7)
        {
            returnLetterPress = Input.GetKey(KeyCode.H);
        }
        if (letterNum == 8)
        {
            returnLetterPress = Input.GetKey(KeyCode.I);
        }
        if (letterNum == 9)
        {
            returnLetterPress = Input.GetKey(KeyCode.J);
        }
        if (letterNum == 10)
        {
            returnLetterPress = Input.GetKey(KeyCode.K);
        }
        if (letterNum == 11)
        {
            returnLetterPress = Input.GetKey(KeyCode.L);
        }
        if (letterNum == 12)
        {
            returnLetterPress = Input.GetKey(KeyCode.M);
        }
        if (letterNum == 13)
        {
            returnLetterPress = Input.GetKey(KeyCode.N);
        }
        if (letterNum == 14)
        {
            returnLetterPress = Input.GetKey(KeyCode.O);
        }
        if (letterNum == 15)
        {
            returnLetterPress = Input.GetKey(KeyCode.P);
        }
        if (letterNum == 16)
        {
            returnLetterPress = Input.GetKey(KeyCode.Q);
        }
        if (letterNum == 17)
        {
            returnLetterPress = Input.GetKey(KeyCode.R);
        }
        if (letterNum == 18)
        {
            returnLetterPress = Input.GetKey(KeyCode.S);
        }
        if (letterNum == 19)
        {
            returnLetterPress = Input.GetKey(KeyCode.T);
        }
        if (letterNum == 20)
        {
            returnLetterPress = Input.GetKey(KeyCode.U);
        }
        if (letterNum == 21)
        {
            returnLetterPress = Input.GetKey(KeyCode.V);
        }
        if (letterNum == 22)
        {
            returnLetterPress = Input.GetKey(KeyCode.W);
        }
        if (letterNum == 23)
        {
            returnLetterPress = Input.GetKey(KeyCode.X);
        }
        if (letterNum == 24)
        {
            returnLetterPress = Input.GetKey(KeyCode.Y);
        }
        if (letterNum == 25)
        {
            returnLetterPress = Input.GetKey(KeyCode.Z);
        }
        if (letterNum == 26)
        {
            returnLetterPress = Input.GetKey(KeyCode.Space);
        }
        if (letterNum == 27)
        {
            returnLetterPress = (Input.GetKey(KeyCode.Delete) || Input.GetKey(KeyCode.Backspace));
        }

        return returnLetterPress;
    }

    void CheckForInput(){
        for (int i = 0; i < inputsUp.Length; i++)
        {
            if (inputsUp[i] && CheckForLetter(i)){

                if (i == inputsUp.Length-1){
                    // this is delete key
                    PressDel(true);
                }else{
                    // add key
                    currentLetter = i;
                    SetLetter(true, false);
                }

                inputsUp[i] = false;
            }
        }
    }
}
