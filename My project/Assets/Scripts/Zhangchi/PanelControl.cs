using UnityEngine;
using UnityEngine.UI;

public class PanelControl : MonoBehaviour
{
    [Tooltip("需要控制的目标面板")]
    public GameObject targetPanel;

    [Tooltip("关闭按钮（自动找子物体Button）")]
    public Button hideButton;

    [Tooltip("当前面板在UIPanelManager.panelOrderList中的索引")]
    public int panelIndex = 0;

    [Tooltip("关闭时是否自动显示下一个面板")]
    public bool showNextPanelOnHide = true;

    private void Awake()
    {
        // 未配置目标面板时，默认使用自身
        if (targetPanel == null)
        {
            targetPanel = gameObject;
            Debug.LogWarning($"PanelControl: 未配置targetPanel，使用自身 {gameObject.name}");
        }

        // 初始隐藏面板
        if (targetPanel != null)
            targetPanel.SetActive(false);
        else
            Debug.LogError("PanelControl: 目标面板为空！");
    }

    private void Start()
    {
        // 自动绑定关闭按钮
        if (hideButton == null && targetPanel != null)
            hideButton = targetPanel.GetComponentInChildren<Button>();

        if (hideButton != null)
        {
            hideButton.onClick.RemoveAllListeners();
            hideButton.onClick.AddListener(HidePanel);
        }
        else
        {
            Debug.LogWarning($"PanelControl: {gameObject.name} 未找到关闭按钮！");
        }
    }

    // 隐藏面板（调用UIManager逻辑）
    public void HidePanel()
    {
        // 对话层激活时，禁止关闭普通面板
        if (UIPanelManager.instance != null && UIPanelManager.instance.IsDialogueActive)
        {
            Debug.Log("PanelControl: 对话层激活中，无法关闭普通面板！");
            return;
        }

        if (targetPanel != null)
        {
            targetPanel.SetActive(false);
            Debug.Log($"PanelControl: 隐藏面板 [{panelIndex}] {targetPanel.name}");
        }

        // 触发下一个面板显示（由UIManager控制顺序）
        if (showNextPanelOnHide && UIPanelManager.instance != null)
        {
            UIPanelManager.instance.ShowNextPanel();
        }
    }

    // 显示面板（调用UIManager逻辑）
    public void ShowPanel()
    {
        if (UIPanelManager.instance != null)
        {
            UIPanelManager.instance.ShowPanel(panelIndex);
        }
        else
        {
            if (targetPanel != null)
            {
                targetPanel.SetActive(true);
                Debug.Log($"PanelControl: 显示面板 [{panelIndex}] {targetPanel.name}");
            }
        }
        // 显示面板时暂停对话自动触发
        DialogManager.instance?.PauseDialogAutoTrigger();
    }

    // 刷新面板状态（同步UIManager的当前面板）
    
}