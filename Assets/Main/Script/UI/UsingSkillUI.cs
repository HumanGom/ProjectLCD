using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UsingSkillUI : MonoBehaviour
{
    [Header("스킬 정보")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillNameText;

    [Header("코인")]
    [SerializeField] private Transform coinGroup;
    [SerializeField] private CoinUI coinPrefab;

    [Header("스킬 위력")]
    [SerializeField] private TextMeshProUGUI powerText;

    private readonly List<CoinUI> spawnedCoins = new List<CoinUI>();

    public void ShowSkill(SkillObjectOS skill, List<SkillCoinOS> coins)
    {
        gameObject.SetActive(true);

        skillIcon.sprite = skill.Icon;
        skillNameText.text = skill.SkillName;
        
        powerText.text = "";
        ClearCoins();

        foreach (SkillCoinOS coin in coins)
        {
            CoinUI coinUI = Instantiate(coinPrefab, coinGroup);
            spawnedCoins.Add(coinUI);
        }
    }

    public void SetPower(int power)
    {
        if (powerText == null) return;
        powerText.text = power.ToString();
    }

    public void SetCoinResult(int coinIndex, bool isFront)
    {
        if (coinIndex < 0 || coinIndex >= spawnedCoins.Count) return;
        spawnedCoins[coinIndex].SetCoinSide(isFront);
    }

    public void Hide()
    {
        ClearCoins();
        powerText.text = "";
        gameObject.SetActive(false);
    }

    public void BreakCoin(int coinIndex)
    {
        if (coinIndex < 0 || coinIndex >= spawnedCoins.Count)
            return;

        CoinUI coin = spawnedCoins[coinIndex];

        if (coin == null)
            return;

        Destroy(coin.gameObject);
        spawnedCoins.RemoveAt(coinIndex);
    }

    private void ClearCoins()
    {
        foreach (CoinUI coin in spawnedCoins)
        {
            if (coin != null)
                Destroy(coin.gameObject);
        }

        spawnedCoins.Clear();
    }
    private void Start()
    {
        Hide();
    }
}