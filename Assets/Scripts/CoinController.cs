using UnityEngine;

public class CoinController : MonoBehaviour
{
    public int value = 1;

    private float lifeTimer;
    private const float LIFE_TIME = 6f;
    private float bobOffset;
    private SpriteRenderer sr;
    private Color baseColor;
    private bool collected;

    // 오브젝트 풀에서 꺼낼 때 초기화
    public void Init(int coinValue, Vector3 spawnPos)
    {
        value       = coinValue;
        lifeTimer   = 0f;
        collected   = false;
        bobOffset   = Random.Range(0f, Mathf.PI * 2f);
        transform.position = spawnPos;
        transform.localScale = Vector3.one;

        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            baseColor = sr.color;
            var c = baseColor; c.a = 1f; sr.color = c;
        }
    }

    void Update()
    {
        if (collected) return;
        lifeTimer += Time.deltaTime;

        // 천천히 위로 부유
        transform.position += new Vector3(0f, 0.35f * Time.deltaTime, 0f);
        // 좌우 bob
        float bob = Mathf.Sin(Time.time * 2.2f + bobOffset) * 0.06f;
        transform.position += new Vector3(bob * Time.deltaTime, 0f, 0f);

        // 마지막 1.5초 페이드
        if (sr != null && lifeTimer > LIFE_TIME - 1.5f)
        {
            float alpha = 1f - (lifeTimer - (LIFE_TIME - 1.5f)) / 1.5f;
            var c = baseColor; c.a = Mathf.Clamp01(alpha); sr.color = c;
        }

        if (lifeTimer >= LIFE_TIME)
            ReturnToPool();
    }

    // 마우스 호버로 습득
    void OnMouseEnter()
    {
        Collect();
    }

    // 클릭도 지원
    void OnMouseDown()
    {
        Collect();
    }

    public void Collect()
    {
        if (collected) return;
        collected = true;
        if (GameManager.Instance != null)
            GameManager.Instance.AddCoins(value);
        ReturnToPool();
    }

    void ReturnToPool()
    {
        collected = true;
        if (GameManager.Instance != null)
            GameManager.Instance.coinPool.Return(this);
        else
            gameObject.SetActive(false);
    }
}
