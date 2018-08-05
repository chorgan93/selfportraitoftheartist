using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformEffectUI : MonoBehaviour {

    [Header("Left/Right Images")]
    public Image[] leftImages;
    private Vector2 leftStartSize;
    private Vector2 leftStartPos;
    public Image[] rightImages;
    private Vector2 rightStartSize;
    private Vector2 rightStartPos;

    private float leftRightSpawnVar = 300;
    private float leftRightSizeVar = 30f;

    private Vector2 newPos;
    private Vector2 newSize;

    [Header("Up/Down Images")]
    public Image[] upImages;
    private Vector2 upStartSize;
    private Vector2 upStartPos;
    public Image[] downImages;
    private Vector2 downStartSize;
    private Vector2 downStartPos;

    [Header("Percent Images")]
    public Image[] percentImages;
    private Vector2 percentStartSize;
    private Vector2 percentStartPos;
    private float percentSpawnVar = 100;

    private float upDownSpawnVar = 400;
    private float upDownSizeVar = 20f;

    private float changeRate = 0.012f;
    private float changeCountdown = 0f;

	// Use this for initialization
	void Start () {

        leftStartPos = leftImages[0].rectTransform.anchoredPosition;
        leftStartSize = leftImages[0].rectTransform.sizeDelta;
        rightStartPos = rightImages[0].rectTransform.anchoredPosition;
        rightStartSize = rightImages[0].rectTransform.sizeDelta;

        upStartPos = upImages[0].rectTransform.anchoredPosition;
        upStartSize = upImages[0].rectTransform.sizeDelta;
        downStartPos = downImages[0].rectTransform.anchoredPosition;
        downStartSize = downImages[0].rectTransform.sizeDelta;

        percentStartPos = percentImages[0].rectTransform.anchoredPosition;
        percentStartSize = percentImages[0].rectTransform.sizeDelta;

        SetImages();

	}
	
	// Update is called once per frame
	void Update () {
        changeCountdown -= Time.deltaTime;
        if (changeCountdown <= 0){
            SetImages();
        }
	}

    void SetImages(){

        for (int i = 0; i < leftImages.Length; i++){
            newPos = leftStartPos;
            newPos.y += leftRightSpawnVar * Random.insideUnitCircle.y;
            leftImages[i].rectTransform.anchoredPosition = newPos;

            newSize = leftStartSize;
            newSize.y += leftRightSizeVar * Random.insideUnitCircle.y;
            leftImages[i].rectTransform.sizeDelta = newSize;
        }

        for (int i = 0; i < rightImages.Length; i++)
        {
            newPos = rightStartPos;
            newPos.y += leftRightSpawnVar * Random.insideUnitCircle.y;
            rightImages[i].rectTransform.anchoredPosition = newPos;

            newSize = rightStartSize;
            newSize.y += leftRightSizeVar * Random.insideUnitCircle.y;
            rightImages[i].rectTransform.sizeDelta = newSize;
        }

        for (int i = 0; i < upImages.Length; i++)
        {
            newPos = upStartPos;
            newPos.x += upDownSpawnVar * Random.insideUnitCircle.x;
            upImages[i].rectTransform.anchoredPosition = newPos;

            newSize = upStartSize;
            newSize.y *= 0.5f;
            newSize.y += upDownSizeVar * Random.insideUnitCircle.y;
            upImages[i].rectTransform.sizeDelta = newSize;
        }

        for (int i = 0; i < downImages.Length; i++)
        {
            newPos = downStartPos;
            newPos.x += upDownSpawnVar * Random.insideUnitCircle.x;
            downImages[i].rectTransform.anchoredPosition = newPos;

            newSize = downStartSize;
            newSize.y *= 0.5f;
            newSize.y += upDownSizeVar * Random.insideUnitCircle.y;
            downImages[i].rectTransform.sizeDelta = newSize;
        }

        for (int i = 0; i < percentImages.Length; i++)
        {
            newPos = percentStartPos;
            newPos.x += percentSpawnVar * Random.insideUnitCircle.x;
            percentImages[i].rectTransform.anchoredPosition = newPos;

            newSize = percentStartSize;
            newSize.y += upDownSizeVar * Random.insideUnitCircle.y;
            percentImages[i].rectTransform.sizeDelta = newSize;
        }

        changeCountdown = changeRate;

    }
}
