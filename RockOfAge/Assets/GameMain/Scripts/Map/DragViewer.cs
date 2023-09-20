using System.Collections.Generic;
using UnityEngine;

public class DragViewer : MonoBehaviour
{
    public GameObject previewObject; // 미리보기로 사용할 오브젝트
    public LayerMask groundLayer; // 지면 레이어

    private GameObject previewInstance;
    private LineRenderer lineRenderer;

    private Vector3 startPos;
    private Vector3 endPos;

    public List<Material> materials;

    enum WhatMaterial
    {
        DENY = 0,
        ALLOW = 1
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false; // 초기에는 비활성화
    }

    public void UpdateColor(bool canBuild) 
    {
        if(canBuild)
        {
            lineRenderer.material = materials[(int)WhatMaterial.ALLOW];
        }
        else
        {
            lineRenderer.material = materials[(int)WhatMaterial.DENY];
        }
    }

    public void StartDrag(Vector3 start)
    {
        RaycastHit hit;
        if (Physics.Raycast(start+Vector3.up*BuildManager.MAP_SIZE_Y, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            startPos = hit.point + Vector3.up*.1f;
            //previewInstance = Instantiate(previewObject, hit.point, Quaternion.identity);
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, startPos);
        }
    }

    public void ContinueDrag(Vector3 end)
    {
        end.y = startPos.y;
        lineRenderer.SetPosition(1, end);
    }

    public void EndDrag()
    {
        lineRenderer.enabled = false;
    }
}