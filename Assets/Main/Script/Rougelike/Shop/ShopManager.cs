using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [Header("골드 UI텍스트")]
    [SerializeField] private TextMeshProUGUI goldTMP;
    [Header("판매 가능 아이템 리스트")]
    [SerializeField] private List<ItemObjectOS> items = new List<ItemObjectOS>();
    [Header("판매중인 아이템 리스트")]
    [SerializeField] private List<GameObject> sellingItems = new List<GameObject>();
    [Header("빈 아이템 오브젝트")]
    [SerializeField] private GameObject NullItem;
    [Header("상점 진열 루트")]
    [SerializeField] private Transform itemRoot;

    private void ItemExhibition()
    {
        foreach (ItemObjectOS item in items)
        {
            GameObject newSellIngItem = Instantiate(NullItem, itemRoot);
            if (newSellIngItem.GetComponent<ShopItemOBJ>() != null)
            {
                newSellIngItem.GetComponent<ShopItemOBJ>().Initialize(item);
                sellingItems.Add(newSellIngItem);
            }
        }
    }

    public void RequestItemBuy(ItemObjectOS item)
    {
        RemoveSellingItem(item);
        GoodsManager.Instance.AddItem(item);
    }

    public void RefreshSellItemList()
    {
        items = new List<ItemObjectOS>(GoodsManager.Instance.GetNomalItems);

        List<ItemObjectOS> ownedItems = GoodsManager.Instance.GetOwnedItems;
        foreach (ItemObjectOS ownedItem in ownedItems)
        {
            if (ownedItem != null) items.Remove(ownedItem);
        }
    }


    public void AddSellingItem(ItemObjectOS addItem)
    {
        items.Add(addItem);
    }

    public void RemoveSellingItem(ItemObjectOS removeItem)
    {
        RefreshGoldUI();
        items.Remove(removeItem);
    }

    public void RefreshGoldUI()
    {
        goldTMP.text = GoodsManager.Instance.GoldValue.ToString();
    }

    public void ExitShop()
    {
        //SaveCharacterState();

        MapData.CurrentNode.IsCleared = true;

        foreach (RoguelikeMapNode next in MapData.CurrentNode.NextNodes)
        {
            next.IsReachable = true;
        }

        BattleSceneData.EnemysClear();
        

        SceneManager.LoadScene("TestRougelike");
    }

    private void Start()
    {
        if (BGMManager.Instance != null) BGMManager.Instance.RequestChangeBGM(true);
        RefreshGoldUI();
        RefreshSellItemList();
        ItemExhibition();
    }
}
