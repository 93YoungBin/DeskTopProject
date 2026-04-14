using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Coins")]
    public int  totalCoins = 0;
    public Text coinText;

    [Header("Fish Limit")]
    public int  maxFish     = 5;
    public int  currentFish = 0;
    public Text fishCountText;

    [Header("Pool")]
    public CoinPool coinPool;

    [Header("Fish Sprites")]
    public Sprite[] fishSprites;

    [Header("Fish Tank")]
    public Transform fishTank;

    [Header("Shop UI")]
    public GameObject shopPanel;
    public Transform  shopContent;
    public GameObject shopItemPrefab;

    public enum ShopTab { Buy, Sell, Upgrade }
    private ShopTab currentTab = ShopTab.Buy;
    public Text tabBuyText;
    public Text tabSellText;
    public Text tabUpgradeText;

    public enum ShopCategory { Fish, Upgrade }

    [System.Serializable]
    public class ShopEntry
    {
        public ShopCategory category;
        public string  displayName;
        public string  description;
        public int     cost;
        public int     sellPrice;
        public int     spriteIndex;
        public float   speed;
        public float   scale;
        public float   coinInterval;
        public int     coinValue;
        public MovingObject.ObjectType moveType;
        public int     slotIncrease;
    }

    private List<ShopEntry> catalog = new List<ShopEntry>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        BuildCatalog();
        if (fishTank != null) currentFish = fishTank.childCount;
    }

    void Start()
    {
        UpdateCoinUI();
        UpdateFishCountUI();
        if (shopPanel) shopPanel.SetActive(false);
        SwitchTab(ShopTab.Buy);
    }

    void BuildCatalog()
    {
        catalog.Clear();
        catalog.Add(new ShopEntry { category=ShopCategory.Fish, displayName="\uc0c8\uc6b0", description="\ubb34\ub8cc / 15\ucd08\ub9c8\ub2e4 \ud83d\udcb0x1", cost=0, sellPrice=0, spriteIndex=0, speed=1.5f, scale=2f, coinInterval=15f, coinValue=1, moveType=MovingObject.ObjectType.Shrimp });
        catalog.Add(new ShopEntry { category=ShopCategory.Fish, displayName="\uac8c", description="10\ucf54\uc778 / 12\ucd08\ub9c8\ub2e4 \ud83d\udcb0x2", cost=10, sellPrice=5, spriteIndex=1, speed=1.2f, scale=2.5f, coinInterval=12f, coinValue=2, moveType=MovingObject.ObjectType.Crab });
        catalog.Add(new ShopEntry { category=ShopCategory.Fish, displayName="\ubcf4\ub77c \uc0c8\uc6b0", description="25\ucf54\uc778 / 9\ucd08\ub9c8\ub2e4 \ud83d\udcb0x3", cost=25, sellPrice=12, spriteIndex=2, speed=1.8f, scale=2f, coinInterval=9f, coinValue=3, moveType=MovingObject.ObjectType.Shrimp });
        catalog.Add(new ShopEntry { category=ShopCategory.Fish, displayName="\ube60\ub978 \uc0c8\uc6b0", description="50\ucf54\uc778 / 6\ucd08\ub9c8\ub2e4 \ud83d\udcb0x5", cost=50, sellPrice=25, spriteIndex=0, speed=3.2f, scale=1.5f, coinInterval=6f, coinValue=5, moveType=MovingObject.ObjectType.Shrimp });
        catalog.Add(new ShopEntry { category=ShopCategory.Fish, displayName="\ud669\uae08 \uac8c", description="100\ucf54\uc778 / 4\ucd08\ub9c8\ub2e4 \ud83d\udcb0x8", cost=100, sellPrice=50, spriteIndex=1, speed=2f, scale=3.5f, coinInterval=4f, coinValue=8, moveType=MovingObject.ObjectType.Crab });
        catalog.Add(new ShopEntry { category=ShopCategory.Upgrade, displayName="\uc5b4\ud56d \ud655\uc7a5 +1", description="\uc218\uc6a9 \uc0dd\ubb3c +1\ub9c8\ub9ac", cost=30, sellPrice=0, slotIncrease=1 });
        catalog.Add(new ShopEntry { category=ShopCategory.Upgrade, displayName="\ub300\ud615 \uc5b4\ud56d +3", description="\uc218\uc6a9 \uc0dd\ubb3c +3\ub9c8\ub9ac", cost=75, sellPrice=0, slotIncrease=3 });
    }

    public void SwitchTab(ShopTab tab) { currentTab = tab; RefreshTabUI(); RebuildShopUI(); }
    public void OnTabBuy()     { SwitchTab(ShopTab.Buy); }
    public void OnTabSell()    { SwitchTab(ShopTab.Sell); }
    public void OnTabUpgrade() { SwitchTab(ShopTab.Upgrade); }

    void RefreshTabUI()
    {
        Color active   = new Color(1f, 0.85f, 0.1f);
        Color inactive = new Color(0.6f, 0.6f, 0.6f);
        if (tabBuyText)     tabBuyText.color     = currentTab == ShopTab.Buy     ? active : inactive;
        if (tabSellText)    tabSellText.color     = currentTab == ShopTab.Sell    ? active : inactive;
        if (tabUpgradeText) tabUpgradeText.color  = currentTab == ShopTab.Upgrade ? active : inactive;
    }

    public void RebuildShopUI()
    {
        if (!shopContent || !shopItemPrefab) return;
        foreach (Transform ch in shopContent) Destroy(ch.gameObject);

        if (currentTab == ShopTab.Buy)
        {
            foreach (var e in catalog)
                if (e.category == ShopCategory.Fish)
                    CreateShopItem(e, false);
        }
        else if (currentTab == ShopTab.Sell)
        {
            if (fishTank == null) return;
            var seen = new Dictionary<string, int>();
            foreach (Transform child in fishTank)
            {
                if (!seen.ContainsKey(child.name)) seen[child.name] = 0;
                seen[child.name]++;
            }
            if (seen.Count == 0)
            {
                var ph = Instantiate(shopItemPrefab, shopContent);
                var si2 = ph.GetComponent<ShopItem>();
                if (si2) si2.SetupEmpty("\uc5b4\ud56d\uc774 \ube44\uc5b4\uc788\uc2b5\ub2c8\ub2e4");
                return;
            }
            foreach (var kv in seen)
            {
                var entry = catalog.Find(e => e.displayName == kv.Key);
                if (entry == null) continue;
                var sellEntry = new ShopEntry
                {
                    displayName = entry.displayName,
                    description = string.Format("\ud310\ub9e4\uac00 \ud83d\udcb0{0}  (\ubcf4\uc720 {1}\ub9c8\ub9ac)", entry.sellPrice, kv.Value),
                    sellPrice   = entry.sellPrice,
                };
                CreateShopItem(sellEntry, true);
            }
        }
        else
        {
            foreach (var e in catalog)
                if (e.category == ShopCategory.Upgrade)
                    CreateShopItem(e, false);
        }
    }

    void CreateShopItem(ShopEntry e, bool isSell)
    {
        var item = Instantiate(shopItemPrefab, shopContent);
        var si   = item.GetComponent<ShopItem>();
        if (si) si.Setup(e, this, isSell);
    }

    public bool CanSpawnFish() { return currentFish < maxFish; }

    public void SpawnFish(ShopEntry data)
    {
        if (!CanSpawnFish()) return;
        Camera cam = Camera.main;
        float h = 2f * cam.orthographicSize;
        float w = h * cam.aspect;
        var go = new GameObject(data.displayName);
        go.transform.position   = new Vector3(Random.Range(-w*0.4f, w*0.4f), Random.Range(-h*0.3f, h*0.3f), 0f);
        go.transform.localScale = new Vector3(data.scale, data.scale, 1f);
        if (fishTank != null) go.transform.SetParent(fishTank, true);
        var sr = go.AddComponent<SpriteRenderer>();
        if (fishSprites != null && data.spriteIndex < fishSprites.Length) sr.sprite = fishSprites[data.spriteIndex];
        var mo = go.AddComponent<MovingObject>(); mo.objectType = data.moveType; mo.speed = data.speed;
        var cs = go.AddComponent<CoinSpawner>();  cs.coinInterval = data.coinInterval; cs.coinValue = data.coinValue;
        currentFish++;
        UpdateFishCountUI();
    }

    public void SellFish(ShopEntry entry)
    {
        if (fishTank == null) return;
        Transform target = null;
        foreach (Transform child in fishTank)
            if (child.name == entry.displayName) { target = child; break; }
        if (target == null) return;
        Destroy(target.gameObject);
        currentFish = Mathf.Max(0, currentFish - 1);
        AddCoins(entry.sellPrice);
        UpdateFishCountUI();
        RebuildShopUI();
    }

    public void ApplyUpgrade(ShopEntry data)
    {
        maxFish += data.slotIncrease;
        UpdateFishCountUI();
        RebuildShopUI();
    }

    public bool TryBuy(ShopEntry entry)
    {
        if (totalCoins < entry.cost) return false;
        if (entry.category == ShopCategory.Fish && !CanSpawnFish()) return false;
        totalCoins -= entry.cost;
        UpdateCoinUI();
        if (entry.category == ShopCategory.Fish) SpawnFish(entry);
        else ApplyUpgrade(entry);
        RebuildShopUI();
        return true;
    }

    public void SpawnCoin(Vector3 pos, int val) { if (coinPool != null) coinPool.Get(val, pos + new Vector3(0f, 0.3f, 0f)); }

    public void AddCoins(int amount) { totalCoins += amount; UpdateCoinUI(); }

    void UpdateCoinUI()      { if (coinText)      coinText.text      = "\ud83d\udcb0 " + totalCoins; }
    void UpdateFishCountUI() { if (fishCountText) fishCountText.text = string.Format("\ud83d\udc20 {0}/{1}", currentFish, maxFish); }

    public void ToggleShop()
    {
        if (!shopPanel) return;
        bool opening = !shopPanel.activeSelf;
        shopPanel.SetActive(opening);
        if (opening) RebuildShopUI();
    }
}
