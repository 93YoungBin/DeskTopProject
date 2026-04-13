using UnityEngine;

public class CoinController : MonoBehaviour
{
    public int value = 1;
    private float lifeTimer = 0f;
    private float lifeTime = 8f;
    private float riseSpeed = 0.4f;
    private float bobOffset;
    private Vector3 startPos;
    private SpriteRenderer sr;
    private bool collected = false;
    private GameManager gm;

    void Start()
    {
        gm = GameManager.Instance;
        bobOffset = Random.Range(0f, Mathf.PI * 2f);
        startPos = transform.position;
        sr = GetComponent<SpriteRenderer>();
        if (!sr) sr = gameObject.AddComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (collected) return;
        lifeTimer += Time.deltaTime;

        // Rise slowly
        transform.position += new Vector3(0f, riseSpeed * Time.deltaTime, 0f);
        float bob = Mathf.Sin(Time.time * 2f + bobOffset) * 0.05f;
        transform.position += new Vector3(bob * Time.deltaTime, 0f, 0f);

        // Fade near end
        if (sr && lifeTimer > lifeTime - 2f)
        {
            float alpha = 1f - (lifeTimer - (lifeTime - 2f)) / 2f;
            Color c = sr.color; c.a = alpha; sr.color = c;
        }

        if (lifeTimer >= lifeTime)
            Destroy(gameObject);
    }

    void OnMouseDown()
    {
        Collect();
    }

    public void Collect()
    {
        if (collected) return;
        collected = true;
        if (gm) gm.AddCoins(value);
        Destroy(gameObject);
    }
}
