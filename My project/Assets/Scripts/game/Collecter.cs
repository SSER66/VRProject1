using UnityEngine;
// 引入XR交互工具包命名空间
using UnityEngine.XR.Interaction.Toolkit;

public class BasketCollector : MonoBehaviour
{
    [Header("单个水果基础得分")]
    public int scorePerFruit = 10;

    // 水果进入篮子触发器时触发
    void OnTriggerEnter(Collider other)
    {
        // 只识别带有XR Grab Interactable的水果物体
        if (other.GetComponent<XRGrabInteractable>() != null)
        {
            // 加分
            GameManager.Instance.AddScore(scorePerFruit);
            // 销毁水果
            Destroy(other.gameObject);
            // 可选：加收集音效、粒子特效
        }
    }
}
