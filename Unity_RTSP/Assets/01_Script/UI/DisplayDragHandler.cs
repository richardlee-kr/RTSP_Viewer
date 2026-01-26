using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DisplayDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image playerTitleImage;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 originalPosition;
    private bool dragging = false;
    private Vector2 pointerOffset;

    private PageManager pageManager;
    private GridLayoutGroup layoutGroup; // 부모의 GridLayoutGroup

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        pageManager = GetComponentInParent<PageManager>();
        layoutGroup = GetComponentInParent<GridLayoutGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter == playerTitleImage.gameObject)
        {
            dragging = true;
            originalPosition = rectTransform.position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out pointerOffset
            );

            // 드래그 중 GridLayoutGroup 비활성 → 다른 Display 위치 고정
            if(layoutGroup != null)
                layoutGroup.enabled = false;

            // 드래그 중 최상위로 이동
            rectTransform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging) return;

        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out globalMousePos))
        {
            rectTransform.position = globalMousePos - (Vector3)pointerOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragging) return;
        dragging = false;

        // 드랍 위치 감지
        GameObject targetDisplay = GetDisplayUnderPointer(eventData);

        if (targetDisplay != null && targetDisplay != this.gameObject)
        {
            {
                pageManager.SwapDisplays(this.gameObject, targetDisplay);
            }
        }

        // GridLayoutGroup 재활성화 → Reorder 후 UI 재배치
        if(layoutGroup != null)
            layoutGroup.enabled = true;

        pageManager.ReorderDisplays();
    }

    private GameObject GetDisplayUnderPointer(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("Display") && result.gameObject != this.gameObject)
            {
                return result.gameObject;
            }
        }

        return null;
    }
}
