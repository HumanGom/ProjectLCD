using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemUI : MonoBehaviour
{
    [Header("기타 TMP")]
    [SerializeField] private TextMeshProUGUI etcTMP;
    [Header("아이템 이름 TMP")]
    [SerializeField] private TextMeshProUGUI itemNameTMP;
    [Header("아이템 가격 TMP")]
    [SerializeField] private TextMeshProUGUI itemPriceTMP;
    [Header("구매할 아이템 이미지")]
    [SerializeField] private Image itemImage;
    [Header("구매 수락 버튼")]
    [SerializeField] private Button posButton;

    private int totalGold = 0;
    private int price = 0;
    private ShopItemOBJ selectedItemOBJ;
    private ItemObjectOS selectedItemObjOS;
    private ShopManager shopManager;

    private void ClearSavedInfo()
    {
        totalGold = 0;
        price = 0;
        etcTMP.text = null;
        itemPriceTMP.text = "0";
        itemNameTMP.text = null;
        itemImage.sprite = null;
        selectedItemOBJ = null;
        selectedItemObjOS = null;
    }

    private void UISetting(int totalGold)
    {
        string totalPriceTxt = "";
        if (totalGold < 0)
        {
            posButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Can't Buy";
            posButton.interactable = false;
            totalPriceTxt = $"구매불가";
        }
        else
        {
            posButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
            posButton.interactable = true;
            totalPriceTxt = $"{GoodsManager.Instance.GoldValue} - {price} = {totalGold}";
        }

        if (selectedItemObjOS.GetSprite != null)
        {
            itemImage.sprite = selectedItemObjOS.GetSprite;
        }
        
        itemPriceTMP.text = totalPriceTxt;
    }

    public void OpenBuyUI(ItemObjectOS itemData, ShopItemOBJ itemOBJ)
    {
        gameObject.SetActive(true);
        price = itemData.GetPrice;
        totalGold = GoodsManager.Instance.GoldValue - price;
        //string goldRewardString = $"{GoodsManager.Instance.GoldValue} - {itemData.GetPrice} = {totalGold}";
        //etcTMP.text = goldRewardString;
        etcTMP.text = itemData.GetItemInfo();
        selectedItemOBJ = itemOBJ;
        selectedItemObjOS = itemData;
        itemNameTMP.text = itemData.GetName;
        UISetting(totalGold);
    }

    public void OnPositiveButton()
    {
        GoodsManager.Instance.GoldValue = totalGold;

        if (selectedItemOBJ != null)
        {
            shopManager.RequestItemBuy(selectedItemObjOS);
            selectedItemOBJ.OnSelled();
            ClearSavedInfo();
        }

        gameObject.SetActive(false);
    }
    public void OnNegativeButton()
    {
        ClearSavedInfo();

        gameObject.SetActive(false);
    }

    

    private void Awake()
    {
        shopManager = GetComponentInParent<ShopManager>();
    }



}
