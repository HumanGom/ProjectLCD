using TMPro;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [Header("°ñµå º¸»ó TMP")]
    [SerializeField] private TextMeshProUGUI goldRewardTMP;

    private void OpenRewardUI()
    {
        gameObject.SetActive(true);
        int totalGold = GoodsManager.Instance.SaveRewardsValue.gold + GoodsManager.Instance.GoldValue;
        string goldRewardString = $"{GoodsManager.Instance.GoldValue} + {GoodsManager.Instance.SaveRewardsValue.gold} = {totalGold}";
        goldRewardTMP.text = goldRewardString;
        GoodsManager.Instance.IsGetRewardValue = false;
        GoodsManager.Instance.GoldValue = totalGold;
    }


    public void OnCloseButton()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        gameObject.SetActive(false);
        if (GoodsManager.Instance != null && GoodsManager.Instance.IsGetRewardValue) OpenRewardUI();
    }

}
