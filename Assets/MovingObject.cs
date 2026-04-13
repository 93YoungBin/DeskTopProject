using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public enum ObjectType { Crab, Shrimp, Fish }
    public ObjectType objectType;

    public float speed = 2f;
    private Vector2 direction;
    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;
    private float timeOffset;
    private Vector3 baseScale;

    // Fish 전용
    private Vector3 fishTarget;
    private float fishWanderTimer;
    private float fishWanderDur;
    private float fishBobOffset;

    void Start()
    {
        direction = new Vector2(Random.value > 0.5f ? 1 : -1, 0f);
        baseScale = transform.localScale;
        Camera cam = Camera.main;
        screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.transform.position.z));
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            objectWidth  = sr.bounds.size.x / 2;
            objectHeight = sr.bounds.size.y / 2;
        }
        timeOffset = Random.Range(0f, 2f * Mathf.PI);

        // Fish 초기화
        fishBobOffset  = Random.Range(0f, Mathf.PI * 2f);
        fishWanderDur  = Random.Range(2f, 5f);
        FishPickTarget();
    }

    void Update()
    {
        switch (objectType)
        {
            case ObjectType.Crab:   UpdateCrab();   break;
            case ObjectType.Shrimp: UpdateShrimp(); break;
            case ObjectType.Fish:   UpdateFish();   break;
        }
        UpdateFacing();
    }

    void UpdateCrab()
    {
        Vector2 pos = transform.position;
        pos.x += direction.x * speed * Time.deltaTime;
        transform.position = pos;
        BounceWalls();
    }

    void UpdateShrimp()
    {
        Vector2 pos = transform.position;
        float angle = Time.time * speed + timeOffset;
        Vector2 moveDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 0.5f;
        pos += moveDir * Time.deltaTime;
        direction.x = Mathf.Sign(moveDir.x);
        transform.position = pos;
    }

    void UpdateFish()
    {
        // 목표 지점으로 부드럽게 이동
        Vector3 dir = fishTarget - transform.position;
        dir.z = 0f;

        if (dir.magnitude > 0.15f)
            transform.position += dir.normalized * speed * Time.deltaTime;

        // 위아래 너울 bob
        float bob = Mathf.Sin(Time.time * 1.5f + fishBobOffset) * 0.08f;
        transform.position += new Vector3(0f, bob * Time.deltaTime, 0f);

        // 방향 업데이트
        direction.x = dir.x >= 0f ? 1f : -1f;

        // 목표 재설정
        fishWanderTimer += Time.deltaTime;
        if (fishWanderTimer >= fishWanderDur || dir.magnitude < 0.2f)
        {
            fishWanderTimer = 0f;
            fishWanderDur   = Random.Range(2f, 6f);
            FishPickTarget();
        }
    }

    void FishPickTarget()
    {
        float margin = 1.2f;
        float px = Random.Range(-screenBounds.x + margin, screenBounds.x - margin);
        float py = Random.Range(-screenBounds.y + margin, screenBounds.y - margin);
        fishTarget = new Vector3(px, py, 0f);
    }

    void BounceWalls()
    {
        Vector2 pos = transform.position;
        if (pos.x > screenBounds.x - objectWidth || pos.x < -screenBounds.x + objectWidth)
            direction.x = -direction.x;
        pos.x = Mathf.Clamp(pos.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
        pos.y = Mathf.Clamp(pos.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);
        transform.position = pos;
    }

    void UpdateFacing()
    {
        if (direction.x == 0) return;
        Vector3 scale = baseScale;
        scale.x = Mathf.Abs(baseScale.x) * (direction.x < 0 ? 1 : -1);
        transform.localScale = scale;
    }
}
