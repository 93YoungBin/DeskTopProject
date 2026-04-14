using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public enum ObjectType { Crab, Shrimp, Fish }
    public ObjectType objectType;
    public float speed = 2f;

    private Vector2 direction;
    private float   timeOffset;
    private Vector3 baseScale;

    // Fish 전용
    private Vector3 fishTarget;
    private float   fishWanderTimer;
    private float   fishWanderDur;
    private float   fishBobOffset;

    // 경계 (매 프레임 갱신)
    private Camera  cam;
    private float   halfW, halfH;
    private float   extX,  extY;   // 오브젝트 절반 크기 (스케일 포함)

    void Start()
    {
        cam       = Camera.main;
        baseScale = transform.localScale;
        direction = new Vector2(Random.value > 0.5f ? 1 : -1, 0f);
        timeOffset    = Random.Range(0f, 2f * Mathf.PI);
        fishBobOffset = Random.Range(0f, Mathf.PI * 2f);
        fishWanderDur = Random.Range(2f, 5f);
        RefreshBounds();
        FishPickTarget();
    }

    void RefreshBounds()
    {
        if (cam == null) return;
        halfH = cam.orthographicSize;
        halfW = halfH * cam.aspect;

        // 스프라이트 렌더러 기준 오브젝트 절반 크기
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            extX = sr.bounds.extents.x;
            extY = sr.bounds.extents.y;
        }
        else
        {
            extX = extY = 0.3f;
        }
    }

    void Update()
    {
        RefreshBounds();
        switch (objectType)
        {
            case ObjectType.Crab:   UpdateCrab();   break;
            case ObjectType.Shrimp: UpdateShrimp(); break;
            case ObjectType.Fish:   UpdateFish();   break;
        }
        ClampToScreen();
        UpdateFacing();
    }

    // ── 이동 ────────────────────────────────────
    void UpdateCrab()
    {
        Vector3 pos = transform.position;
        pos.x += direction.x * speed * Time.deltaTime;
        transform.position = pos;

        // 벽에 닿으면 반전
        if (pos.x >= halfW - extX || pos.x <= -halfW + extX)
            direction.x = -direction.x;
    }

    void UpdateShrimp()
    {
        float angle = Time.time * speed + timeOffset;
        Vector2 moveDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle * 0.7f)) * 0.5f;
        transform.position += (Vector3)(moveDir * Time.deltaTime);
        direction.x = Mathf.Sign(moveDir.x);
    }

    void UpdateFish()
    {
        Vector3 dir = fishTarget - transform.position;
        dir.z = 0f;

        if (dir.magnitude > 0.15f)
            transform.position += dir.normalized * speed * Time.deltaTime;

        float bob = Mathf.Sin(Time.time * 1.5f + fishBobOffset) * 0.08f;
        transform.position += new Vector3(0f, bob * Time.deltaTime, 0f);

        direction.x = dir.x >= 0f ? 1f : -1f;

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
        // 오브젝트 크기를 고려한 안전 범위 안에서 목표 선택
        float safeW = Mathf.Max(0f, halfW - extX - 0.1f);
        float safeH = Mathf.Max(0f, halfH - extY - 0.1f);
        fishTarget = new Vector3(
            Random.Range(-safeW, safeW),
            Random.Range(-safeH, safeH),
            0f);
    }

    // ── 화면 범위 강제 클램프 ───────────────────
    void ClampToScreen()
    {
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, -halfW + extX, halfW - extX);
        p.y = Mathf.Clamp(p.y, -halfH + extY, halfH - extY);
        transform.position = p;
    }

    // ── 좌우 플립 ───────────────────────────────
    void UpdateFacing()
    {
        if (direction.x == 0f) return;
        Vector3 s = baseScale;
        s.x = Mathf.Abs(s.x) * (direction.x < 0f ? 1f : -1f);
        transform.localScale = s;
    }
}
