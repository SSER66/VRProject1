using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// 树上果实：生成时固定悬挂，被抓取后解锁物理
/// 挂载到水果预制体上
/// </summary>
public class FruitOnTree : MonoBehaviour
{
    [Header("分值设置")]
    public int scoreValue = 10;

    private Rigidbody _rb;
    private XRGrabInteractable _grabInteractable;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _grabInteractable = GetComponent<XRGrabInteractable>();

        // 初始状态：运动学固定，不受重力，挂在树上
        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }

        // 监听抓取事件：被抓起时解锁物理
        if (_grabInteractable != null)
        {
            _grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
    }

    // 被玩家抓取时触发
    void OnGrabbed(SelectEnterEventArgs args)
    {
        // 解锁刚体，恢复正常物理
        if (_rb != null)
        {
            _rb.isKinematic = false;
            _rb.useGravity = true;
        }

        // 只触发一次，抓取后移除监听
        if (_grabInteractable != null)
        {
            _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        }
    }
}

