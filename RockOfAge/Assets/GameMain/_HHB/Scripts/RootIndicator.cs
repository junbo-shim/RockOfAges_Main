using UnityEngine;
using UnityEngine.EventSystems;

public class RootIndicator : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    protected Vector2 movePoint;
    public void OnPointerClick(PointerEventData eventData)
    {
        CameraManager.Instance.MoveTurnOnCameraPosition(movePoint);
    }
}
