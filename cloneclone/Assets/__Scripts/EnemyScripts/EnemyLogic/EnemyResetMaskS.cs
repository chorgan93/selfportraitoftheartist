using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyResetMaskS : MonoBehaviour {

    private SpriteRenderer mySprite;
    private SpriteMask myMask;

	// Use this for initialization
	void Start () {
        mySprite = GetComponentInParent<SpriteRenderer>();
        myMask = GetComponent<SpriteMask>();
        if (mySprite.flipX){
            Vector3 fixScale = transform.localScale;
            fixScale.x *= -1f;
            transform.localScale = fixScale;
        }
        myMask.sprite = mySprite.sprite;
    }
	
	// Update is called once per frame
	void Update () {
        myMask.sprite = mySprite.sprite;
	}
}
