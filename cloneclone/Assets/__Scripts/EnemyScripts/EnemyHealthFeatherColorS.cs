using UnityEngine;
using System.Collections;

public class EnemyHealthFeatherColorS : MonoBehaviour {

	private SpriteRenderer myRenderer;
	private EnemyS myEnemy;
	private Color currentColor;
	private float ambientAlpha = 0.8f;

	private float maxWhiteTime = 0.12f;
	private float currentWhiteTime = 0f;
	private bool flashing = false;

	public Transform partnerFeather;
	private Vector3 partnerLocalPos;
	private Vector3 startPos;
	private Quaternion partnerRotation;
	private Quaternion startRot;
	private Vector3 partnerLocalScale;
	private Vector3 startScale;

	private bool inOppPos;

	private bool _initialized;

	// Use this for initialization
	void Start () {

		Initialize();

	}
	
	// Update is called once per frame
	void Update () {

		if (flashing){
			currentWhiteTime -= Time.deltaTime;
			if (currentWhiteTime <= 0){
				myRenderer.color = currentColor;
				flashing = false;
			}
		}

		if (myEnemy){
			if (!myEnemy.isDead){
				if (myEnemy.transform.localScale.x < 0 && !inOppPos){
					transform.localPosition = partnerLocalPos;
					transform.localRotation = partnerRotation;
					transform.localScale = partnerLocalScale;
					inOppPos = true;
				}else{
					if (myEnemy.transform.localScale.x > 0 && inOppPos){
						transform.localPosition = startPos;
						transform.localRotation = startRot;
						transform.localScale = startScale;
						inOppPos = false;
					}
				}
			}
		}
	
	}

	void Initialize(){
		if (!_initialized){
			
			myRenderer = GetComponent<SpriteRenderer>();
			_initialized = true;

			partnerLocalPos = partnerFeather.localPosition;
			partnerRotation = partnerFeather.localRotation;
			partnerLocalScale = partnerFeather.localScale;
			startPos = transform.localPosition;
			startRot = transform.localRotation;
			startScale = transform.localScale;

			myEnemy = GetComponentInParent<EnemyHealthFeathersS>().enemyRef;

		}
	}

	public void SetUpFeather(Color newCol){
		Initialize();
		currentColor = newCol;
		currentColor.a = ambientAlpha;
		FlashWhite();
	}

	public void FlashWhite(bool extra = false){
		myRenderer.color = Color.white;
		currentWhiteTime = maxWhiteTime;
		if (extra){
			currentWhiteTime *= 1.6f;
		}
		flashing = true;
	}

	public void ChangeCurrentColor(Color newC){
		currentColor = newC;
		currentColor.a = ambientAlpha;
		if (!flashing){
			myRenderer.color = currentColor;
		}
	}
}
