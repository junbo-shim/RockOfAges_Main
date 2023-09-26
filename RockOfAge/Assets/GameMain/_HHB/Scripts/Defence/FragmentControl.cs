using PlayFab.GroupsModels;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum Position 
{ 
    LEFT = 0, MID =1, RIGHT = 2, LENGTH = 3
}

public class FragmentControl : MonoBehaviour
{
    private RectTransform fragmentRect;
    float startX = -512f;
    float startY = -376f;
    Vector2 startPosition;
    Vector2 targetPosition;
    private void Awake()
    {
        fragmentRect = GetComponent<RectTransform>();
        targetPosition = default;
        startPosition = new Vector2(startX, startY);
    }

    private void Start()
    {
        DefineTargetPosition();
    }

    private void DefineTargetPosition()
    {
    
        int randomRoot = UnityEngine.Random.Range((int)Position.LEFT, (int)Position.LENGTH);
        switch (randomRoot) 
        {
            case (int)Position.LEFT:
                MoveLeft(fragmentRect);
                break;
            case (int)Position.MID:
                MoveMid(fragmentRect);
                break;
            case (int)Position.RIGHT:
                MoveRight(fragmentRect);
                break;
        }
            
    }

    private void MoveLeft(RectTransform fragment)
    {
        targetPosition = new Vector2(-574f, -480f);
        StartCoroutine(MoveRectWithLerp(targetPosition, fragment));
    }

    private void MoveRight(RectTransform fragment) 
    {
        targetPosition = new Vector2(-512f, -480f);
        StartCoroutine(MoveRectWithLerp(targetPosition, fragment));
    }

    private void MoveMid(RectTransform fragment)
    {
        targetPosition = new Vector2(-432f, -480f);
        StartCoroutine(MoveRectWithLerp(targetPosition, fragment));

    }

    IEnumerator MoveRectWithLerp(Vector2 targetPosition, RectTransform fragement)
    {
        float startTime = 0f;
        float timer = 0.3f;
        while (startTime < timer)
        {
            if (RockObjectPooling.objectPooling.LimitRockCount())
            {
                float alpha = startTime / timer;
                fragement.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, alpha);
                startTime += Time.deltaTime;
                yield return null;
            }
            else { yield return null; }
        }
    }

}
