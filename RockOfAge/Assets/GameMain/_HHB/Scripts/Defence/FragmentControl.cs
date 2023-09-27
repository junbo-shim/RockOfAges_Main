using System;
using System.Collections;
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
    int randomRoot;
    Vector2 startPosition;
    Vector2 targetPosition;

    public void PackFragmentMovement()
    {
        fragmentRect = GetComponent<RectTransform>();
        float targetPositionX = UnityEngine.Random.Range(-574f, -432f);
        float tartgetPositionY = -480f;

        targetPosition = new Vector2 (targetPositionX, tartgetPositionY);   

        float randomX = UnityEngine.Random.Range(-100f, 100f);
        startPosition = new Vector2(startX + randomX, startY);
        DefineTargetPosition();
    }

    private void DefineTargetPosition()
    {    
        randomRoot = UnityEngine.Random.Range((int)Position.LEFT, (int)Position.LENGTH);
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
        float randomX = UnityEngine.Random.Range(-220f, -200f);
        float randomY = UnityEngine.Random.Range(-10f, 10f);
        Vector2 controlPoint = new Vector2(startPosition.x + randomX, startPosition.y + randomY);

        StartCoroutine(MoveRectWithBezier(targetPosition, fragment, controlPoint));
    }

    private void MoveRight(RectTransform fragment) 
    {
        float randomX = UnityEngine.Random.Range(200f, 220f);
        float randomY = UnityEngine.Random.Range(-10f, 10f);
        Vector2 controlPoint = new Vector2(startPosition.x + randomX, startPosition.y + randomY);

        StartCoroutine(MoveRectWithBezier(targetPosition, fragment, controlPoint));
    }

    private void MoveMid(RectTransform fragment)
    {
        Vector2 controlPoint = (startPosition + targetPosition) * 0.5f;
        StartCoroutine(MoveRectWithBezier(targetPosition, fragment, controlPoint));
    }

    #region LERP LEGACY
    //IEnumerator MoveRectWithLerp(Vector2 targetPosition, RectTransform fragement)
    //{
    //    float startTime = 0f;
    //    float timer = 0.4f;
    //    while (startTime < timer)
    //    {
    //        {
    //            float alpha = startTime / timer;
    //            fragement.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, alpha);
    //            startTime += Time.deltaTime;
    //            yield return null;
    //            if (Vector2.Distance(fragement.anchoredPosition, targetPosition) <= 0.001f)
    //            {
    //                RockObjectPooling.objectPooling.ReturnRockWithTimer(gameObject);
    //            }
    //        }
    //    }
    //}
    #endregion

    IEnumerator MoveRectWithBezier(Vector2 targetPosition, RectTransform fragment, Vector2 controlPoint)
    {
        float startTime = 0f;
        float duration = 0.5f;
        Vector2 newPosition;

        while (startTime < duration)
        {
            float t = startTime / duration;
            newPosition = CalculateBezier(startPosition, controlPoint, targetPosition, t);
            fragment.anchoredPosition = newPosition;

            startTime += Time.deltaTime;
            yield return null;

            if (Vector2.Distance(fragment.anchoredPosition, targetPosition) <= 0.001f)
            {
                RockObjectPooling.objectPooling.ReturnRockWithTimer(gameObject);
            }
        }
    }

    private Vector2 CalculateBezier(Vector2 start, Vector2 control, Vector2 end, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector2 p = (uu * start) + (2 * u * t * control) + (tt * end);
        return p;
    }
}
