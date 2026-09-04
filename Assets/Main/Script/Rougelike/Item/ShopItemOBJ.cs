using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemOBJ : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] Button button;
    [Header("아이템 오브젝트 데이터")]
    [SerializeField] ItemObjectOS itemData;
    [Header("아이템 가격 텍스트")]
    [SerializeField] private TextMeshProUGUI itemPrice;

    private BuyItemUI itemBuyUI;
    private Image itemImage;

    public void Initialize(ItemObjectOS newItemData)
    {
        itemImage = GetComponent<Image>();

        itemData = newItemData;

        if (itemData != null)
        {
            itemPrice.text = itemData.GetPrice.ToString();
            if (itemData.GetSprite != null) itemImage.sprite = itemData.GetSprite;
        }
        button.onClick.AddListener(OnItemClick);
    }

    public void OnItemClick()
    {
        itemBuyUI.OpenBuyUI(itemData, this);
    }

    public void OnSelled()
    {
        button.interactable = false;
        itemPrice.text = "Sold";
    }

    private void Awake()
    {
        itemBuyUI = FindFirstObjectByType<BuyItemUI>(FindObjectsInactive.Include);
    }
}
