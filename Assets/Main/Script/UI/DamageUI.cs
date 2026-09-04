using TMPro;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;

    private int totalDamage = 0;

    public void BeginDamage()
    {
        totalDamage = 0;
        damageText.text = "0";
        gameObject.SetActive(true);
    }

    public void AddDamage(int damage)
    {
        if (damage <= 0) return;

        totalDamage += damage;
        damageText.text = totalDamage.ToString();
    }

    public void EndDamage()
    {
        totalDamage = 0;
        damageText.text = "";
        gameObject.SetActive(false);
    }
}