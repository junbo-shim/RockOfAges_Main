using UnityEngine;
using System;
using System.Collections;
using Unity.VisualScripting;

public class MirrorRotate : MonoBehaviour
{
    private GameObject _mirror;
    //private float _rotationSpeed = 3000f;
    private bool _isRotating = false;
    private float addAngle = 10f;


    public void RotateMirror()
    {
        _mirror = this.gameObject;

        if (_isRotating == false)
        {
            StartCoroutine(RotateObject());
        }
        else { StartCoroutine(ReRotateObject()); }

    }

    IEnumerator RotateObject()
    {
        float startAngle = 0f;
        float targetAngle = 80f;
        Vector3 center = _mirror.transform.position;
        while (startAngle <= targetAngle)
        {
            startAngle += 10f;
            yield return new WaitForSeconds(0.05f);
            _mirror.transform.RotateAround(center, Vector3.back, addAngle);
        }
        _isRotating = true;
    }

    IEnumerator ReRotateObject()
    {
        float startAngle = 80f;
        float targetAngle = 0f;
        Vector3 center = _mirror.transform.position;
        while (startAngle >= targetAngle)
        {
            startAngle -= 10f;
            yield return new WaitForSeconds(0.05f);
            _mirror.transform.RotateAround(center, Vector3.forward, addAngle);
        }
        _isRotating = false;
    }
}