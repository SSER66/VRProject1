using System.Collections.Generic;
using UnityEngine;

public class UIPanelManager : MonoBehaviour
{
    public static UIPanelManager instance;

    [Header("UI面板顺序")]
    public List<GameObject> panelOrderList;

    [Header("对话面板配置")]
    public int dialoguePanelIndex;
    public GameObject catSprite;

    private int _currentIndex = -1;
    public bool IsDialogueActive => _currentIndex == dialoguePanelIndex;

    private void Awake()
    {
        instance = this;
        InitAllPanels();
    }

    private void Start()
    {
        // 确保列表非空且索引有效
        if (panelOrderList != null && panelOrderList.Count > 0)
            ShowPanel(0);
        else
            Debug.LogError("UIPanelManager: panelOrderList 为空或没有元素！");
    }

    void InitAllPanels()
    {
        if (panelOrderList == null) return;
        foreach (var p in panelOrderList)
            if (p != null) p.SetActive(false);

        if (catSprite != null)
            catSprite.SetActive(false);
    }

    public void ShowPanel(int index)
    {
        // 边界检查
        if (panelOrderList == null || panelOrderList.Count == 0)
        {
            Debug.LogError("UIPanelManager: panelOrderList 为空，无法显示面板");
            return;
        }
        if (index < 0 || index >= panelOrderList.Count)
        {
            Debug.LogError($"UIPanelManager: 索引 {index} 超出范围 (0-{panelOrderList.Count - 1})");
            return;
        }

        // 隐藏当前面板
        if (_currentIndex >= 0 && _currentIndex < panelOrderList.Count && panelOrderList[_currentIndex] != null)
            panelOrderList[_currentIndex].SetActive(false);

        // 第一个UI关闭时显示猫精灵（根据需求调整）
        if (_currentIndex == 0 && catSprite != null)
            catSprite.SetActive(true);

        _currentIndex = index;
        if (panelOrderList[index] != null)
            panelOrderList[index].SetActive(true);
    }

    public void ShowNextPanel()
    {
        int next = _currentIndex + 1;
        if (next < panelOrderList.Count)
            ShowPanel(next);
    }
}