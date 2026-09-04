using TMPro;
using UnityEngine;

public class RougelikeUIManager : MonoBehaviour
{
    [Header("∞ÒµÂ UI≈ÿΩ∫∆Æ")]
    [SerializeField] private TextMeshProUGUI goldTMP;

    public void RefreshGoldUI()
    {
        goldTMP.text = GoodsManager.Instance.GoldValue.ToString();
    }

    private void Start()
    {
        RefreshGoldUI();
    }

}
