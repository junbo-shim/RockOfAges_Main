using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;

[ExecuteInEditMode()]
public class SamplingSpline : MonoBehaviour
{
    //spline의 정보를 가지고 있는 container
    [SerializeField]
    private SplineContainer m_splineContainer;

    //ground의 폭의 반(6임)
    [SerializeField]
    private float m_width = 3f;




    public void SampleSplineWidth(float time, out Vector3 rightPoint, out Vector3 leftPoint, int index=0)
    {
        //evaout에 사용할 변수들
        float3 position;
        float3 tangent;
        float3 upVector;

        //index : 이 오브젝트가 참조하는 spline 인덱스
        m_splineContainer.Evaluate(index, time, out position, out tangent, out upVector);

        Vector3 _position = position;
        Vector3 right = Vector3.Cross(tangent, upVector).normalized;

        rightPoint = _position + (right * m_width);
        leftPoint = _position + (-right * m_width);
    }

    public bool IsClosed()
    {
        return m_splineContainer.Spline.Closed;
    }
    public int GetSplineCount()
    {
        return m_splineContainer.Spline.Count;
    }
}
