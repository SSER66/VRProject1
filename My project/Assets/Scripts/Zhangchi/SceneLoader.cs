using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 如果你需要在脚本中直接获取 Button 组件，可以保留

/// <summary>
/// 挂载到任意游戏物体上，通过按钮的 OnClick 事件调用 LoadScene 方法，即可跳转场景。
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Tooltip("在 Inspector 中填写要加载的场景名称")]
    public string sceneName;

    /// <summary>
    /// 供按钮 OnClick 事件调用的公开方法
    /// </summary>
    public void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("场景名称为空，请在 Inspector 中填写 sceneName！");
            return;
        }

        // 使用 SceneManager 加载场景
        SceneManager.LoadScene(sceneName);
    }
}