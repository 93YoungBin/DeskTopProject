using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Text   nameText;
    public Text   descText;
    public Text   costText;
    public Button buyButton;
    public Image  bgImage;

    private GameManager.ShopEntry entry;
    private GameManager gm;
    private bool isSell;

    static readonly Color COL_BUY     = new Color(0.12f, 0.28f, 0.45f, 0.95f);
    static readonly Color COL_SELL    = new Color(0.40f, 0.12f, 0.12f, 0.95f);
    static readonly Color COL_UPGRADE = new Color(0.30f, 0.18f, 0.05f, 0.95f);
    static readonly Color COL_BTN_OK  = new Color(0.10f, 0.65f, 0.28f, 1f);
    static readonly Color COL_BTN_SELL= new Color(0.75f, 0.20f, 0.10f, 1f);
    static readonly Color COL_BTN_NG  = new Color(0.35f, 0.35f, 0.35f, 1f);

    public void Setup(GameManager.ShopEntry e, GameManager manager, bool sell)
    {
        entry  = e;
        gm     = manager;
        isSell = sell;

        if (nameText) nameText.text = e.displayName;
        if (descText) descText.text = e.description;

        if (sell)
        {
            if (costText) costText.text = "\ud310\ub9e4";
            if (bgImage)  bgImage.color = COL_SELL;
            if (buyButton)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(OnClick);
                var img = buyButton.GetComponent<Image>();
                if (img) img.color = COL_BTN_SELL;
                buyButton.interactable = true;
            }
        }
        else
        {
            bool isUpgrade = e.category == GameManager.ShopCategory.Upgrade;
            string costStr = e.cost == 0 ? "\ubb34\ub8cc" : e.cost + " \ud83d\udcb0";
            if (isUpgrade) costStr += " \ud83d\udd27";
            if (costText) costText.text = costStr;
            if (bgImage)  bgImage.color = isUpgrade ? COL_UPGRADE : COL_BUY;
            if (buyButton)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(OnClick);
                RefreshBuyButton();
            }
        }
    }

    public void SetupEmpty(string msg)
    {
        if (nameText) nameText.text = msg;
        if (descText) descText.text = "";
        if (costText) costText.text = "";
        if (buyButton) buyButton.gameObject.SetActive(false);
        if (bgImage)   bgImage.color = new Color(0.15f, 0.15f, 0.15f, 0.8f);
    }

    void RefreshBuyButton()
    {
        if (buyButton == null || gm == null) return;
        bool canAfford = gm.totalCoins >= entry.cost;
        bool hasSlot   = entry.category == GameManager.ShopCategory.Upgrade || gm.CanSpawnFish();
        bool ok        = canAfford && hasSlot;
        buyButton.interactable = ok;
        var img = buyButton.GetComponent<Image>();
        if (img) img.color = ok ? COL_BTN_OK : COL_BTN_NG;
        if (!hasSlot && descText && entry.category == GameManager.ShopCategory.Fish)
            descText.text = "\u26a0 \uc5b4\ud56d \uac00\ub4dd! \uc5c5\uadf8\ub808\uc774\ub4dc \ud544\uc694";
    }

    void OnClick()
    {
        if (gm == null) return;
        if (isSell) gm.SellFish(entry);
        else        gm.TryBuy(entry);
    }
}
