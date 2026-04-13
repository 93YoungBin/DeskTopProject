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
    }

    void Update()
    {
        Vector2 pos = transform.position;
        switch (objectType)
        {
            case ObjectType.Crab:
                pos.x += direction.x * speed * Time.deltaTime;
                break;
            case ObjectType.Shrimp:
                float angle = Time.time * speed + timeOffset;
                Vector2 moveDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 0.5f;
                pos += moveDir * Time.deltaTime;
                direction.x = Mathf.Sign(moveDir.x);
                break;
            case ObjectType.Fish:
                float waveY = Mathf.Sin(Time.time * 2f + timeOffset) * 0.5f;
                pos += new Vector2(direction.x * speed * Time.deltaTime, waveY * Time.deltaTime);
                break;
        }
        transform.position = pos;
        BounceWalls();
        UpdateFacing();
    }

    void BounceWalls()
    {
        Vector2 pos = transform.position;
        if (objectType == ObjectType.Crab || objectType == ObjectType.Fish)
        {
            if (pos.x > screenBounds.x - objectWidth || pos.x < -screenBounds.x + objectWidth)
                direction.x = -direction.x;
            pos.x = Mathf.Clamp(pos.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
            pos.y = Mathf.Clamp(pos.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);
            transform.position = pos;
        }
    }

    void UpdateFacing()
    {
        if (direction.x == 0) return;
        Vector3 scale = baseScale;
        scale.x = Mathf.Abs(baseScale.x) * (direction.x < 0 ? 1 : -1);
        transform.localScale = scale;
    }
}
