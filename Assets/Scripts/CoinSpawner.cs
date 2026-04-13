using UnityEngine;

/// <summary>
/// MovingObject(Fish) 위에 얹어서 코인을 주기적으로 뱉는 컴포넌트
/// </summary>
[RequireComponent(typeof(MovingObject))]
public class CoinSpawner : MonoBehaviour
{
    public float coinInterval = 10f;   // 코인 뱉는 주기 (초)
    public int   coinValue    = 1;     // 코인 가치

    private float timer;

    void Start()
    {
        // 물고기마다 타이머 시작 시점 분산
        timer = Random.Range(0f, coinInterval);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= coinInterval)
        {
            timer = 0f;
            SpawnCoin();
        }
    }

    void SpawnCoin()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SpawnCoin(transform.position, coinValue);
    }
}
