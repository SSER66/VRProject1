using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talkable : MonoBehaviour
{
    [Header("对话设置")]
    [TextArea(1, 3)] public string[] dialogueLines; // 重命名更语义化
    [SerializeField] private string interactPromptText = "按空格对话"; // 交互提示文本
    [SerializeField] private KeyCode interactKey = KeyCode.Space; // 自定义交互键

    [Header("触发设置")]
    [SerializeField] private Collider triggerCollider; // 显式引用触发器
    [SerializeField] private bool showPrompt = true; // 是否显示交互提示

    private bool isPlayerInTrigger;
    private GameObject promptUI; // 可选：交互提示UI

    private void Awake()
    {
        // 自动获取触发器（如果未手动赋值）
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<Collider>();
            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true; // 确保是触发器
            }
            else
            {
                Debug.LogError($"{gameObject.name} 缺少Collider组件！");
            }
        }

        // 初始化提示UI（需自行创建并赋值，或注释掉）
        // promptUI = transform.Find("InteractPrompt").gameObject;
        // if (promptUI != null) promptUI.SetActive(false);
    }

    // 触发器进入
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            if (showPrompt && promptUI != null)
            {
                promptUI.SetActive(true);
            }
        }
    }

    // 触发器离开
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            if (showPrompt && promptUI != null)
            {
                promptUI.SetActive(false);
            }

            // 可选：玩家离开后关闭对话
            // DialogMananger.instance.CloseDialog();
        }
    }

    private void Update()
    {
        HandleInteractInput();
    }

    /// <summary>
    /// 处理交互输入
    /// </summary>
    private void HandleInteractInput()
    {
        // 条件校验：玩家在触发器内 + 按下交互键 + 对话面板未激活
        if (isPlayerInTrigger
            && Input.GetKeyDown(interactKey)
            && !DialogManager.instance.dialogueBox.activeInHierarchy)
        {
            // 空对话校验
            if (dialogueLines == null || dialogueLines.Length == 0)
            {
                Debug.LogWarning($"{gameObject.name} 未设置对话内容！");
                return;
            }

            // 显示对话
            DialogManager.instance.ShowDialog(dialogueLines);

            // 隐藏交互提示
            if (showPrompt && promptUI != null)
            {
                promptUI.SetActive(false);
            }
        }
    }

    // 可选：Gizmos绘制触发器范围（便于调试）
    private void OnDrawGizmosSelected()
    {
        if (triggerCollider != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(triggerCollider.bounds.center, triggerCollider.bounds.size);
        }
    }
}