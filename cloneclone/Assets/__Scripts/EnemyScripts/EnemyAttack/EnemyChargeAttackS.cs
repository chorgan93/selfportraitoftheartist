using UnityEngine;
using System.Collections;

public class EnemyChargeAttackS : MonoBehaviour
{

    private EnemyS myEnemy;
    public EnemyS enemyReference { get { return myEnemy; } }
    public Renderer _myRenderer;
    private Collider _myCollider;
    private float _animateRate = 0.033f;
    private float animateCountdown;
    private Vector2 startTiling;
    private float tilingRandomMult = 0.5f;

    private float visibleTime = 0.4f;
    private float startAlpha = 1f;
    private float fadeRate;
    private Color fadeColor;

    private float capturedChargeTime;
    public float chargeUpTime;
    public float maxChargeAlpha;
    private bool charging = false;

    public Texture startFlash;
    private Texture startTexture;
    private int flashFrames = 3;
    private int colliderFrames = 6;
    private int flashMax = 3;
    private int blackFlashFrames = 4;

    public GameObject hitObj;
    public GameObject soundObj;

    [Header("Attack Properties")]
    public float knockbackForce = 1000f;
    public float dmg = 5f;
    private Vector3 knockBackDir;
    public float knockbackTime = 0.8f;
    public float offsetChargeRange = 0f;
    private Vector3 chargeStartPos;
    private Vector3 renderStartPos;
    private Vector3 chargeOffsetPos;

    public bool standalone = false;
    public bool shakeOverride = false;
    public float standaloneTurnOnTime = 0.6f;

    [Header("Spawn Properties")]
    public GameObject spawnOnCharge;
    private Vector3 spawnPos;
    public Vector3 spawnOffset;
    public float spawnZ = 3f;
    public float spawnRadius;
    public int numToSpawn = 1;

    private bool doKill = false;
    [Header("Friendly Properties")]
    public bool isFriendly = false;

    [Header("Corrupt Properties")]
    public bool corruptEnemies = false;
    public ChargeRiseEffectS riseEffect;

    private bool _dontDealDamage = false;
    public bool dontDealDamage { get { return _dontDealDamage; } }

    [Header("Fos Charge Properties")]
    public bool isFosCharge = false;
    private bool fosFired = false;
    private float floatRadius = 3f;
    public float fosVisibleTime = 6f;
    public float shootLifetime = 3f;
    public float shootSpeed = 1000f;
    private Rigidbody _myRigid;
    private Vector3 currentFosPos = Vector3.zero;
    private float floatAxisA = 3f;
    private float floatAxisB = 1.75f;
    private float currentAngle = 0f;
    private float angleRate = 8f;
    private int maxHits = 1;
    private int currentHit = 0;
    private float startRotate = -1;
    private float fosFadeRate = 2f;
    private float fosFiredFade = 5f;
    public float punchMult = 1f;
    public float slowTime = 0.2f;
    public float hangTime = 0.3f;
    private Vector3 savedFireDirection = Vector3.zero;


    // Use this for initialization
    void Start()
    {

        //_myRenderer = GetComponent<Renderer>();
        _myCollider = GetComponent<Collider>();
        animateCountdown = _animateRate;

        if (PlayerController.equippedTech.Contains(14)){
            dmg *= 0.5f;
        }

        if (!standalone)
        {
            myEnemy = GetComponentInParent<EnemyS>();
            chargeStartPos = transform.localPosition;
            renderStartPos = _myRenderer.transform.localPosition;
            if (myEnemy)
            {
                isFriendly = myEnemy.isFriendly;
            }
        }

        startTiling = _myRenderer.material.GetTextureScale("_MainTex");
        startTexture = _myRenderer.material.GetTexture("_MainTex");


        if (isFosCharge)
        {
            visibleTime = fosVisibleTime;
            startRotate = transform.rotation.eulerAngles.z;
            fadeRate = fosFadeRate;
            _myRigid = GetComponent<Rigidbody>();
            myEnemy.AddFosCharge(this);
        }
        else
        {
            fadeRate = startAlpha / visibleTime;
        }

        fadeColor = _myRenderer.material.color;
        if (!standalone)
        {
            _myRenderer.enabled = false;
        }
        else
        {
            TurnOn(standaloneTurnOnTime);
        }


    }

    // Update is called once per frame
    void Update()
    {


        if (isFosCharge)
        {
            FosMovement();
        }

        if (_myRenderer.enabled)
        {

            if (charging)
            {
                chargeUpTime -= Time.deltaTime;
                fadeColor.a = maxChargeAlpha * (1f - chargeUpTime / capturedChargeTime);
                _myRenderer.material.color = fadeColor;
                if (chargeUpTime <= 0)
                {
                    TriggerAttack();
                }
            }
            else
            {

                flashFrames--;
                if (flashFrames < 0)
                {

                    if (_myCollider.enabled && flashFrames < -colliderFrames && (!isFosCharge || (isFosCharge && currentHit >= maxHits)))
                    {
                        _myCollider.enabled = false;
                        if (isFosCharge && fosFired)
                        {
                            _myRigid.velocity = Vector3.zero;
                            visibleTime = 0f;
                        }
                        if (isFosCharge)
                        {
                            myEnemy.RemoveFosCharge(this);
                        }
                    }

                    if (_myRenderer.material.GetTexture("_MainTex") == startFlash)
                    {
                        if (_myRenderer.material.color == Color.black)
                        {
                            _myRenderer.material.color = Color.white;
                            flashFrames = blackFlashFrames;
                        }
                        else
                        {
                            _myRenderer.material.SetTexture("_MainTex", startTexture);
                            if (myEnemy)
                            {
                                if (!myEnemy.isCritical && !myEnemy.isDead)
                                {
                                    _myCollider.enabled = true;
                                    if (doKill)
                                    {
                                        myEnemy.KillWithoutXP();
                                    }
                                }
                                else
                                {
                                    _myRenderer.enabled = false;
                                }
                            }
                            else if (!myEnemy)
                            {
                                _myCollider.enabled = true;
                            }
                            SpawnObjects();
                        }
                    }
                    else
                    {
                        if (!isFosCharge || (isFosCharge && visibleTime <= 0))
                        {
                            fadeColor.a -= Time.deltaTime * fadeRate;
                        }

                        if (fadeColor.a <= 0)
                        {
                            fadeColor.a = 0;
                            _myRenderer.enabled = false;
                            _myCollider.enabled = false;
                            if (standalone)
                            {
                                Destroy(transform.parent.gameObject);
                            }
                        }
                        else
                        {
                            _myRenderer.material.color = fadeColor;
                        }
                        animateCountdown -= Time.deltaTime;
                        if (animateCountdown <= 0)
                        {
                            animateCountdown = _animateRate;
                            Vector2 newTiling = startTiling;
                            newTiling.x += Random.insideUnitCircle.x * tilingRandomMult;
                            newTiling.y += Random.insideUnitCircle.y * tilingRandomMult;
                            _myRenderer.material.SetTextureScale("_MainTex", newTiling);
                        }
                    }
                }
            }
        }

    }

    public void TriggerAttack()
    {

        fadeColor = _myRenderer.material.color;
        fadeColor.a = startAlpha;
        _dontDealDamage = false;
        _myRenderer.material.color = Color.black;

        _myRenderer.material.SetTexture("_MainTex", startFlash);
        _myRenderer.enabled = true;
        flashFrames = flashMax;

        if (!standalone && !shakeOverride)
        {
            CameraShakeS.C.TimeSleep(0.06f);
        }
        if (soundObj)
        {
            Instantiate(soundObj);
        }
        if (shakeOverride)
        {
            CameraShakeS.C.LargeShake();
        }
        else
        {
            CameraShakeS.C.SmallShake();
        }

        charging = false;

        if (riseEffect)
        {
            riseEffect.TriggerEffect(Vector3.zero);
        }
    }

    private float FaceDirection(Vector3 direction)
    {

        float rotateZ = 0;

        Vector3 targetDir = direction.normalized;

        if (targetDir.x == 0)
        {
            if (targetDir.y > 0)
            {
                rotateZ = 90;
            }
            else
            {
                rotateZ = -90;
            }
        }
        else
        {
            rotateZ = Mathf.Rad2Deg * Mathf.Atan((targetDir.y / targetDir.x));
        }


        if (targetDir.x < 0)
        {
            rotateZ += 180;
        }

        rotateZ += -90f + 25f * Random.insideUnitCircle.x;

        return rotateZ;
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player" && !isFriendly && !corruptEnemies && !_dontDealDamage)
        {

            knockBackDir = (other.transform.position - transform.position).normalized;
            knockBackDir.z = 1f;

            float actingDamage = dmg * Random.Range(1f - EnemyS.DAMAGE_VARIANCE, 1f + EnemyS.DAMAGE_VARIANCE);

            if (myEnemy)
            {
                actingDamage *= myEnemy.CorruptedPowerMult();
                other.gameObject.GetComponent<PlayerController>().myStats.TakeDamage
                    (myEnemy, actingDamage, knockBackDir * knockbackForce * Time.deltaTime, knockbackTime);
                _dontDealDamage = true;
            }
            else
            {
                other.gameObject.GetComponent<PlayerController>().myStats.TakeDamage
                (null, actingDamage, knockBackDir * knockbackForce * Time.deltaTime, knockbackTime);
                _dontDealDamage = true;
            }

            //HitEffect(other.transform.position, other.gameObject.GetComponent<EnemyS>().bloodColor);
        }

        if (other.gameObject.tag == "Enemy")
        {


            EnemyS hitEnemy = other.gameObject.GetComponent<EnemyS>();

            if (hitEnemy != null && !myEnemy != null)
            {
                if (hitEnemy != myEnemy && !hitEnemy.isDead
                        && ((hitEnemy.isFriendly != isFriendly) || (hitEnemy.isFriendly == isFriendly && corruptEnemies)))
                {

                    if (hitEnemy.isFriendly == isFriendly && corruptEnemies)
                    {
                        myEnemy.myStatusMessenger.AddBuffedEnemy(hitEnemy);
                    }
                    else
                    {
                        knockBackDir = (other.transform.position - transform.position).normalized;
                        knockBackDir.z = 1f;

                        float actingDamage = dmg * Random.Range(1f - EnemyS.DAMAGE_VARIANCE, 1f + EnemyS.DAMAGE_VARIANCE);


                        hitEnemy.TakeDamage
                            (other.transform, knockBackDir * knockbackForce * Time.deltaTime, actingDamage, 1f, 1f, 0, 0f, knockbackTime, true);

                    }
                }
            }

            //HitEffect(other.transform.position, other.gameObject.GetComponent<EnemyS>().bloodColor);
        }


    }

    /*void HitEffect(Vector3 spawnPos, Color bloodCol){

		Vector3 hitObjSpawn = spawnPos;
		GameObject newHitObj = Instantiate(hitObj, hitObjSpawn, Quaternion.identity)
			as GameObject;
		
		newHitObj.transform.Rotate(new Vector3(0,0,FaceDirection((spawnPos-transform.position).normalized)));
		
		SpriteRenderer hitRender = newHitObj.GetComponent<SpriteRenderer>();
		hitRender.color = bloodCol;

		newHitObj.transform.localScale = Vector3.one*10f;

		
		hitObjSpawn += newHitObj.transform.up*Mathf.Abs(newHitObj.transform.localScale.x)/2f;
		newHitObj.transform.position = hitObjSpawn;
	}*/

    public void TurnOn(float attackWarmup, bool killOnCast = false)
    {

        if (!standalone)
        {
            chargeOffsetPos = offsetChargeRange * Random.insideUnitSphere;
            chargeOffsetPos.z = 0f;
            transform.localPosition = chargeOffsetPos + chargeStartPos;
            _myRenderer.transform.localPosition = renderStartPos + chargeOffsetPos;
        }
        doKill = killOnCast;
        _dontDealDamage = false;
        capturedChargeTime = chargeUpTime = attackWarmup;
        _myRenderer.material.SetTexture("_MainTex", startFlash);
        fadeColor.a = 0;
        _myRenderer.material.color = fadeColor;
        _myCollider.enabled = false;
        _myRenderer.enabled = true;
        charging = true;



        /*if (soundObj && standalone){
			Instantiate(soundObj);
		}**/

    }

    void SpawnObjects()
    {
        if (spawnOnCharge)
        {
            for (int i = 0; i < numToSpawn; i++)
            {
                spawnPos = transform.position + spawnOffset;
                spawnPos += Random.insideUnitSphere * spawnRadius;
                spawnPos.z = spawnZ;
                Instantiate(spawnOnCharge, spawnPos, Quaternion.identity);
            }
        }
    }

    public void SetEnemy(EnemyS myRef)
    {
        if (myRef)
        {
            myEnemy = myRef;
            isFriendly = myEnemy.isFriendly;
        }
    }

    //________________________________________________________FOS STUFF
    void FosMovement()
    {
        if (myEnemy && visibleTime > 0 && visibleTime < 10 && !fosFired)
        {
            if (currentHit >= maxHits)
            {
                visibleTime = 0f;
            }
            else
            {
                visibleTime -= Time.deltaTime;
                if (visibleTime <= 0)
                {
                    FosEndFire();
                }
            }
            currentAngle += angleRate * Time.deltaTime;
            currentFosPos.x = floatAxisA * Mathf.Cos(currentAngle);
            currentFosPos.y = floatAxisB * Mathf.Sin(currentAngle);
            transform.position = myEnemy.transform.position + currentFosPos;
            DoRotation();
        }
    }

    public void FosPause(Vector3 newDir)
    {
        float rotateZ = 0;

        newDir = savedFireDirection = newDir.normalized;

        if (newDir.x == 0)
        {
            if (newDir.y > 0)
            {
                rotateZ = 90;
            }
            else
            {
                rotateZ = -90;
            }
        }
        else
        {
            rotateZ = Mathf.Rad2Deg * Mathf.Atan((newDir.y / newDir.x));
        }


        if (newDir.x < 0)
        {
            rotateZ += 180;
        }


        _myCollider.enabled = false;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotateZ + startRotate + 90f));
        visibleTime = 9999f;
        if (currentHit < maxHits)
        {
            currentHit = maxHits - 1;
        }
    }

    public void FosDirectedFire(bool first = false)
    {
        Vector3 shootForce = savedFireDirection;
        shootForce.z = 0f;
        shootForce = shootForce * shootSpeed * Time.unscaledDeltaTime;
        _myRigid.AddForce(shootForce, ForceMode.Impulse);
        visibleTime = shootLifetime;
        fosFired = true;
        _myCollider.enabled = true;

        CameraShakeS.C.SmallShake();
        if (first)
        {
            CameraShakeS.C.SmallSleep();
            CameraShakeS.C.SloAndPunch(slowTime, punchMult, hangTime);
        }
        if (currentHit < maxHits)
        {
            currentHit = maxHits - 1;
        }
        fadeRate = fosFiredFade;
    }

    public void FosEndFire(bool fromPlayer = false)
    {

        Vector3 shootForce = myEnemy.transform.position - transform.position;
        shootForce.z = 0f;
        shootForce = shootForce.normalized * shootSpeed * Time.unscaledDeltaTime;
        _myRigid.AddForce(shootForce, ForceMode.Impulse);
        visibleTime = shootLifetime;
        fosFired = true;
        _myCollider.enabled = true;
        if (currentHit < maxHits)
        {
            currentHit = maxHits - 1;
        }

        if (!fromPlayer)
        {
            CameraShakeS.C.SmallShake();
            CameraShakeS.C.SmallSleep();
            CameraShakeS.C.SloAndPunch(slowTime, punchMult, hangTime);
            myEnemy.TriggerAllFos(this);
        }
        fadeRate = fosFiredFade;

    }

    void DoRotation()
    {
        float rotateZ = 0;

        Vector3 targetDir = (transform.position - myEnemy.transform.position).normalized;

        if (targetDir.x == 0)
        {
            if (targetDir.y > 0)
            {
                rotateZ = 90;
            }
            else
            {
                rotateZ = -90;
            }
        }
        else
        {
            rotateZ = Mathf.Rad2Deg * Mathf.Atan((targetDir.y / targetDir.x));
        }


        if (targetDir.x < 0)
        {
            rotateZ += 180;
        }



        transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotateZ + startRotate + 90f));

    }
}
