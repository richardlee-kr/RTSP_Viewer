using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayDoubleclickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float doubleClickTime = 0.3f;
    private DetailSettingPanel detailSettingPanel;

    private float lastClickTime;

    private void Start()
    {
        detailSettingPanel = (DetailSettingPanel)Object.FindFirstObjectByType(typeof(DetailSettingPanel), FindObjectsInactive.Include);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 왼쪽 마우스만 처리 (모바일이면 이 조건 제거 가능)
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (Time.unscaledTime - lastClickTime <= doubleClickTime)
        {
            OnDoubleClick();
            lastClickTime = 0f; // 연속 트리플 클릭 방지
        }
        else
        {
            lastClickTime = Time.unscaledTime;
        }
    }

    public void OnDoubleClick()
    {
        detailSettingPanel.SetTarget(GetComponent<RTSP_Player>());
        detailSettingPanel.gameObject.SetActive(true);
    }
}
