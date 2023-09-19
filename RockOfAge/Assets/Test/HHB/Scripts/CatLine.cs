using UnityEngine;

public class CatLine : MonoBehaviour
{
    private Transform catTransform;
    private Transform ballonTransform;
    private Vector3 lineUp = new Vector3(0f, 0.2f, 0f);

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = transform.GetChild(0).GetComponent<LineRenderer>();
        catTransform = transform.GetChild(0).transform;
        ballonTransform = transform.GetChild(1).transform;
    }

    private void Update()
    {
        DrawLineBetweenCatAndBallon();
    }


    private void DrawLineBetweenCatAndBallon()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, catTransform.position + lineUp);
        lineRenderer.SetPosition(1, ballonTransform.position);
    }
}
