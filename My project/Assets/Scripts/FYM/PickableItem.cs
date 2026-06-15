using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 可拾取物体标识 – 挂在地面掉落的物体上
/// </summary>
public class PickableItem : MonoBehaviour
{
    [Header("拾取后事件（可选）")]
    public UnityEvent onPickup;   // 可在 Inspector 中绑定其他逻辑

    /// <summary>
    /// 当物体被玩家拾取时调用（由 AutoPickupByRange 触发）
    /// </summary>
    public void OnPickup()
    {
        onPickup?.Invoke();
        // 你也可以在这里自定义逻辑，比如加分
    }
}
