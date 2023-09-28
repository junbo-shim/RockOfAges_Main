using UnityEngine;
using UnityEngine.EventSystems;

public class RootIndicator : MonoBehaviour, IPointerClickHandler
{
    public Vector2 movePoint;
    public void OnPointerClick(PointerEventData eventData)
    {
        CameraManager.Instance.MoveTurnOnCameraPosition(movePoint);
    }
}
