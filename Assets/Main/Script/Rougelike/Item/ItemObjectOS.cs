using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemObjectOS : ScriptableObject
{
    [Header("아이템 이름")]
    [SerializeField] private string itemName;
    [Header("아이템 등급")]
    [SerializeField] private int itemGrade;
    [Header("아이템 스프라이트")]
    [SerializeField] private Sprite itemSprite;
    [Header("아이템 키워드")]
    [SerializeField] private ItemKeyword keyword;
    [Header("아이템 가격")]
    [SerializeField] private int price;
    [Header("아이템 설명")]
    [SerializeField] private string itemInfoString;
 

    public Sprite GetSprite { get {return itemSprite; } }
    public int GetPrice { get { return price; } }
    public string GetName { get { return itemName; } }
    public string GetItemInfo() { return itemInfoString; }
    public void SetItemInfo(string newItemInfo) { itemInfoString = newItemInfo; }
    public virtual void OnBattleStart() { }
    public virtual void OnBattleEnd() { }
    public virtual void OnBeforeTurnStart() { }
    public virtual void OnTurnStart(List<BattleActionContext> battleActions) { }
    public virtual void OnTurnEnd(List<BattleActionContext> battleActions) { }

    public virtual void OnBeforeAttack(BattleActionContext battleAction) { }
    public virtual void OnAfterAttack(BattleActionContext battleAction) { }
}
