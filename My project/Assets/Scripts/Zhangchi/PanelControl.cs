using UnityEngine;
using UnityEngine.UI;

public class PanelControl : MonoBehaviour
{
    [Tooltip("可选：指定要绑定隐藏事件的按钮，如果不指定则自动在子物体中查找")]
    public Button hideButton;

    private void Awake()
    {
        // 确保面板在游戏运行时立即显示（因为初始状态可能是隐藏的）
        gameObject.SetActive(true);
    }

    private void Start()
    {
        // 如果没有手动指定按钮，尝试从子物体中获取第一个 Button 组件
        if (hideButton == null)
        {
            hideButton = GetComponentInChildren<Button>();
        }

        if (hideButton != null)
        {
            // 添加点击监听，避免重复绑定（先移除再添加）
            hideButton.onClick.RemoveListener(HidePanel);
            hideButton.onClick.AddListener(HidePanel);
        }
        else
        {
            Debug.LogWarning("未找到可用的按钮，无法自动绑定隐藏功能。");
        }
    }

    // 公有方法，供按钮事件或外部调用，用于隐藏面板
    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}