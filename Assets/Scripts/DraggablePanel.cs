using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ShopPanel 등 UI 패널을 드래그로 이동할 수 있게 해주는 컴포넌트.
/// RectTransform에 붙이고, 패널 상단 헤더(Title 영역)에 IDragHandler 이벤트를 연결합니다.
/// </summary>
public class DraggablePanel : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas        rootCanvas;
    private Vector2       dragOffset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rootCanvas    = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 클릭 위치와 패널 pivot 위치의 차이를 기록
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform,
            eventData.position,
            rootCanvas.worldCamera,
            out Vector2 localPoint);

        dragOffset = rectTransform.anchoredPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rootCanvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform,
            eventData.position,
            rootCanvas.worldCamera,
            out Vector2 localPoint);

        rectTransform.anchoredPosition = localPoint + dragOffset;
    }
}
