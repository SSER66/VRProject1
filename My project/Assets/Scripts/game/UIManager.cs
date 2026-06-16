using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("积分显示文本")]
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 更新积分显示
    /// </summary>
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
            // 可选：添加数字跳动动画效果（推荐搭配DOTween或LeanTween）
            // scoreText.transform.localScale = Vector3.one * 1.2f;
            // LeanTween.scale(scoreText.gameObject, Vector3.one, 0.3f).setEaseOutBack();
        }
    }

    /// <summary>
    /// 初始化时显示0分
    /// </summary>
    public void InitializeScore()
    {
        if (scoreText != null)
            scoreText.text = "0";
    }
}