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
        Cursor.lockState = CursorLockMode.Locked; // 커서를 화면 중앙에 고정
    }

    void Update()
    {
        // 마우스 입력을 받아 카메라 회전
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // 상하 회전 각도 제한

        transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // 카메라의 상하 회전
        transform.Rotate(Vector3.up * mouseX); // 플레이어 오브젝트의 좌우 회전
    }
}
