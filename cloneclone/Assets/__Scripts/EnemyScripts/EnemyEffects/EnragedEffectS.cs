using UnityEngine;
using System.Collections;

public class EnragedEffectS : MonoBehaviour {

	private EnemyS myEnemy;
	private SpriteRenderer myRender;
	public GameObject[] frustrationParticles;
	private FadeSpriteObjectS[] frustrationFades;
	private AnimObjS[] frustrationAnims;
	private Vector3[] frustrationStartPos;
	public float frustrateSpawnMult = 0.25f;
	public float frustrationLifeTime = 1f;
	public float timeBetweenFrustrations = 0.24f;
	private float[] frustrationSpawnDelays;

	// Use this for initialization
	void Start () {
	
		myEnemy = GetComponentInParent<EnemyS>();
		myEnemy.SetEnragedEffect(this);

		myRender = GetComponent<SpriteRenderer>();
		Color renderCol = myEnemy.bloodColor;
		renderCol.a = myRender.color.a;
		myRender.color = renderCol;

		gameObject.SetActive(false);

		frustrationStartPos = new Vector3[frustrationParticles.Length];
		frustrationSpawnDelays = new float[frustrationParticles.Length];
		frustrationAnims = new AnimObjS[frustrationParticles.Length];
		frustrationFades = new FadeSpriteObjectS[frustrationParticles.Length];
		for (int i = 0; i < frustrationParticles.Length; i++){
			frustrationSpawnDelays[i] = timeBetweenFrustrations*i;
			frustrationStartPos[i] = frustrationParticles[i].transform.localPosition;
			frustrationFades[i] = frustrationParticles[i].GetComponent<FadeSpriteObjectS>();
			frustrationAnims[i] = frustrationParticles[i].GetComponent<AnimObjS>();
			frustrationParticles[i].transform.parent = null;
			frustrationParticles[i].SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		for (int i = 0; i < frustrationParticles.Length; i++){
			frustrationSpawnDelays[i] -= Time.deltaTime;
			if (frustrationSpawnDelays[i] <= 0){
				frustrationSpawnDelays[i] = timeBetweenFrustrations*frustrationParticles.Length+frustrationLifeTime;
				frustrationParticles[i].transform.position = transform.position+frustrationStartPos[i]
					+Random.insideUnitSphere*frustrateSpawnMult;
				frustrationFades[i].Reinitialize();
				frustrationAnims[i].ResetAnimation();
				frustrationParticles[i].SetActive(true);
			}
		}

	}

	public void ActivateEffect(){

		for (int i = 0; i < frustrationParticles.Length; i++){
			frustrationSpawnDelays[i] = timeBetweenFrustrations*i;
			frustrationParticles[i].SetActive(false);
		}
		gameObject.SetActive(true);
		
	}

	public void DeactivateEffect(){
		for (int i = 0; i < frustrationParticles.Length; i++){
			frustrationParticles[i].SetActive(false);
		}
		gameObject.SetActive(false);

	}
}
