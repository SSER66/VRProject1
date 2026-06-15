using UnityEngine;

/// <summary>
/// 自动拾取：当可拾取物体进入半径 1 米的球形触发器时自动拾取
/// 挂载在玩家身上的一个子物体（SphereCollider 触发器）上
/// </summary>
public class AutoPickupByRange : MonoBehaviour
{
    [Header("拾取设置")]
    public LayerMask pickupLayer = 1 << 6;  // 假设第六层是 Pickup，在 Inspector 中手动选

    [Header("视听反馈（可选）")]
    public AudioClip pickupSound;
    public GameObject pickupEffect;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && pickupSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查层级是否匹配
        if (((1 << other.gameObject.layer) & pickupLayer) != 0)
        {
            // 获取可拾取组件
            PickableItem item = other.GetComponent<PickableItem>();
            if (item != null)
            {
                Pickup(item);
            }
        }
    }

    private void Pickup(PickableItem item)
    {
        // 反馈效果
        if (pickupSound != null && audioSource != null)
            audioSource.PlayOneShot(pickupSound);
        if (pickupEffect != null)
            Instantiate(pickupEffect, item.transform.position, Quaternion.identity);

        // 调用物品自身的拾取逻辑（加分、记录等）
        item.OnPickup();

        // 销毁物体
        Destroy(item.gameObject);
    }
}