using UnityEngine;
using System.Collections.Generic;

public class CoinPool : MonoBehaviour
{
    public GameObject coinPrefab;
    public int initialSize = 20;

    private Queue<CoinController> pool = new Queue<CoinController>();

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
            CreateOne();
    }

    CoinController CreateOne()
    {
        var go = Instantiate(coinPrefab, transform); // 풀 오브젝트 하위에
        go.SetActive(false);
        var cc = go.GetComponent<CoinController>();
        pool.Enqueue(cc);
        return cc;
    }

    public CoinController Get(int value, Vector3 pos)
    {
        CoinController cc;
        if (pool.Count > 0)
            cc = pool.Dequeue();
        else
            cc = CreateOne(); // 풀 소진 시 확장

        cc.gameObject.SetActive(true);
        cc.Init(value, pos);
        return cc;
    }

    public void Return(CoinController cc)
    {
        cc.gameObject.SetActive(false);
        pool.Enqueue(cc);
    }
}
