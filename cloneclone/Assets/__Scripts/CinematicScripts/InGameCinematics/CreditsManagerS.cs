using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsManagerS : MonoBehaviour {

    private bool _creditsFinished = false;
    public bool creditsFinished { get { return _creditsFinished; } }

    public Transform[] checkTransforms;
    private float endCreditMoveY;
    public Transform creditDoneTransform;
    int numOfFinishedTransforms = 0;

    public float scrollRate = 2f;
    private float startScrollRate;
    public Vector3 scrollDir = new Vector3(0, 1f, 0);

	private void Start()
	{
        endCreditMoveY = creditDoneTransform.position.y;
        startScrollRate = scrollRate;
	}


	// Update is called once per frame
	void Update () {

        if (!creditsFinished)
        {
            if (Input.GetKey(KeyCode.E))
            {
                scrollRate = 10f;
            }
            else{
                scrollRate = startScrollRate;
            }
            for (int i = numOfFinishedTransforms; i < checkTransforms.Length; i++)
            {
                if (checkTransforms[i].position.y >= endCreditMoveY)
                {
                    numOfFinishedTransforms++;
                    checkTransforms[i].parent.gameObject.SetActive(false);
                    if (numOfFinishedTransforms >= checkTransforms.Length)
                    {
                        _creditsFinished = true;
                        Debug.Log(_creditsFinished);
                    }
                }
                else
                {
                    checkTransforms[i].parent.position += scrollDir * scrollRate * Time.deltaTime;
                }
            }
        }
	}
}
