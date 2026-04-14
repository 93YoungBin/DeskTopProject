using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Text   nameText;
    public Text   descText;
    public Text   costText;
    public Button buyButton;
    public Image  bgImage;         // 카테고리별 색상 구분

    private GameManager.ShopEntry entry;
    private GameManager gm;

    static readonly Color COL_FISH    = new Color(0.12f, 0.28f, 0.45f, 0.95f);
    static readonly Color COL_UPGRADE = new Color(0.35f, 0.20f, 0.05f, 0.95f);
    static readonly Color COL_BTN_OK  = new Color(0.10f, 0.65f, 0.28f, 1f);
    static readonly Color COL_BTN_NG  = new Color(0.40f, 0.40f, 0.40f, 1f);

    public void Setup(GameManager.ShopEntry e, GameManager manager)
    {
        entry = e;
        gm    = manager;

        if (nameText) nameText.text = e.displayName;
        if (descText) descText.text = e.description;

        bool isUpgrade = e.category == GameManager.ShopCategory.Upgrade;
        string costStr = e.cost == 0 ? "무료" : e.cost + " 💰";
        if (isUpgrade) costStr += "  🔧";
        if (costText) costText.text = costStr;

        if (bgImage) bgImage.color = isUpgrade ? COL_UPGRADE : COL_FISH;

        if (buyButton)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuy);
            RefreshButton();
        }
    }

    void RefreshButton()
    {
        if (buyButton == null) return;
        bool canAfford = gm.totalCoins >= entry.cost;
        bool hasSlot   = entry.category == GameManager.ShopCategory.Upgrade || gm.CanSpawnFish();
        bool ok        = canAfford && hasSlot;

        buyButton.interactable = ok;
        var img = buyButton.GetComponent<Image>();
        if (img) img.color = ok ? COL_BTN_OK : COL_BTN_NG;

        // 슬롯 꽉 찼을 때 텍스트 힌트
        if (entry.category == GameManager.ShopCategory.Fish && !hasSlot)
            if (descText) descText.text = "⚠ 어항 가득참! 업그레이드 필요";
    }

    void OnBuy()
    {
        if (gm == null) return;
        gm.TryBuy(entry);
        // 모든 ShopItem 버튼 갱신
        foreach (var si in FindObjectsOfType<ShopItem>())
            si.RefreshButton();
    }
}
