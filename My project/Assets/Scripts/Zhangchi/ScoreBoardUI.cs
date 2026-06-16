using TMPro;
using UnityEngine;
using System.Collections;

public class ScoreBoardUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Animation")]
    [SerializeField] private Transform scoreContainer;
    [SerializeField] private float popScale = 1.15f;
    [SerializeField] private float popDuration = 0.15f;

    private Coroutine popCoroutine;

    private void Start()
    {
        if (scoreContainer == null)
            scoreContainer = scoreText.transform;

        Refresh(GameManager.Instance.currentScore);
    }

    public void Refresh(int score)
    {
        scoreText.text = score.ToString("N0");

        if (popCoroutine != null)
            StopCoroutine(popCoroutine);

        popCoroutine = StartCoroutine(PopAnimation());
    }

    private IEnumerator PopAnimation()
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * popScale;

        float timer = 0f;

        // ·Å´ó
        while (timer < popDuration)
        {
            timer += Time.deltaTime;
            float t = timer / popDuration;

            scoreContainer.localScale = Vector3.Lerp(
                originalScale,
                targetScale,
                EaseOutBack(t)
            );

            yield return null;
        }

        timer = 0f;

        // Ëõ»Ø
        while (timer < popDuration)
        {
            timer += Time.deltaTime;
            float t = timer / popDuration;

            scoreContainer.localScale = Vector3.Lerp(
                targetScale,
                originalScale,
                t
            );

            yield return null;
        }

        scoreContainer.localScale = originalScale;
    }

    private float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(x - 1f, 3f)
                     + c1 * Mathf.Pow(x - 1f, 2f);
    }
}