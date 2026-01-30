using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DynamicGridSizer : MonoBehaviour
{
    public bool useFullscreen = false;   // 🔥 이걸로 ON/OFF
    public int maxColumns = 3;
    public int maxRows = 2;

    private GridLayoutGroup grid;
    private RectTransform rect;

    [SerializeField] private DisplayPage page;

    [Header("Objects to toggle")]

    [SerializeField] private GameObject btn_expand;
    [SerializeField] private GameObject btn_shrink;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject addDisplay;

    private Vector2 originalCellSize; // 원래 셀 크기 저장

    void Start()
    {
        Initialize();
    }

     void Update()
    {
        btn_expand.SetActive(page.GetDisplayCount() > 0 && !useFullscreen);
    }

    public void ApplySizing()
    {
        if (!useFullscreen)
        {
            grid.cellSize = originalCellSize; // 원래 크기로 복구
            return;
        }

        UpdateCellSize();
    }

    private void UpdateCellSize()
    {
        List<RectTransform> activeChildren = GetActiveChildren();
        int activeChildCount = activeChildren.Count;
        if (activeChildCount == 0) return;

        int columns = Mathf.Min(activeChildCount, maxColumns);
        int rows = Mathf.CeilToInt(activeChildCount / (float)columns);
        rows = Mathf.Min(rows, maxRows);

        float totalWidth = rect.rect.width - grid.padding.left - grid.padding.right - grid.spacing.x * (columns - 1);
        float totalHeight = rect.rect.height - grid.padding.top - grid.padding.bottom - grid.spacing.y * (rows - 1);

        float maxCellWidth = totalWidth / columns;
        float maxCellHeight = totalHeight / rows;

        float aspect = GetReferenceAspectRatio(activeChildren[0]);

        Vector2 finalSize = CalculateCellSizeWithAspect(maxCellWidth, maxCellHeight, aspect);

        grid.cellSize = finalSize;
    }

    private List<RectTransform> GetActiveChildren()
    {
        List<RectTransform> list = new List<RectTransform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeInHierarchy)
            {
                RectTransform rt = child as RectTransform;
                if (rt != null)
                    list.Add(rt);
            }
        }

        return list;
    }

    private float GetReferenceAspectRatio(RectTransform target)
    {
        float width = target.rect.width;
        float height = target.rect.height;

        if (height <= 0.01f) return 1f;

        return width / height;
    }

    private Vector2 CalculateCellSizeWithAspect(float maxWidth, float maxHeight, float aspect)
    {
        float width = maxWidth;
        float height = width / aspect;

        if (height > maxHeight)
        {
            height = maxHeight;
            width = height * aspect;
        }

        return new Vector2(width, height);
    }

    public void Expand()
    {
        useFullscreen = true;
        backgroundImage.enabled = true;
        btn_shrink.SetActive(true);
        addDisplay.SetActive(false);

        ApplySizing();
    }
    public void Shrink()
    {
        useFullscreen = false;
        backgroundImage.enabled = false;
        btn_shrink.SetActive(false);
        addDisplay.SetActive(true);

        page.CheckAddDisplayVisible();
        
        ApplySizing();
    }


    public void Initialize()
    {
        grid = GetComponent<GridLayoutGroup>();
        rect = GetComponent<RectTransform>();
        originalCellSize = grid.cellSize; // 시작값 저장
    }
}