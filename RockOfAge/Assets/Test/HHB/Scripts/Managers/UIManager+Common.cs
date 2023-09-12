using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Common
public partial class UIManager: MonoBehaviour
{
    public GameObject commonUI;
    private bool _mButtonPressed = false;
    private float _pressedTime = 0f;

    public void TurnOnCommonUI()
    {
        if (commonUI.activeSelf == false)
        {
            commonUI.SetActive(true);
        }
        else { return; }
    }

    public void GetRotationKey()
    {
        if (Input.GetKey(KeyCode.M) == true)
        {
            if (_mButtonPressed == false)
            {
                _mButtonPressed = true;
                _pressedTime = Time.time;
            }
        }
        else
        {
            _mButtonPressed = false;
        }

        if (_mButtonPressed && (Time.time - _pressedTime) >= 1f)
        {
            _mButtonPressed = false;
            _pressedTime = 0f;
            RotateMirror();
        }
    }

    public void RotateMirror()
    {
        // 상대가 공 굴리고 있을 때 카메라 연동과 함께(포톤처리)
        MirrorRotate mirrorRotate = FindObjectOfType<MirrorRotate>();
        mirrorRotate.RotateMirror();
    }
}
