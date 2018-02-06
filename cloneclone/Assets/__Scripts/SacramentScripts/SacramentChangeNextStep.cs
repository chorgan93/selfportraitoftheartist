using UnityEngine;
using System.Collections;

public class SacramentChangeNextStep : MonoBehaviour {

	public SacramentStepS targetStep;
	public SacramentStepS newNextStep;

	public void Activate(){

		targetStep.nextStep = newNextStep;

	}
}
