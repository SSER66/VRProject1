using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int currentScore = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 初始化UI
            if (UIManager.Instance != null)
                UIManager.Instance.InitializeScore();
        }
        else
            Destroy(gameObject);
    }

    public void AddScore(int value)
    {
        currentScore += value;
        // 同步更新UI
        UIManager.Instance.UpdateScore(currentScore);
    }
}