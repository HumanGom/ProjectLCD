using System.Collections.Generic;
using UnityEngine;

public class GoodsManager : MonoBehaviour
{
    public static GoodsManager Instance { get; private set; }

    [Header("소지중인 아이템")]
    [SerializeField] private List<ItemObjectOS> Owneditems = new List<ItemObjectOS>();
    [Header("모든 일반적 아이템")]
    [SerializeField] private List<ItemObjectOS> NomalItems = new List<ItemObjectOS>();
    [Header("소지중인 골드")]
    [SerializeField] private int Gold = 0;

    private BGMManager bgmManager;
    private bool isGetReward = false;
    private Rewards savedRewards;

    public int GoldValue { get { return Gold; } set { Gold = Mathf.Clamp(value, 0, int.MaxValue); } }
    public Rewards SaveRewardsValue
    {
        get { return savedRewards; }
        set { savedRewards = value; isGetReward = true; }
    }
    public bool IsGetRewardValue { get { return isGetReward; } set { isGetReward = value; } }
    public List<ItemObjectOS> GetOwnedItems {  get { return Owneditems; } }
    public List<ItemObjectOS> GetNomalItems { get { return NomalItems; } }
    
 
    public void AddItem(ItemObjectOS item)
    {
        if (item == null) return;
        Owneditems.Add(item);
    }

    public void RemoveItem(ItemObjectOS item)
    {
        if (item == null) return;
        Owneditems.Remove(item);
    }

    public void GoldClear()
    {
        Gold = 0;
    }

    /*----------------------------------------------------------------------------*/

    public void OnBattleStart()
    {
        foreach (ItemObjectOS item in Owneditems)
        {
            item.OnBattleStart();
        }
    }

    public void OnBattleEnd()
    {
        foreach (ItemObjectOS item in Owneditems)
        {
            item.OnBattleEnd();
        }
    }

    public void OnBeforeTurnStart()
    {
        foreach (ItemObjectOS item in Owneditems)
        {
            item.OnBeforeTurnStart();
        }
    }

    public void OnTurnStart(List<BattleActionContext> battleActions)
    {
        foreach (ItemObjectOS item in Owneditems)
        {
            item.OnTurnStart(battleActions);
        }
    }

    public void OnTurnEnd(List<BattleActionContext> battleActions)
    {
        foreach (ItemObjectOS item in Owneditems)
        {
            item.OnTurnEnd(battleActions);
        }
    }

    public void OnBeforeAttack(BattleActionContext battleAction)
    {
        foreach (ItemObjectOS item in Owneditems)
        {
            item.OnBeforeAttack(battleAction);
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        bgmManager = GetComponent<BGMManager>();
    }
}
