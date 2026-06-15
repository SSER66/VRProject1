using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VRLocomotionController : MonoBehaviour
{
    [Header("输入动作引用 (Input Action References)")]
    // 在Inspector中拖入左手柄的摇杆动作 (通常是 XRI LeftHand/Move)
    public InputActionReference moveAction;

    // 在Inspector中拖入右手柄的摇杆动作 (通常是 XRI RightHand/Move)
    public InputActionReference turnAction;

    [Header("移动参数")]
    public float moveSpeed = 2.0f; // 移动速度
    public float turnSpeed = 90.0f; // 转向速度 (度/秒)

    private Transform cameraOffset; // 用于旋转的节点 (Camera Offset)
    private Transform xrOriginTransform; // 用于移动的节点 (XR Origin 根节点)

    void Start()
    {
        // 获取组件引用
        xrOriginTransform = this.transform; // 脚本挂在 XR Origin 上，this.transform 就是根节点

        // 找到 Camera Offset 子物体
        cameraOffset = xrOriginTransform.Find("Camera Offset");
        if (cameraOffset == null)
        {
            Debug.LogError("找不到 Camera Offset 子物体，请检查层级结构！");
        }

        // 启用输入动作
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Enable();

        if (turnAction != null && turnAction.action != null)
            turnAction.action.Enable();
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (moveAction == null || moveAction.action == null) return;

        // 读取左手柄摇杆值 (Vector2: x为左右, y为前后)
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // 防止死区抖动
        if (input.magnitude < 0.1f) return;

        // 获取主相机的朝向（作为玩家的前方）
        Transform cameraTransform = Camera.main.transform;

        // 计算移动方向：只在地面移动，忽略Y轴高度
        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0;
        right.Normalize();

        // 结合摇杆的X和Y计算最终方向
        Vector3 direction = (forward * input.y + right * input.x).normalized;

        // 移动 XR Origin 根节点
        xrOriginTransform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        if (turnAction == null || turnAction.action == null) return;

        // 读取右手柄摇杆值
        Vector2 input = turnAction.action.ReadValue<Vector2>();

        // 通常使用摇杆的 X 轴（左右拨动）来控制转向
        float turnInput = input.x;

        if (Mathf.Abs(turnInput) < 0.1f) return;

        // 旋转 Camera Offset 节点 (只旋转 Y 轴)
        // 这样头显会跟随身体转动
        float rotationAmount = turnInput * turnSpeed * Time.deltaTime;
        cameraOffset.Rotate(Vector3.up, rotationAmount);
    }

    void OnDestroy()
    {
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Disable();
        if (turnAction != null && turnAction.action != null)
            turnAction.action.Disable();
    }
}