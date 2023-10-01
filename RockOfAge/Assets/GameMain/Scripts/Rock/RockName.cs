using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RockName : MonoBehaviour
{
    Camera main;

    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private GameObject rockObject;
    [SerializeField]
    TMP_Text tmpText;
    
    public bool isMineEnable = true;
    public bool isEnable = true;

    public Color color = Color.white;
    public string id = default;

    private void Awake()
    {
        main = Camera.main;
        rockObject = transform.parent.Find("RockObject").gameObject;
        tmpText = GetComponent<TMP_Text>();
        tmpText.color = color;
        tmpText.text = id;
    }

    void Update()
    {
        if (isEnable)
        {
            //조건 추가 : 내거가 아니거나+ 내거면서 mineenable이 활성화되있거나
            //if (isMineEnable) 
            {

                transform.position = rockObject.transform.position+(Vector3.up * rockObject.GetHeight(.1f)) + offset;

                transform.rotation = main.transform.rotation;

                // 현재 객체의 up 방향을 기준으로 180도 회전
                Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;

                // 객체의 회전을 목표 회전으로 직접 설정
                transform.rotation = targetRotation;
            }
        }
    }
}
