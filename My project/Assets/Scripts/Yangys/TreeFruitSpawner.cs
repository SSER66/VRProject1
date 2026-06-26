using UnityEngine;
using System.Collections.Generic;

public class TreeFruitSpawner : MonoBehaviour
{
    [Header("水果预制体数组")]
    public GameObject[] fruitPrefabs;

    [Header("树上挂果锚点数组（拖入所有SpawnPoint）")]
    public Transform[] fruitSpawnPoints;

    [Header("生成间隔（秒）")]
    public float spawnInterval = 3f;

    [Header("树上同时存在的最大果实数量")]
    public int maxFruitsOnTree = 6;

    [Header("生成位置随机偏移（米），避免完全对齐")]
    public float randomOffset = 0.1f;

    // 记录当前已占用的锚点，避免同一个点重复生成
    private List<int> _occupiedIndexes = new List<int>();
    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= spawnInterval && _occupiedIndexes.Count < maxFruitsOnTree)
        {
            SpawnFruit();
            _timer = 0;
        }
    }

    void SpawnFruit()
    {
        // 随机选一个未被占用的锚点
        List<int> freeIndexes = new List<int>();
        for (int i = 0; i < fruitSpawnPoints.Length; i++)
        {
            if (!_occupiedIndexes.Contains(i))
                freeIndexes.Add(i);
        }

        if (freeIndexes.Count == 0) return;

        int randomIndex = freeIndexes[Random.Range(0, freeIndexes.Count)];
        Transform spawnPoint = fruitSpawnPoints[randomIndex];

        // 随机偏移位置，更自然
        Vector3 offset = new Vector3(
            Random.Range(-randomOffset, randomOffset),
            Random.Range(-randomOffset, randomOffset),
            Random.Range(-randomOffset, randomOffset)
        );
        Vector3 spawnPos = spawnPoint.position + offset;

        // 随机选一种水果生成
        int randomFruit = Random.Range(0, fruitPrefabs.Length);
        GameObject newFruit = Instantiate(fruitPrefabs[randomFruit], spawnPos, Quaternion.identity);

        // 标记锚点已占用
        _occupiedIndexes.Add(randomIndex);

        // 水果被销毁时，释放锚点
        FruitOnTree fruit = newFruit.GetComponent<FruitOnTree>();
        StartCoroutine(WaitForFruitDestroy(newFruit, randomIndex));
    }

    // 等待水果销毁后释放锚点
    System.Collections.IEnumerator WaitForFruitDestroy(GameObject fruit, int pointIndex)
    {
        while (fruit != null)
        {
            yield return null;
        }
        _occupiedIndexes.Remove(pointIndex);
    }
}

