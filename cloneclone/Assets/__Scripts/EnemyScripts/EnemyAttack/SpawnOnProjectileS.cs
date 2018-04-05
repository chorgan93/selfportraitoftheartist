using UnityEngine;
using System.Collections;

public class SpawnOnProjectileS : MonoBehaviour {

	public float spawnRate;
	private float spawnRateCountdown;
	public float firstSpawnDelay = 0f;
	public int maxSpawns = -1;
	private bool infiniteSpawn = true;

	[Header("Prefab-based Spawning")]
	public GameObject spawnObject;
	public GameObject[] spawnObjects;
	public bool dontSpawnOnReflect = false;

	[Header("Trail-based Spawning")]
	public bool useSpawnPool = false;
	public Color trailColor = Color.red;
	public float driftSpeed = 0f;
	private EffectSpawnManagerS effectManager;
	public ProjectileS taperBasedOnLifetime;
	public EnemyProjectileS taperBasedOnLifetimeEnemy;
	public BuddyProjectileS taperBasedOnLifetimeBuddy;

	private int currentSpawn = 0;
	[Header("Spawn Positioning")]
	public Vector3 spawnOffset = Vector3.zero;
	public float spawnObjectRadius = 2.5f;
	public float spawnRadiusAdd = 0f;
	public float spawnObjZ = 0f;

	private Vector3 spawnPos;
	private Vector3 spawnPosAdd = Vector3.zero;

	public bool chargeSpawner = false;
	public bool laserSpawn = false;
	public bool enemyChargeSpawner = false;
	private PlayerController playerRef;
	private EnemyS myEnemyRef;
	private EnemyProjectileS myEnemyProj;
	private bool stopSpawningFriendly = false;
	public bool turnOffStun = false;

	public float spawnOnHitEnemyDelay = -1f;
	public bool spawnOnHitEnemies = false;
	private bool spawnedOnHitEnemies = false;
	public GameObject spawnObjectOnHitEnemies;

	private bool firstSpawned = false;

	// Use this for initialization
	void Start () {

		spawnRateCountdown = firstSpawnDelay;
		if (maxSpawns > 0){
			infiniteSpawn = false;
		}
		if (chargeSpawner || spawnOnHitEnemies){
			playerRef = GetComponent<ProjectileS>().myPlayer;
		}

		effectManager = EffectSpawnManagerS.E;

		if (enemyChargeSpawner){
			myEnemyRef = GetComponent<EnemyProjectileS>().myEnemy;
		}

		if (dontSpawnOnReflect){
		myEnemyProj = GetComponent<EnemyProjectileS>();
		}
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (dontSpawnOnReflect){
			if (myEnemyProj.isFriend){
				stopSpawningFriendly = true;
			}
		}
		if ((infiniteSpawn || (!infiniteSpawn && maxSpawns > 0)) && !stopSpawningFriendly){
			spawnRateCountdown -= Time.deltaTime;
			if (spawnRateCountdown <= 0){
				spawnRateCountdown = spawnRate;
				if (!infiniteSpawn){
					maxSpawns --;
				}

				spawnPos = transform.position+spawnOffset;

				spawnPosAdd = Random.insideUnitSphere*spawnObjectRadius;

				if (taperBasedOnLifetime){
					spawnPosAdd *= 1-(taperBasedOnLifetime.rangeRef/taperBasedOnLifetime.range);
				}
				if (taperBasedOnLifetimeEnemy){
					spawnPosAdd *= 1-(taperBasedOnLifetimeEnemy.range/taperBasedOnLifetimeEnemy.maxRange);
				}
				if (taperBasedOnLifetimeBuddy){
					spawnPosAdd *= 1-(taperBasedOnLifetimeBuddy.range/taperBasedOnLifetimeBuddy.maxRange);
				}

				spawnPos += spawnPosAdd;
				spawnObjectRadius+=spawnRadiusAdd;

				spawnPos.z = spawnObjZ;
				GameObject newSpawn;

				if (useSpawnPool){
					newSpawn = effectManager.SpawnProjectileFade(spawnPos, trailColor, transform.rotation, driftSpeed);
				}
				else if (spawnObject != null){
					newSpawn = Instantiate(spawnObject, spawnPos, spawnObject.transform.rotation)
						as GameObject;
				}else{
					newSpawn = Instantiate(spawnObjects[currentSpawn], spawnPos, spawnObjects[currentSpawn].transform.rotation)
						as GameObject;
					currentSpawn++;
				}

				if (enemyChargeSpawner){
					newSpawn.GetComponentInChildren<EnemyChargeAttackS>().SetEnemy(myEnemyRef);
				}

				if (chargeSpawner){
					newSpawn.GetComponent<ChargeAttackS>().SetPlayer(playerRef);
					if (!firstSpawned){
						newSpawn.GetComponent<ChargeAttackS>().SetFirstSpawned();
					}
					if (turnOffStun){
						newSpawn.GetComponent<ChargeAttackS>().TurnOffStun();
					}

					if (laserSpawn){
						Vector3 targetPoint = playerRef.transform.position+(transform.position-playerRef.transform.position).normalized*(spawnObjectRadius+1f);
						newSpawn.transform.position = playerRef.transform.position+(transform.position-playerRef.transform.position).normalized*spawnObjectRadius+Random.insideUnitSphere*spawnRadiusAdd;
						newSpawn.transform.Rotate(new Vector3(0,0, LaserFace(targetPoint-newSpawn.transform.position)-90f));
					}
				}
				firstSpawned = true;
			}
		}

		if (spawnOnHitEnemies && !spawnedOnHitEnemies){
			spawnOnHitEnemyDelay -= Time.deltaTime;
			if (spawnOnHitEnemyDelay <= 0){
			if (playerRef){
				for (int i = 0; i < playerRef.enemiesHitByAttackRef.Count; i++){
					spawnPos = playerRef.enemiesHitByAttackRef[i].transform.position;
					spawnPos += Random.insideUnitSphere*spawnObjectRadius;
					spawnObjectRadius+=spawnRadiusAdd;
					spawnPos.z = spawnObjZ;
						GameObject newSpawn = Instantiate(spawnObjectOnHitEnemies, spawnPos, spawnObjectOnHitEnemies.transform.rotation)
						as GameObject;
					
					if (chargeSpawner){
						newSpawn.GetComponent<ChargeAttackS>().SetPlayer(playerRef);
						if (!firstSpawned){
							newSpawn.GetComponent<ChargeAttackS>().SetFirstSpawned();
						}
						if (turnOffStun){
							newSpawn.GetComponent<ChargeAttackS>().TurnOffStun();
						}

							if (laserSpawn){
								Vector3 targetPoint = playerRef.transform.position+(transform.position-playerRef.transform.position).normalized*(spawnObjectRadius+1f);
								newSpawn.transform.position = playerRef.transform.position+(transform.position-playerRef.transform.position).normalized*spawnObjectRadius+Random.insideUnitSphere*spawnRadiusAdd;
								newSpawn.transform.Rotate(new Vector3(0,0, LaserFace(targetPoint-newSpawn.transform.position)-90f));
							}
					}
				}
			}
				spawnedOnHitEnemies = true;
			}
		}
	
	}

	private float LaserFace(Vector3 direction){

		float rotateZ = 0;

		Vector3 targetDir = direction.normalized;

		if(targetDir.x == 0){
			if (targetDir.y > 0){
				rotateZ = 90;
			}
			else{
				rotateZ = -90;
			}
		}
		else{
			rotateZ = Mathf.Rad2Deg*Mathf.Atan((targetDir.y/targetDir.x));
		}	


		if (targetDir.x < 0){
			rotateZ += 180;
		}



		return rotateZ;

	}							

	public void SetNewParticleColor(Color newCol){
		Color switchCol = newCol;
		switchCol.a = trailColor.a;
		trailColor = switchCol;
	}
}
