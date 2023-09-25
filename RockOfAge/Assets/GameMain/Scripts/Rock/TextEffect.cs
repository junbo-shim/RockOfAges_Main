using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TextEffect : MonoBehaviour
{
    const float ROTATE_SPEED = 20F;
    const float SCALE_MULTIPLE = .85F;
    const float ROOP_TIME = .025F;
    const float END_TIME = 3F;

    private void Start()
    {
        StartCoroutine(TextEffectRoutine(ROOP_TIME, END_TIME));
    }

    IEnumerator TextEffectRoutine(float roopTime, float endTime)
    {
        float time = 0;
        float count = 0;
        while(time < endTime)
        {
            RotateObject(ROTATE_SPEED);
            if(count%3==0)
                ScaleObject(SCALE_MULTIPLE);

            yield return new WaitForSeconds(roopTime);
            time += roopTime;
            count+=1;
        }
    }

    void RotateObject(float speed)
    {
        float rotationAngle = speed * Time.fixedDeltaTime;

        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation += Vector3.forward * rotationAngle;

        transform.rotation = Quaternion.Euler (currentRotation);
    }

    void ScaleObject(float multiple)
    {

        Vector3 currentScale = transform.localScale;
        currentScale *= multiple;

        transform.localScale = currentScale;
    }
}
