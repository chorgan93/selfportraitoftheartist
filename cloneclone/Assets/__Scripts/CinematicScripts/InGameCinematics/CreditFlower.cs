using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditFlower : MonoBehaviour {

    public float startDistance = 50;
    private Vector3 targetPos;
    private Vector3 driftDir;
    public float targetPosXMin = 0.1f;
    public float targetPosXMax = 0.3f;
    public float targetPosYMin = 0.45f;
    public float targetPosYMax = 0.8f;

    public float driftRate = 1f;
    public float rotateRateMin = 1f;
    public float rotateRateMax = 3f;

    private Vector3 rotateAngle = new Vector3(0, 0, 1f);

    public CreditsManagerS myCredits;
    private bool stopDrifting = false;

    public float delayDrift = 10f;
    public float triggerEndDrift = 120f;
    public float endDriftAccMin = 1f;
    public float endDriftAccMax = 2f;
    private float endDriftAccel;
    private float endDriftRate = 0f;
    public float endDriftAccelTime = 1f;
    private Vector3 endDriftDir = Vector3.zero;

    // end drift stuff

	// Use this for initialization
	void Start () {
        targetPos = Random.insideUnitSphere;
        targetPos.z = transform.localPosition.z;

        targetPos.x *= Random.Range(targetPosXMin, targetPosXMax);
        targetPos.y *= Random.Range(targetPosYMin, targetPosYMax);

        Vector3 startPos = targetPos;
        startPos.z = transform.localPosition.z;
        Vector3 fixNormal = startPos;
        fixNormal.z = 0f;
        startPos += fixNormal.normalized * startDistance;
        transform.localPosition = startPos;

        driftDir = (targetPos - transform.localPosition).normalized;
        driftDir.z = 0f;

        transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));

        rotateAngle.z *= Random.Range(rotateRateMin, rotateRateMax) * Random.insideUnitCircle.x;

        endDriftDir = Random.insideUnitSphere;
        endDriftDir.z = 0f;
        endDriftDir = endDriftDir.normalized;
        endDriftAccel = Random.Range(endDriftAccMin, endDriftAccMax);
	}
	
	// Update is called once per frame
	void Update () {

        if (triggerEndDrift > 0)
        {
            if (myCredits.fastForwarding)
            {
                triggerEndDrift -= Time.deltaTime * myCredits.fastForwardMult;
            }
            else
            {
                triggerEndDrift -= Time.deltaTime;
            }
        }

        if (delayDrift > 0) {
            if (myCredits.fastForwarding)
            {
                delayDrift -= Time.deltaTime * myCredits.fastForwardMult;
            }
            else
            {
                delayDrift -= Time.deltaTime;
            }

        }
        else
        {
            if (!stopDrifting)
            {
                DoDrift();
            }
            if (triggerEndDrift <= 0){
                DoEndDrift();
            }
        }
        AmbientRotate();

	}

    void DoDrift(){

        if (myCredits.fastForwarding)
        {
            transform.localPosition += driftDir * driftRate * Time.deltaTime * myCredits.fastForwardMult;
            startDistance -= driftRate * Time.deltaTime * myCredits.fastForwardMult;
        }
        else
        {
            transform.localPosition += driftDir * driftRate * Time.deltaTime;
            startDistance -= driftRate * Time.deltaTime;
        }

        if (startDistance <= 0){
            transform.localPosition = targetPos;
            stopDrifting = true;
        }

    }

    void DoEndDrift(){

        if (endDriftAccelTime > 0)
        {
            endDriftRate += endDriftAccel * Time.deltaTime;
            endDriftAccelTime -= Time.deltaTime;
        }
        transform.position += endDriftRate * endDriftDir * Time.deltaTime;
        
    }

    void AmbientRotate(){
        transform.Rotate(rotateAngle * Time.deltaTime);
    }
}
