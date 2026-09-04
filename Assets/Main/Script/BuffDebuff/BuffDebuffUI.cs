using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuffUI : MonoBehaviour
{
    [Header("효과 이미지")]
    [SerializeField] private Image effectImage;
    [Header("위력")]
    [SerializeField] private TextMeshProUGUI powerText;
    [Header("횟수")]
    [SerializeField] private TextMeshProUGUI countText;

    private BuffDebuffEffect effect;

    public BuffDebuffEffect Effect => effect;

    public void Initialize(BuffDebuffEffect newEffect, Sprite icon)
    {
        effect = newEffect;
        effectImage.sprite = icon;
        Refresh();
    }

    public void Refresh()
    {
        if (effect == null) return;

        bool hasPower = effect.effectPower >= 0;
        bool hasCount = effect.effectCount >= 0;

        powerText.gameObject.SetActive(hasPower);
        countText.gameObject.SetActive(hasCount);

        if (hasPower)
        {
            powerText.text = effect.effectPower.ToString();
        }

        if (hasCount)
        {
            countText.text = effect.effectCount.ToString();
        }
    }
}