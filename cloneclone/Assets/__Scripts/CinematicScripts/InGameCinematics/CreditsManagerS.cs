using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsManagerS : MonoBehaviour
{

    public static int currentEnding = 2;
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

    private float failsafeTime = 180f;

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

    [Header("Descent Credits")]
    public bool isDescentCredits = false;
    public string endingDNextScene = "";
    public int skipEndingDSceneAtWeaponNum;
    public string skipEndingDScene = "";

    private void Start()
    {
        endCreditMoveY = creditDoneTransform.position.y;
        startScrollRate = scrollRate;

        if (isDescentCredits)
        {
            delayEndCountdown = timeAfterEnd[0];
            for (int i = 0; i < creditArtToUse.Length; i++)
            {
                creditArtToUse[i].SetActive(false);
            }
        }
        else
        {
            creditArtToUse[currentEnding].SetActive(true);
            delayEndCountdown = timeAfterEnd[currentEnding];
        }
    }


    // Update is called once per frame
    void Update()
    {

        if (!creditsFinished)
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.E))
            {
                scrollRate = fastForwardRate;
                fastForwarding = true;
            }
            else
            {
                scrollRate = startScrollRate;
                fastForwarding = false;
            }
#else
            scrollRate = startScrollRate;
                fastForwarding = false;
#endif
            if (!checkForEnd)
            {
                if (fastForwarding)
                {
                    failsafeTime -= Time.deltaTime * fastForwardMult;
                }
                else {
                    failsafeTime -= Time.deltaTime;
                }
                if (failsafeTime <= 0) {
                    checkForEnd = true;
                }
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
        if (isDescentCredits){
            if (!PlayerInventoryS.I.CheckForWeaponNum(skipEndingDSceneAtWeaponNum))
            {
                returnString = endingDNextScene;
            }else{
                returnString = skipEndingDScene;
            }
        }
        else if (currentEnding == 0){
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
