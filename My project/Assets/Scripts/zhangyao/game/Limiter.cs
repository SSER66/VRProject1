using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// 可抓取物体距离限制器：超出距离后禁用抓取，引导用户靠近
/// 挂载到带有 XRGrabInteractable 的水果预制体上
/// </summary>
public class GrabbableDistanceLimiter : MonoBehaviour
{
    [Header("抓取设置")]
    [Tooltip("最大可抓取距离（米），超出后无法抓取")]
    public float maxGrabDistance = 1.0f;

    [Tooltip("距离计算模式")]
    public DistanceMode distanceMode = DistanceMode.ThreeDimensional;

    [Header("视觉反馈")]
    [Tooltip("是否开启状态变色提示")]
    public bool enableVisualFeedback = true;
    [Tooltip("不可抓取时的颜色（半透明灰）")]
    public Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);

    [Header("震动反馈")]
    [Tooltip("进入可抓取范围时是否震动手柄提示")]
    public bool enableHaptic = true;
    [Tooltip("震动强度 0~1")]
    public float hapticStrength = 0.2f;
    [Tooltip("震动时长（秒）")]
    public float hapticDuration = 0.1f;

    // 内部组件引用
    private XRGrabInteractable _grabInteractable;
    private Renderer _objectRenderer;
    private Color _originalColor;
    private Transform _headTransform;
    private bool _wasInRange = false;

    public enum DistanceMode
    {
        ThreeDimensional, // 三维直线距离（上下左右前后都算）
        HorizontalOnly    // 仅水平距离（忽略高度差，适合引导左右侧身）
    }

    void Awake()
    {
        // 自动获取自身组件，不用手动拖
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _objectRenderer = GetComponent<Renderer>();

        // 缓存原始颜色
        if (_objectRenderer != null)
        {
            _originalColor = _objectRenderer.material.color;
        }

        // 自动找头显相机（XR Origin 下的 Main Camera）
        _headTransform = Camera.main.transform;

        // 初始默认禁用抓取，等进入范围再开启
        if (_grabInteractable != null)
        {
            _grabInteractable.enabled = false;
            UpdateVisualState(false);
        }
    }

    void Update()
    {
        if (_grabInteractable == null || _headTransform == null) return;

        // 计算当前距离
        float currentDistance = CalculateDistance();
        bool isInRange = currentDistance <= maxGrabDistance;

        // 状态变化时切换抓取权限
        if (isInRange != _wasInRange)
        {
            _grabInteractable.enabled = isInRange;
            UpdateVisualState(isInRange);

            // 刚进入范围时触发手柄震动提示
            if (isInRange && enableHaptic)
            {
                TriggerHapticFeedback();
            }

            _wasInRange = isInRange;
        }
    }

    // 计算距离
    float CalculateDistance()
    {
        Vector3 objPos = transform.position;
        Vector3 headPos = _headTransform.position;

        if (distanceMode == DistanceMode.HorizontalOnly)
        {
            // 水平距离：忽略Y轴高度
            objPos.y = 0;
            headPos.y = 0;
        }

        return Vector3.Distance(objPos, headPos);
    }

    // 更新视觉状态
    void UpdateVisualState(bool canGrab)
    {
        if (!enableVisualFeedback || _objectRenderer == null) return;
        _objectRenderer.material.color = canGrab ? _originalColor : disabledColor;
    }

    // 手柄震动反馈（双手柄都触发，提示用户附近有可抓取物体）
    void TriggerHapticFeedback()
    {
        // 左手柄震动
        if (UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand)
            .TryGetHapticCapabilities(out var leftCap) && leftCap.numChannels > 0)
        {
            UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand)
                .SendHapticImpulse(0, hapticStrength, hapticDuration);
        }

        // 右手柄震动
        if (UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand)
            .TryGetHapticCapabilities(out var rightCap) && rightCap.numChannels > 0)
        {
            UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand)
                .SendHapticImpulse(0, hapticStrength, hapticDuration);
        }
    }

    // Scene视图绘制范围球，方便调试（运行时也可见）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxGrabDistance);
    }
}
