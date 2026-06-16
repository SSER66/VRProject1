using UnityEngine;
using UnityEngine.XR; // VR输入必须引用的命名空间

public class ShootController : MonoBehaviour
{
    [Header("枪口位置：把场景里的MuzzlePoint拖进来")]
    public Transform muzzlePoint;

    [Header("子弹预制体：拖入Bullet预制体")]
    public GameObject bulletPrefab;

    [Header("子弹飞行速度")]
    public float bulletSpeed = 30f;

    // 代表右手柄设备
    private InputDevice _rightHand;
    // 记录上一帧扳机状态，用于判断「按下瞬间」
    private bool _lastTriggerState;

    void Start()
    {
        // 游戏启动时自动获取右手柄设备
        _rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    void Update()
    {
        // 手柄断开重连后自动重新获取，防止追踪失效
        if (!_rightHand.isValid)
        {
            _rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            return;
        }

        // 读取当前扳机是否按下
        if (_rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool currentTrigger))
        {
            // 当前按下 + 上一帧未按下 = 刚按下的瞬间，只发射一次
            if (currentTrigger && !_lastTriggerState)
            {
                FireBullet();
            }
            // 保存当前状态，供下一帧对比
            _lastTriggerState = currentTrigger;
        }
    }

    // 发射子弹逻辑
    void FireBullet()
    {
        // 在枪口位置生成子弹，方向与枪口朝向一致
        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);

        // 给子弹一个向前的初速度
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = muzzlePoint.forward * bulletSpeed;
        }

        // 2秒后自动销毁子弹，避免场景物体过多
        Destroy(bullet, 2f);
    }
}