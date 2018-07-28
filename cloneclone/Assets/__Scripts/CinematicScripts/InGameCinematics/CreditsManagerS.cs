using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsManagerS : MonoBehaviour
{

    public static int currentEnding = 1;
    public GameObject[] creditArtToUse;
    public float[] timeAfterEnd;

    private float delayEndCountdown;

    private bool _creditsFinished = false;
    public bool creditsFinished { get { return _creditsFinished; } }

    public Transform[] checkTransforms;
    private float endCreditMoveY;
    public Transform creditDoneTransform;
    int numOfFinishedTransforms = 0;

    public float scrollRate = 2f;
    private float startScrollRate;
    private float fastForwardRate = 10f;
    public float fastForwardMult { get { return fastForwardRate / startScrollRate; } }
    public Vector3 scrollDir = new Vector3(0, 1f, 0);

    [HideInInspector]
    public bool fastForwarding = false;

    private bool checkForEnd = false;

    public GameObject turnOnOnEnd; // for turning off music

    [Header("Next Scene Logic")]
    public string endingANextScene = "";
    public int skipEndingASceneAtWeaponNum;
    public string endingBNextScene = "";
    public int skipEndingBSceneAtWeaponNum;
    public string endingCNextScene = "";
    public int skipEndingCSceneAtVirtueNum;

	private void Start()
	{
        endCreditMoveY = creditDoneTransform.position.y;
        startScrollRate = scrollRate;

        creditArtToUse[currentEnding].SetActive(true);
        delayEndCountdown = timeAfterEnd[currentEnding];
	}


	// Update is called once per frame
	void Update () {

        if (!creditsFinished)
        {
            if (Input.GetKey(KeyCode.E))
            {
                scrollRate = fastForwardRate;
                fastForwarding = true;
            }
            else{
                scrollRate = startScrollRate;
                fastForwarding = false;
            }
            if (!checkForEnd)
            {
                for (int i = numOfFinishedTransforms; i < checkTransforms.Length; i++)
                {
                    if (checkTransforms[i].position.y >= endCreditMoveY)
                    {
                        numOfFinishedTransforms++;
                        checkTransforms[i].parent.gameObject.SetActive(false);
                        if (numOfFinishedTransforms >= checkTransforms.Length)
                        {
                            checkForEnd = true;
                        }
                    }
                    else
                    {
                        checkTransforms[i].parent.position += scrollDir * scrollRate * Time.deltaTime;
                    }
                }
            }else{
                if (fastForwarding){
                    delayEndCountdown -= Time.deltaTime * fastForwardMult;
                }else{
                    delayEndCountdown -= Time.deltaTime;
                }
                if (delayEndCountdown <= 0f){
                    _creditsFinished = true;
                    turnOnOnEnd.SetActive(true);
                }
            }
        }
	}

    public string GetNextScene(){
        string returnString = "";
        if (currentEnding == 0){
            if (!PlayerInventoryS.I.CheckForWeaponNum(skipEndingASceneAtWeaponNum))
            {
                returnString = endingANextScene;
            }
        }else if (currentEnding == 1){
            if (!PlayerInventoryS.I.CheckForWeaponNum(skipEndingBSceneAtWeaponNum))
            {
                returnString = endingBNextScene;
            }
        }else if (currentEnding == 2){
            if (!PlayerInventoryS.I._earnedVirtues.Contains(skipEndingCSceneAtVirtueNum)){
                returnString = endingCNextScene;
            }
        }
        return returnString;
    }
}
