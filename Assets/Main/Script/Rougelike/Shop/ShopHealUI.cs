using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopHealUI : MonoBehaviour
{
    [Header("캐릭터 이름 TMP")]
    [SerializeField] private TextMeshProUGUI characterNameTMP;
    [Header("캐릭터 초상화")]
    [SerializeField] private Image characterPortrait;
    [Header("가격 TMP")]
    [SerializeField] private TextMeshProUGUI priceTMP;
    [Header("부활 버튼")]
    [SerializeField] private Button reviveButton;
    [Header("회복 버튼")]
    [SerializeField] private Button healButton;

    private int totalGold = 0;
    private int price = 0;
    private CharacterSaveData saveData;
    private ShopHealer shopHealer;

    private void ClearSavedInfo()
    {
        totalGold = 0;
        price = 0;
        priceTMP.text = "0";
        characterNameTMP.text = null;
        characterPortrait.sprite = null;
        saveData = null;
    }

    private void UISetting(int totalGold)
    {
        string totalPriceTxt = "";
        if (totalGold < 0)
        {
            reviveButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Can't Buy";
            reviveButton.interactable = false;
            healButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Can't Buy";
            healButton.interactable = false;
            priceTMP.text = $"구매불가";
            return;
        }

        if (saveData.Hp <= 0)
        {
            reviveButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Revive";
            reviveButton.interactable = true;
            healButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Can't Heal";
            healButton.interactable = false;
        }
        else if (saveData.Hp < saveData.Status.GetDefaultHP) 
        {
            reviveButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Can't Revive";
            reviveButton.interactable = false;
            healButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Heal";
            healButton.interactable = true;
        }
        else
        {
            reviveButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Can't Revive";
            reviveButton.interactable = false;
            healButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Can't Heal";
            healButton.interactable = false;
        }
        
        
        totalPriceTxt = $"{GoodsManager.Instance.GoldValue} - {price} = {totalGold}";

        if (saveData.Status.GetPortrait != null)
        {
            characterPortrait.sprite = saveData.Status.GetPortrait;
        }

        priceTMP.text = totalPriceTxt;
    }

    public void OpenHealUI(CharacterSaveData saveData)
    {
        gameObject.SetActive(true);
        price = 100;
        totalGold = GoodsManager.Instance.GoldValue - price;
        this.saveData = saveData;
        UISetting(totalGold);
    }

    public void OnReviveButton()
    {
        GoodsManager.Instance.GoldValue = totalGold;

        if (saveData != null)
        {
            saveData.Hp= saveData.Status.GetDefaultHP;
            ClearSavedInfo();
        }
        shopHealer.RefreshAllHealButtonUI();
        gameObject.SetActive(false);
    }

    public void OnHealButton()
    {
        GoodsManager.Instance.GoldValue = totalGold;

        if (saveData != null)
        {
            saveData.Hp = saveData.Status.GetDefaultHP;
            saveData.Mentality = Mathf.Clamp(saveData.Mentality + 15, -45, 45);
            ClearSavedInfo();
        }
        shopHealer.RefreshAllHealButtonUI();
        gameObject.SetActive(false);
    }

    public void OnNegativeButton()
    {
        ClearSavedInfo();

        gameObject.SetActive(false);
    }

    private void Awake()
    {
        shopHealer = FindFirstObjectByType<ShopHealer>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
}
