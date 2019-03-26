using UnityEngine;
using System.Collections;

public class ExamineLabelS : MonoBehaviour {

	private PlayerController myRef;
	private TextMesh myMesh;
    public TextMesh myOutline;
	private string startString;


	public SpriteRenderer examineButtonSprite;
	public SpriteRenderer examineButtonSpritePS4;
	public SpriteRenderer examineKeySprite;

	private bool buttonSet = false;
	private Vector3 floatPos;
	private Vector3 wanderPos;
	private float wanderMultX = 0.2f;
	private float wanderMultY = 0.4f;
	private float wanderSpeed = 0.05f;
	private float wanderCount;
	private float wanderChangeMin = 0.5f;
	private float wanderChangeMax = 1f;

	private float buttonSetDelay = 0.04f;
	private float currentButtonSet;

	private bool dontShow = false;

	// Use this for initialization
	void Start () {

		myRef = GetComponentInParent<PlayerController>();
		myMesh = GetComponent<TextMesh>();
        startString = LocalizationManager.instance.GetLocalizedValue(myMesh.text);
		myMesh.text = "";

        if (myOutline){
            myOutline.text = "";
        }

		examineButtonSprite.gameObject.SetActive(false);
		examineButtonSpritePS4.gameObject.SetActive(false);
		examineKeySprite.gameObject.SetActive(false);
		buttonSet = false;

		currentButtonSet = buttonSetDelay;
		dontShow = PlayerStatDisplayS.RECORD_MODE;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (myRef.examining && !myRef.talking && !dontShow){

			if (!buttonSet){


				if (currentButtonSet > 0f){
					currentButtonSet -= Time.deltaTime;
				}else{

					wanderPos = Random.insideUnitSphere;
					wanderPos.z = 0f;
					wanderPos.x *= wanderMultX;
					wanderPos.y *= wanderMultY;
					wanderCount = Random.Range(wanderChangeMin, wanderChangeMax);
					floatPos = Vector3.zero;
					transform.localPosition = myRef.examineStringPos+floatPos;

					if (ControlManagerS.controlProfile == 3){
						examineButtonSpritePS4.gameObject.SetActive(true);
					}
					else if (myRef.myControl.ControllerAttached() && ControlManagerS.controlProfile == 0){
					examineButtonSprite.gameObject.SetActive(true);
				}else{
					examineKeySprite.gameObject.SetActive(true);
				}
			
				if (myRef.overrideExamineString != ""){
					if (myRef.overrideExamineString.Contains("A Button")){ 
						myMesh.text = myRef.overrideExamineString.Replace("A Button", "");
                            if (myOutline){
                                myOutline.text = myMesh.text;
                            }
					}
					else if (myRef.overrideExamineString.Contains("E Key")){ 
						myMesh.text = myRef.overrideExamineString.Replace("E Key", "");
                            if (myOutline)
                            {
                                myOutline.text = myMesh.text;
                            }
                        }else{
                            myMesh.text = myRef.overrideExamineString;
                            if (myOutline)
                            {
                                myOutline.text = myMesh.text;
                            }
                        }
				}else{
					myMesh.text = startString;
                        if (myOutline)
                        {
                            myOutline.text = myMesh.text;
                        }
				}
				buttonSet = true;
				}
			}
			Float();

		}else{
			if (buttonSet || currentButtonSet < buttonSetDelay){
				buttonSet = false;
				examineButtonSprite.gameObject.SetActive(false);
				examineButtonSpritePS4.gameObject.SetActive(false);
				examineKeySprite.gameObject.SetActive(false);
				myMesh.text = "";
                if (myOutline)
                {
                    myOutline.text = myMesh.text;
                }
				floatPos = Vector3.zero;
				currentButtonSet = buttonSetDelay;
			}
		}
	
	}

	void Float(){
		wanderCount -= Time.deltaTime;

		if (wanderCount <= 0){
			wanderPos = Random.insideUnitSphere;
			wanderPos.z = 0f;
			wanderPos.x *= wanderMultX;
			wanderPos.y *= wanderMultY;
			wanderCount = Random.Range(wanderChangeMin, wanderChangeMax);
		}

		floatPos += (wanderPos-floatPos).normalized*wanderSpeed*Time.deltaTime;

		transform.localPosition = myRef.examineStringPos+floatPos;

	}
}
