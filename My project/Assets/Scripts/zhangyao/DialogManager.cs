using UnityEngine;
using TMPro;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    [Header("对话UI配置")]
    public GameObject dialogueBox;
    public TMP_Text text;
    public float textSpeed = 0.03f;

    [Header("自动对话设置")]
    public bool autoStartDialog = false;
    [TextArea(1, 3)]
    public string[] startDialogLines;

    // 内部状态
    private string[] currentLines;
    private int currentLine;
    private bool isScrolling;
    private bool isDialogActive;
    private bool isDialogTriggerPaused = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // 空值保护
        if (dialogueBox != null)
            dialogueBox.SetActive(false);

        if (autoStartDialog && startDialogLines != null && startDialogLines.Length > 0 && !isDialogTriggerPaused)
        {
            ShowDialog(startDialogLines);
        }
    }

    // 外部调用：显示对话
    public void ShowDialog(string[] lines)
    {
        if (isDialogTriggerPaused || lines == null || lines.Length == 0)
        {
            Debug.LogWarning("对话内容为空，无法显示");
            return;
        }

        currentLines = lines;
        currentLine = 0;
        isDialogActive = true;

        // 联动UI面板
        if (UIPanelManager.instance != null)
        {
            int dpIndex = UIPanelManager.instance.dialoguePanelIndex;
            // 新增：索引合法才切换面板，避免报错
            if (dpIndex >= 0 && dpIndex < UIPanelManager.instance.panelOrderList.Count)
            {
                UIPanelManager.instance.ShowPanel(dpIndex);
            }
            else
            {
                Debug.LogWarning($"对话面板索引 {dpIndex} 未正确配置，已跳过面板切换，对话仍正常运行");
            }
        }

        if (dialogueBox != null)
            dialogueBox.SetActive(true);

        PlayCurrentLine();
    }

    // 逐字显示当前行
    private void PlayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        isScrolling = true;
        text.text = "";

        foreach (char c in currentLines[currentLine].ToCharArray())
        {
            text.text += c;
            // 改用实时等待，不受游戏暂停影响
            yield return new WaitForSecondsRealtime(textSpeed);
        }

        isScrolling = false;
    }

    // 点击/按键触发下一句
    public void OnClickNext()
    {
        if (!isDialogActive || isDialogTriggerPaused) return;

        if (isScrolling)
        {
            // 正在打字，直接显示完整文本
            StopAllCoroutines();
            text.text = currentLines[currentLine];
            isScrolling = false;
            return;
        }

        // 切换到下一行
        currentLine++;
        if (currentLine < currentLines.Length)
        {
            PlayCurrentLine();
        }
        else
        {
            EndDialog();
        }
    }

    // 结束对话
    public void EndDialog()
    {
        isDialogActive = false;
        if (dialogueBox != null)
            dialogueBox.SetActive(false);

        currentLines = null;

        // 恢复之前的UI面板
        if (UIPanelManager.instance != null)
        {
            UIPanelManager.instance.ShowNextPanel();
        }
    }

    // 暂停/恢复自动触发
    public void PauseDialogAutoTrigger() => isDialogTriggerPaused = true;
    public void ResumeDialogAutoTrigger() => isDialogTriggerPaused = false;
}