using UnityEngine;
using System.Collections;
using TMPro;

// 专为你的 Tomatocol 2D宠物 写的口型同步
public class PetLipSync : MonoBehaviour
{
    [Header("===== 你的2D宠物控制器 =====")]
    public FaceExpressionController faceCtrl;

    [Header("===== 对话框文本 =====")]
    public TextMeshProUGUI dialogueText;

    [Header("===== 说话速度 =====")]
    [Tooltip("文字出现速度")] public float textSpeed = 0.05f;
    [Tooltip("口型切换速度")] public float lipSpeed = 0.12f;

    // 你角色自带的嘴巴形状（直接用插件自带的）
    private string[] talkingMouths = { "a_1", "a_2", "a_3", "a_2" };
    private string defaultMouth = "a_1"; // 闭嘴/默认嘴型

    private bool isTalking = false;

    // ======================
    // 外部调用：让宠物说话
    // ======================
    public void Say(string line)
    {
        if (isTalking) return;
        StopAllCoroutines();
        StartCoroutine(TypeText(line));
        StartCoroutine(LipAnimation());
    }

    // 打字机效果
    IEnumerator TypeText(string line)
    {
        isTalking = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTalking = false;
        faceCtrl.selectedMouth = defaultMouth;
        faceCtrl.ApplyAllExpressions();
    }

    // 自然口型动画（循环切换嘴巴）
    IEnumerator LipAnimation()
    {
        int index = 0;
        while (isTalking)
        {
            faceCtrl.selectedMouth = talkingMouths[index];
            faceCtrl.ApplyAllExpressions();

            index = (index + 1) % talkingMouths.Length;
            yield return new WaitForSeconds(lipSpeed);
        }

        // 说话结束 → 恢复闭嘴
        faceCtrl.selectedMouth = defaultMouth;
        faceCtrl.ApplyAllExpressions();
    }
}