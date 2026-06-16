using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("水果预制体数组，可拖入多种水果")]
    public GameObject[] fruitPrefabs;
    [Header("生成间隔（秒）")]
    public float spawnInterval = 2f;
    [Header("左右生成范围（米）")]
    public float spawnRangeX = 1.5f;
    [Header("水果自动销毁时间（秒）")]
    public float destroyTime = 8f;

    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            SpawnFruit();
            _timer = 0;
        }
    }

    void SpawnFruit()
    {
        // 随机选一种水果
        int randomIndex = Random.Range(0, fruitPrefabs.Length);
        // 随机X轴位置
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPos = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z);

        // 生成水果
        GameObject newFruit = Instantiate(fruitPrefabs[randomIndex], spawnPos, Quaternion.identity);
        // 超时自动销毁，防止场景物体过多卡顿
        Destroy(newFruit, destroyTime);
    }
}