using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Text nameText;
    public Text descText;
    public Text costText;
    public Button buyButton;

    private GameManager.FishShopData data;
    private GameManager gm;

    public void Setup(GameManager.FishShopData fd, GameManager manager)
    {
        data = fd;
        gm   = manager;
        if (nameText) nameText.text = fd.displayName;
        if (descText) descText.text = fd.description;
        if (costText) costText.text = fd.cost == 0 ? "무료" : fd.cost + " 💰";
        if (buyButton) buyButton.onClick.AddListener(OnBuy);
    }

    void OnBuy()
    {
        if (!gm) return;
        if (!gm.TryBuyFish(data))
            Debug.Log("코인 부족!");
    }
}
