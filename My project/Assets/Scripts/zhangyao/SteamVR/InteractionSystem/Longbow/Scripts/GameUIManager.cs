using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public int time = 100;
    private Text txt_Score, txt_Time;
    private int score = 0;
    private float timer = 0;
    private GameObject GameOverPanel;
    public static GameUIManager Instance;
    public bool gameover = false;
    
    private void Awake()
    {
        Instance = this;
        txt_Score = transform.Find("txt_Score").GetComponent<Text>();
        txt_Time = transform.Find("txt_Time").GetComponent<Text>();
        txt_Score.text = "Score:" + 0;
        txt_Time.text = "Time:" + time;
        GameOverPanel = transform.Find("GameOverPanel").gameObject;
        GameOverPanel.SetActive(false);

        GameOverPanel.transform.Find("btn_Restart").GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
        GameOverPanel.transform.Find("btn_Menu").GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Start");
        });
    }
    public void AddScore(int value = 1)
    {
        score++;
        txt_Score.text = "Score:" + score;
    }
    private void Update()
    {
        if (time <= 0)
        {
            GameOver();
            return;
        }
        timer += Time.deltaTime;
        if (timer >= 1)
        {
            timer = 0;
            time--;
            txt_Time.text = "Time:" + time;
        }
    }
    /// <summary>
    /// 游戏结束
    /// </summary>
    private void GameOver()
    {
        if (gameover) return;

        gameover = true;
        //释放弓和箭
        GameObject.FindObjectOfType<ItemPackageSpawner>().DetachAllObject(); GameOverPanel.SetActive(true);
        //可以激活射线了
        foreach (var item in GameObject.FindObjectsOfType<SteamVR_LaserPointer>())
        {
            item.isDefaultActivePointer = true;
        }
    }
}
