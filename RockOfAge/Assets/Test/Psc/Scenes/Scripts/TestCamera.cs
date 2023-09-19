using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    public Transform player; // 플레이어 또는 대상 오브젝트
    public float sensitivity = 2.0f; // 카메라 민감도 (조절 가능)

    private float rotationX = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined; // 커서를 화면 중앙에 고정
    }

}
