using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI hpText;

    private BossPart bossPart;

    public void Initialize(BossPart part)
    {
        bossPart = part;

        slider.maxValue = bossPart.MaxHp;
        Refresh();

        if (!bossPart.IsCore) 
        {
            gameObject.SetActive(false);
        }
    }

    public void Refresh()
    {
        if (bossPart == null) return;

        slider.value = bossPart.HpValue;
        hpText.text = bossPart.HpValue.ToString();
    }
}
