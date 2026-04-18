using UnityEngine;
using UnityEngine.UI;

public class PanelControl : MonoBehaviour
{
    [Tooltip("需要控制显示/隐藏的面板")]
    public GameObject Panel;

    [Tooltip("可选：指定要绑定隐藏事件的按钮，如果不指定则自动在 Panel 的子物体中查找")]
    public Button hideButton;

    private void Awake()
    {
        // 游戏开始时显示面板
        if (Panel != null)
            Panel.SetActive(true);
        else
            Debug.LogError("PanelControl：未指定需要控制的面板！");
    }

    private void Start()
    {
        // 如果没有手动指定按钮，则尝试在 Panel 的子物体中查找第一个 Button
        if (hideButton == null && Panel != null)
        {
            hideButton = Panel.GetComponentInChildren<Button>();
        }

        if (hideButton != null)
        {
            // 先移除再添加，防止重复绑定
            hideButton.onClick.RemoveListener(HidePanel);
            hideButton.onClick.AddListener(HidePanel);
        }
        else
        {
            Debug.LogWarning("未找到可用的按钮，请手动将按钮拖拽到 hideButton 字段");
        }
    }

    // 公有方法，供按钮点击调用
    public void HidePanel()
    {
        if (Panel != null)
            Panel.SetActive(false);
    }
}