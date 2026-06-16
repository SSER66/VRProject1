using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BasketCollector : MonoBehaviour
{
    public int scorePerFruit = 10;

    private void OnTriggerEnter(Collider other)
    {
        XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            // 先销毁水果，保证核心玩法正常
            Destroy(other.gameObject);

            // 加分前做空值判断，避免空引用报错打断逻辑
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scorePerFruit);
            }
            else
            {
                Debug.LogWarning("GameManager不存在，已跳过加分，水果已正常销毁");
            }
        }
    }
}