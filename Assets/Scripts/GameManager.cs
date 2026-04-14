using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Coins")]
    public int totalCoins = 0;
    public Text coinText;

    [Header("Fish Limit")]
    public int maxFish = 5;
    public int currentFish = 0;
    public Text fishCountText; // 현재/최대 표시 (선택)

    [Header("Pool")]
    public CoinPool coinPool;

    [Header("Fish Sprites")]
    public Sprite[] fishSprites; // 0=새우(_876) 1=게(_01) 2=새우2(_02)

    [Header("Fish Tank")]
    public Transform fishTank; // FishTank 오브젝트

    [Header("Shop UI")]
    public GameObject shopPanel;
    public Transform  shopContent;
    public GameObject shopItemPrefab;

    // ── 상점 아이템 종류 ──────────────────────────
    public enum ShopCategory { Fish, Upgrade }

    [System.Serializable]
    public class ShopEntry
    {
        public ShopCategory category;
        // Fish 전용
        public string  displayName;
        public string  description;
        public int     cost;
        public int     spriteIndex;
        public float   speed;
        public float   scale;
        public float   coinInterval;
        public int     coinValue;
        public MovingObject.ObjectType moveType;
        // Upgrade 전용
        public int     slotIncrease;  // maxFish 증가량
    }

    public List<ShopEntry> shopList = new List<ShopEntry>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        BuildShopList();
        // 현재 씬에 이미 있는 생물 수 카운트
        if (fishTank != null) currentFish = fishTank.childCount;
    }

    void Start()
    {
        UpdateCoinUI();
        UpdateFishCountUI();
        if (shopPanel) shopPanel.SetActive(false);
        BuildShopUI();
    }

    void BuildShopList()
    {
        shopList.Clear();

        // ── 생물 ─────────────────────────────────
        shopList.Add(new ShopEntry { category=ShopCategory.Fish,
            displayName="새우",        description="무료! 기본 새우",        cost=0,   spriteIndex=0,
            speed=1.5f, scale=2f,   coinInterval=15f, coinValue=1, moveType=MovingObject.ObjectType.Shrimp });

        shopList.Add(new ShopEntry { category=ShopCategory.Fish,
            displayName="게",          description="10코인 / 옆으로 걷기",   cost=10,  spriteIndex=1,
            speed=1.2f, scale=2.5f, coinInterval=12f, coinValue=2, moveType=MovingObject.ObjectType.Crab });

        shopList.Add(new ShopEntry { category=ShopCategory.Fish,
            displayName="보라 새우",   description="25코인 / 코인 더 많이!", cost=25,  spriteIndex=2,
            speed=1.8f, scale=2f,   coinInterval=9f,  coinValue=3, moveType=MovingObject.ObjectType.Shrimp });

        shopList.Add(new ShopEntry { category=ShopCategory.Fish,
            displayName="빠른 새우",   description="50코인 / 스피드!",       cost=50,  spriteIndex=0,
            speed=3.2f, scale=1.5f, coinInterval=6f,  coinValue=5, moveType=MovingObject.ObjectType.Shrimp });

        shopList.Add(new ShopEntry { category=ShopCategory.Fish,
            displayName="황금 게",     description="100코인 / 럭셔리!",      cost=100, spriteIndex=1,
            speed=2f,   scale=3.5f, coinInterval=4f,  coinValue=8, moveType=MovingObject.ObjectType.Crab });

        // ── 업그레이드 ───────────────────────────
        shopList.Add(new ShopEntry { category=ShopCategory.Upgrade,
            displayName="어항 확장 +1", description="수용 가능 생물 +1마리", cost=30,  slotIncrease=1 });

        shopList.Add(new ShopEntry { category=ShopCategory.Upgrade,
            displayName="대형 어항 +3", description="수용 가능 생물 +3마리", cost=75,  slotIncrease=3 });
    }

    // ─── Fish 스폰 ────────────────────────────────
    public bool CanSpawnFish() => currentFish < maxFish;

    public void SpawnFishFromShop(ShopEntry data)
    {
        if (!CanSpawnFish()) return;

        Camera cam = Camera.main;
        float h = 2f * cam.orthographicSize;
        float w = h * cam.aspect;
        float x = Random.Range(-w * 0.4f, w * 0.4f);
        float y = Random.Range(-h * 0.3f, h * 0.3f);

        var go = new GameObject(data.displayName);
        go.transform.position   = new Vector3(x, y, 0f);
        go.transform.localScale = new Vector3(data.scale, data.scale, 1f);

        // FishTank 하위로
        if (fishTank != null) go.transform.SetParent(fishTank, true);

        var sr = go.AddComponent<SpriteRenderer>();
        if (fishSprites != null && data.spriteIndex < fishSprites.Length)
            sr.sprite = fishSprites[data.spriteIndex];

        var mo = go.AddComponent<MovingObject>();
        mo.objectType = data.moveType;
        mo.speed = data.speed;

        var cs = go.AddComponent<CoinSpawner>();
        cs.coinInterval = data.coinInterval;
        cs.coinValue    = data.coinValue;

        currentFish++;
        UpdateFishCountUI();
    }

    // ─── 업그레이드 ───────────────────────────────
    public void ApplyUpgrade(ShopEntry data)
    {
        maxFish += data.slotIncrease;
        UpdateFishCountUI();
        BuildShopUI(); // 버튼 상태 갱신
    }

    // ─── 코인 ─────────────────────────────────────
    public void SpawnCoin(Vector3 pos, int val)
    {
        if (coinPool == null) return;
        coinPool.Get(val, pos + new Vector3(0f, 0.3f, 0f));
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateCoinUI();
    }

    // ─── UI ───────────────────────────────────────
    void UpdateCoinUI()
    {
        if (coinText) coinText.text = "💰 " + totalCoins;
    }

    void UpdateFishCountUI()
    {
        if (fishCountText) fishCountText.text = string.Format("🐠 {0}/{1}", currentFish, maxFish);
    }

    public void ToggleShop()
    {
        if (shopPanel) shopPanel.SetActive(!shopPanel.activeSelf);
    }

    void BuildShopUI()
    {
        if (!shopContent || !shopItemPrefab) return;
        foreach (Transform ch in shopContent) Destroy(ch.gameObject);
        foreach (var entry in shopList)
        {
            var item = Instantiate(shopItemPrefab, shopContent);
            var si   = item.GetComponent<ShopItem>();
            if (si) si.Setup(entry, this);
        }
    }

    // ─── 구매 진입점 ──────────────────────────────
    public bool TryBuy(ShopEntry entry)
    {
        if (totalCoins < entry.cost) return false;

        if (entry.category == ShopCategory.Fish && !CanSpawnFish()) return false;

        totalCoins -= entry.cost;
        UpdateCoinUI();

        if (entry.category == ShopCategory.Fish)
            SpawnFishFromShop(entry);
        else
            ApplyUpgrade(entry);

        return true;
    }
}
