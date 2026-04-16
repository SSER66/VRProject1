using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    [Header("基础UI")]
    public GameObject dialogueBox;
    public TMP_Text text;

    [Header("文字滚动")]
    public float textSpeed = 0.05f;

    [Header("VR自动开场对话")]
    public bool autoStartDialog = true;
    [TextArea(1, 3)] public string[] startDialogLines;

    [Header("你的2D宠物")]
    public FaceExpressionController faceCtrl;
    public Animator catAnimator; // 拖入猫咪的Animator组件

    private string[] currentLines;
    private int currentLine;
    private bool isScrolling;
    private bool isDialogActive;

    // 口型配置
    private string[] lipShapes = { "a_1", "a_2", "a_3", "a_2" };
    private string defaultMouth = "a_1";
    private Coroutine lipCoroutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        dialogueBox.SetActive(false);
        // 初始化：默认不在说话状态
        if (catAnimator != null)
        {
            catAnimator.SetBool("IsTalking", false);
        }

        // 进入VR自动播放开场对话
        if (autoStartDialog && startDialogLines != null && startDialogLines.Length > 0)
        {
            ShowDialog(startDialogLines);
        }
    }

    private void Update()
    {
        if (!isDialogActive) return;

        // PICO扳机键触发下一句
        bool triggerDown = CheckPicoTriggerDown();

        if (triggerDown)
        {
            if (!isScrolling)
            {
                NextLine();
            }
            else
            {
                // 正在打字 → 直接显示完整句子
                StopAllCoroutines();
                text.text = currentLines[currentLine];
                isScrolling = false;
                StopLipSync();
            }
        }
    }

    // 检测PICO扳机键（兼容所有PICO设备）
    private bool CheckPicoTriggerDown()
    {
        if (Input.GetButtonDown("XR Left Trigger")) return true;
        if (Input.GetButtonDown("XR Right Trigger")) return true;
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) return true;
        return false;
    }

    // 播放对话
    public void ShowDialog(string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        currentLines = lines;
        currentLine = 0;
        isDialogActive = true;
        dialogueBox.SetActive(true);

        // 关键：对话开始时，切换到说话动作
        if (catAnimator != null)
        {
            catAnimator.SetBool("IsTalking", true);
        }

        PlayCurrentLine();
    }

    // 下一句
    private void NextLine()
    {
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

    // 播放当前行文字 + 口型动画
    private void PlayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
        StartLipSync();
    }

    // 打字机效果
    private IEnumerator TypeLine()
    {
        isScrolling = true;
        text.text = "";

        foreach (char c in currentLines[currentLine].ToCharArray())
        {
            text.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isScrolling = false;
        StopLipSync();
    }

    // 结束对话
    private void EndDialog()
    {
        isDialogActive = false;
        dialogueBox.SetActive(false);
        StopLipSync();

        // 关键：对话结束时，切回待机动作
        if (catAnimator != null)
        {
            catAnimator.SetBool("IsTalking", false);
        }

        if (faceCtrl != null)
        {
            faceCtrl.selectedMouth = defaultMouth;
            faceCtrl.ApplyAllExpressions();
        }
    }

    // 口型同步
    private void StartLipSync()
    {
        if (faceCtrl == null) return;
        if (lipCoroutine != null) StopCoroutine(lipCoroutine);
        lipCoroutine = StartCoroutine(LipSyncRoutine());
    }

    private void StopLipSync()
    {
        if (lipCoroutine != null)
            StopCoroutine(lipCoroutine);

        if (faceCtrl != null)
        {
            faceCtrl.selectedMouth = defaultMouth;
            faceCtrl.ApplyAllExpressions();
        }
    }

    private IEnumerator LipSyncRoutine()
    {
        int index = 0;
        while (isScrolling)
        {
            faceCtrl.selectedMouth = lipShapes[index];
            faceCtrl.ApplyAllExpressions();
            index = (index + 1) % lipShapes.Length;
            yield return new WaitForSeconds(0.12f);
        }
    }
}