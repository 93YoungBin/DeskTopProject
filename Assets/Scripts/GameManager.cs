using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Coins")]
    public int totalCoins = 0;
    public Text coinText;

    [Header("Prefabs")]
    public GameObject coinPrefab;

    [Header("Fish Sprites (assign in Inspector)")]
    public Sprite[] fishSprites;   // 0=876(작은물고기) 1=876_01(게?) 2=876_02(새우?)

    [Header("Shop UI")]
    public GameObject shopPanel;
    public Transform  shopContent;
    public GameObject shopItemPrefab;

    // 구매 가능한 물고기 정의
    [System.Serializable]
    public class FishShopData
    {
        public string  displayName;
        public string  description;
        public int     cost;
        public int     spriteIndex;   // fishSprites 인덱스
        public float   speed;
        public float   scale;
        public float   coinInterval;
        public int     coinValue;
        public MovingObject.ObjectType moveType;
    }

    public List<FishShopData> shopList = new List<FishShopData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        BuildShopList();
    }

    void Start()
    {
        UpdateCoinUI();
        if (shopPanel) shopPanel.SetActive(false);
        BuildShopUI();
    }

    void BuildShopList()
    {
        shopList.Clear();

        shopList.Add(new FishShopData { displayName="작은 물고기",  description="무료! 기본 물고기",       cost=0,   spriteIndex=0, speed=1.5f, scale=2f, coinInterval=10f, coinValue=1, moveType=MovingObject.ObjectType.Fish });
        shopList.Add(new FishShopData { displayName="게",           description="10코인 / 옆으로 걷기!",  cost=10,  spriteIndex=1, speed=1.2f, scale=2.5f, coinInterval=8f, coinValue=2, moveType=MovingObject.ObjectType.Crab });
        shopList.Add(new FishShopData { displayName="새우",         description="25코인 / 지그재그!",    cost=25,  spriteIndex=2, speed=1.8f, scale=2f, coinInterval=6f, coinValue=3, moveType=MovingObject.ObjectType.Shrimp });
        shopList.Add(new FishShopData { displayName="빠른 물고기",  description="50코인 / 코인머신!",    cost=50,  spriteIndex=0, speed=3f,   scale=1.5f, coinInterval=4f, coinValue=5, moveType=MovingObject.ObjectType.Fish });
        shopList.Add(new FishShopData { displayName="황금 물고기",  description="100코인 / 럭셔리!",     cost=100, spriteIndex=0, speed=2f,   scale=3f, coinInterval=3f, coinValue=8, moveType=MovingObject.ObjectType.Fish });
    }

    public void SpawnFishFromShop(FishShopData data)
    {
        Camera cam = Camera.main;
        float h = 2f * cam.orthographicSize;
        float w = h * cam.aspect;
        float x = Random.Range(-w * 0.4f, w * 0.4f);
        float y = Random.Range(-h * 0.3f, h * 0.3f);

        var go = new GameObject(data.displayName);
        go.transform.position = new Vector3(x, y, 0f);
        go.transform.localScale = new Vector3(data.scale, data.scale, 1f);

        // SpriteRenderer
        var sr = go.AddComponent<SpriteRenderer>();
        if (fishSprites != null && data.spriteIndex < fishSprites.Length)
            sr.sprite = fishSprites[data.spriteIndex];

        // MovingObject
        var mo = go.AddComponent<MovingObject>();
        mo.objectType = data.moveType;
        mo.speed = data.speed;

        // CoinSpawner
        var cs = go.AddComponent<CoinSpawner>();
        cs.coinInterval = data.coinInterval;
        cs.coinValue    = data.coinValue;

        // Collider for coin click (not fish click)
        go.AddComponent<CircleCollider2D>();
    }

    public void SpawnCoin(Vector3 pos, int val)
    {
        if (!coinPrefab) return;
        var go = Instantiate(coinPrefab, pos + new Vector3(0f, 0.3f, 0f), Quaternion.identity);
        var cc = go.GetComponent<CoinController>();
        if (cc) cc.value = val;
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateCoinUI();
    }

    void UpdateCoinUI()
    {
        if (coinText) coinText.text = "💰 " + totalCoins;
    }

    public void ToggleShop()
    {
        if (shopPanel) shopPanel.SetActive(!shopPanel.activeSelf);
    }

    void BuildShopUI()
    {
        if (!shopContent || !shopItemPrefab) return;
        foreach (Transform ch in shopContent) Destroy(ch.gameObject);
        foreach (var fd in shopList)
        {
            var item = Instantiate(shopItemPrefab, shopContent);
            var si = item.GetComponent<ShopItem>();
            if (si) si.Setup(fd, this);
        }
    }

    public bool TryBuyFish(FishShopData data)
    {
        if (totalCoins < data.cost) return false;
        totalCoins -= data.cost;
        UpdateCoinUI();
        SpawnFishFromShop(data);
        return true;
    }
}
