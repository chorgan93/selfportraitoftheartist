using UnityEngine;
using System.Collections;

public class ForceCurrencyUpdateS : MonoBehaviour {

	public int currencyToAdd = 2500;

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){

			PlayerCurrencyDisplayS cDisplay = GameObject.Find("SinBorder").GetComponent<PlayerCurrencyDisplayS>();
			cDisplay.AddCurrency(currencyToAdd);
			enabled = false;
		}
	}
}
