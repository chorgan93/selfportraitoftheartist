using UnityEngine;
using System.Collections;

public class DepthsLineS : MonoBehaviour {

	public EnemySpawnerS targetEnemy;
	private EnemyS myEnemy;
	private bool enemyAlive = true;
	public Color aliveColor;
	public Color deadColor;
	public Sprite[] animSprites;
	private SpriteRenderer mySprite;
	private int currentSprite;
	public float animAlive = 0.08f;
	public float animDead = 0.6f;
	private float currentAnimCount = 0f;
	public Vector3 offsetPos = new Vector3(0f,15f,1f);

	// Use this for initialization
	void Start () {
	
		mySprite = GetComponent<SpriteRenderer>();
		mySprite.sprite = animSprites[currentSprite];
		if (targetEnemy.currentSpawnedEnemy){
			myEnemy = targetEnemy.currentSpawnedEnemy;
			transform.position = myEnemy.transform.position+offsetPos;
			if (enemyAlive){
				mySprite.color = aliveColor;
			}else{
				mySprite.color = deadColor;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (!myEnemy){
			if (targetEnemy.currentSpawnedEnemy){
			myEnemy = targetEnemy.currentSpawnedEnemy;
				transform.position = myEnemy.transform.position+offsetPos;
				enemyAlive = !myEnemy.isDead;
				if (enemyAlive){
				mySprite.color = aliveColor;
				}else{
					mySprite.color = deadColor;
				}
			}
		}else{
			if (enemyAlive){
				if (myEnemy.isDead){
					enemyAlive = false;
					mySprite.color = deadColor;
				}
			}else{
				if (!myEnemy.isDead){
					enemyAlive = true;
					mySprite.color = aliveColor;
				}
			}

			AnimateLine();
			transform.position = myEnemy.transform.position+offsetPos;
		}
	
	}

	void AnimateLine(){
		currentAnimCount+=Time.deltaTime;
		if ((enemyAlive && currentAnimCount >= animAlive) || (!enemyAlive && currentAnimCount >= animDead)){
			currentAnimCount = 0f;
			currentSprite++;
			if (currentSprite > animSprites.Length-1){
				currentSprite = 0;
			}
			mySprite.sprite = animSprites[currentSprite];
		}
	}
}
