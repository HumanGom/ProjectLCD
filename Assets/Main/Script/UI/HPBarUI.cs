using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    [Header("체력 바 UI")]
    [SerializeField] private Slider slider;
    [Header("체력 수치 UI")]
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private CharacterStatus characterStatus;
    private EnemyStatus enemyStatus;
    private CharacterSaveData CharacterSaveData;
    private int maxHp;

    public void Initialize(CharacterSaveData saveData)
    {
        CharacterSaveData = saveData;
        maxHp = saveData.Status.HpValue;

        slider.maxValue = maxHp;
    }

    public void Initialize(CharacterStatus status)
    {
        characterStatus = status;
        maxHp = status.HpValue;

        slider.maxValue = maxHp;
    }

    public void Initialize(EnemyStatus status)
    {
        enemyStatus = status;
        maxHp = status.HpValue;

        slider.maxValue = maxHp;
    }

    public void ReFreshHPUI()
    {
        if (characterStatus != null)
        {
            slider.value = characterStatus.HpValue;
            textMeshPro.text = characterStatus.HpValue.ToString();
        }
        else if (enemyStatus != null)
        {
            slider.value = enemyStatus.HpValue;
            textMeshPro.text = enemyStatus.HpValue.ToString();
        }
        else if (CharacterSaveData != null) 
        {
            slider.value = CharacterSaveData.Hp > maxHp ? maxHp : CharacterSaveData.Hp;
            textMeshPro.text = CharacterSaveData.Hp.ToString();
        }
    }
}