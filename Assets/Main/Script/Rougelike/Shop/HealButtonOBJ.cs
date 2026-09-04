using UnityEngine;
using UnityEngine.UI;

public class HealButtonOBJ : MonoBehaviour
{
    public void Initialize(CharacterSaveData saveData)
    {
        hpBarUI.Initialize(saveData);
        this.saveData = saveData;
        Image image = GetComponent<Image>();
        image.sprite = saveData.Status.GetPortrait;
        /*        if (saveData.Hp <= 0)
                {
                    Color color = image.color;
                    color.a = 0.5f;
                    image.color = color;
                }*/
        hpBarUI.ReFreshHPUI();
    }


    private HPBarUI hpBarUI;
    private ShopHealUI shopHealUI;
    private CharacterSaveData saveData;

    public HPBarUI getHPBarUI {  get { return hpBarUI; } }


    public void OnButtonClick()
    {
        if (shopHealUI != null) shopHealUI.OpenHealUI(saveData);
    }

    private void Awake()
    {
        hpBarUI = GetComponentInChildren<HPBarUI>();
        shopHealUI = FindFirstObjectByType<ShopHealUI>(FindObjectsInactive.Include);
    }
}
