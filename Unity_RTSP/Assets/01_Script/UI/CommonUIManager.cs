using UnityEngine;
using UnityEditor;

public class CommonUIManager : MonoBehaviour
{
    [SerializeField] private GameObject addDisplayPanel;
    [SerializeField] private GameObject detailSettingPanel;
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private PageManager pageManager;
    private DynamicGridSizer fullscreenPage;

    void Update()
    {
        fullscreenPage = pageManager.GetCurrentPage().holder.GetComponent<DynamicGridSizer>();
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(addDisplayPanel.activeSelf)
            {
                addDisplayPanel.SetActive(false);
            }
            else if(detailSettingPanel.activeSelf)
            {
                detailSettingPanel.SetActive(false);
            }
            else if(exitPanel.activeSelf)
            {
                exitPanel.SetActive(false);
            }
            else if(fullscreenPage.useFullscreen)
            {
                fullscreenPage.Shrink();
            }
            else
            {
                exitPanel.SetActive(true);
            }
        }
    }

    public void Exit()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
}
