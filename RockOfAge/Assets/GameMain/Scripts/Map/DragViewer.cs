using UnityEngine;

public class DragViewer : MonoBehaviour
{
    public GameObject previewObject; // 미리보기로 사용할 오브젝트
    public LayerMask groundLayer; // 지면 레이어

    private GameObject previewInstance;
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false; // 초기에는 비활성화
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }
        else if (Input.GetMouseButton(0))
        {
            ContinueDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    private void StartDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            previewInstance = Instantiate(previewObject, hit.point, Quaternion.identity);
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, previewInstance.transform.position);
            lineRenderer.SetPosition(1, previewInstance.transform.position);
        }
    }

    private void ContinueDrag()
    {
        if (previewInstance != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                previewInstance = Instantiate(previewObject, hit.point, Quaternion.identity);
                previewInstance.transform.position = hit.point;
                lineRenderer.SetPosition(1, previewInstance.transform.position);
            }
        }
    }

    private void EndDrag()
    {
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            lineRenderer.enabled = false;
        }
    }
}