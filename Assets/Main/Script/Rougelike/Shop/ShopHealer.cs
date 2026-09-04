using System.Collections.Generic;
using UnityEngine;

public class ShopHealer : MonoBehaviour
{
    [Header("±âº» Èú ¹öÆ° ÇÁ¸®ÆÕ")]
    [SerializeField] GameObject healOBJ;

    private List<CharacterSaveData> charactersSaveData;
    private List<HealButtonOBJ> healButtons = new List<HealButtonOBJ>();

    private void LoadCharacters()
    {
        if (BattleSceneData.CharactersData == null) return;

        charactersSaveData = new List<CharacterSaveData>(BattleSceneData.CharactersData);
    }

    private void MakeHealButton()
    {
        foreach (CharacterSaveData data in charactersSaveData)
        {
            GameObject newHealButton = Instantiate(healOBJ, this.transform);

            if (newHealButton.GetComponent<HealButtonOBJ>() != null)
            {
                HealButtonOBJ healbutton = newHealButton.GetComponent<HealButtonOBJ>();
                healbutton.Initialize(data);
                healButtons.Add(healbutton);
            }
        }
    }

    public void RefreshAllHealButtonUI()
    {
        foreach(HealButtonOBJ healbutton in healButtons)
        { 
            if(healbutton.getHPBarUI != null) healbutton.getHPBarUI.ReFreshHPUI();
        }
    }

    private void Awake()
    {
        LoadCharacters();
    }

    private void Start()
    {
        if(charactersSaveData != null) MakeHealButton();
    }
}
