using UnityEngine;
using System.Collections;

public class DamageNumberS : MonoBehaviour {

	private TextMesh myRenderer;
	public TextMesh bgRenderer;

	private Color myColor;
	private Color bgColor;

	public Color playerColor;
	public Color enemyColor;
	public Color playerHitColor;

	private bool _isEnemy = false;

	public Vector3 moveRate = new Vector3(0,2f,0);
	public float moveInFadeMult = 0.4f;
	public float delayFadeTime = 1f;
	private float delayFadeCount;

	public float fadeRate = 2f;

	private bool _initialized = false;

	private EffectSpawnManagerS myPool;
	
	// Update is called once per frame
	void Update () {


		if (delayFadeCount > 0){
			delayFadeCount -= Time.deltaTime;
			transform.position += moveRate*Time.deltaTime;
		}else{
			transform.position += moveRate*Time.deltaTime*moveInFadeMult;
			myColor.a -= fadeRate*Time.deltaTime;
			if (myColor.a <= 0){
				myColor.a = 0;
				myRenderer.text = "";
				bgRenderer.text = "";
				myPool.Despawn(gameObject, 3);
				gameObject.SetActive(false);
			}
			bgColor.a = myColor.a;
			myRenderer.color = myColor;
			bgRenderer.color = bgColor;
		}
	
	}

	public void Initialize(bool isEnemy, float dmgNum, bool useHitColor = false){
		if (!_initialized){
			myRenderer = GetComponent<TextMesh>();
			myRenderer.text = "";
			bgRenderer.text = "";
			myColor = myRenderer.color;
			bgColor = bgRenderer.color;
			_initialized = true;
		}
		if (!myPool){
			SetPool(EffectSpawnManagerS.E);
		}

		_isEnemy = isEnemy;
		if (useHitColor){
			//myColor = playerHitColor;
			myColor = enemyColor;
			bgColor = playerColor;
			myRenderer.text = Mathf.RoundToInt(dmgNum*10f).ToString();
		}
		else if (_isEnemy){
			myColor = enemyColor;
			bgColor = Color.black;
			myRenderer.text = Mathf.RoundToInt(dmgNum*100f).ToString();
		}
		else{
			myColor = enemyColor;
			bgColor = playerColor;
			myRenderer.text = Mathf.RoundToInt(dmgNum*100f).ToString();
		}
		bgRenderer.text = myRenderer.text;
		myColor.a = bgColor.a = 1f;
		myRenderer.color = myColor;
		bgRenderer.color = bgColor;
		delayFadeCount = delayFadeTime;
	}

	public void SetPool(EffectSpawnManagerS newPool){
		myPool = newPool;
	}
}
