using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CharactersManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform playerControlPenal;
    [SerializeField] private GameObject chooseSlotPrefab;
    [Header("캐릭터 스포너")]
    [SerializeField] private CharacterSpawner characterSpawner;
    [SerializeField] private int maxActionSlotCount = 6;
    private List<GameObject> chooseSlotObjectsList = new List<GameObject>();
    private List<GameObject> characterList = new List<GameObject>();
    private int currentActionSlotCount;
    private int nextBonusCharacterIndex = 0;
    private bool isActionSlotInitialized = false;
    private Transform sideButtons;

    private readonly Dictionary<GameObject, int> characterSlotCounts = new Dictionary<GameObject, int>();
    private readonly List<HeadSlot> spawnedHeadSlots = new List<HeadSlot>();

    public List<GameObject> GetCharacterList { get { return characterList; } }

    public void SlotSetting(GameObject character, int deckIndex)
    {
        CharacterSkillManager skillManager = character.GetComponent<CharacterSkillManager>();

        CharacterStatus characterStatus = character.GetComponent<CharacterStatus>();
            
        if (skillManager == null)
        {
            Debug.LogError("캐릭터에 CharaterSkillManager가 없음");
            return;
        }


        HeadSlot headSlot = characterStatus.CreateHeadSlot();

        GameObject chooseSlotObj = Instantiate(chooseSlotPrefab, playerControlPenal);
        chooseSlotObj.transform.SetSiblingIndex(sideButtons.GetSiblingIndex());
        chooseSlotObjectsList.Add(chooseSlotObj);

        PlayerSlot slot1 = chooseSlotObj.transform.Find("Slot1").GetComponent<PlayerSlot>();
        PlayerSlot slot2 = chooseSlotObj.transform.Find("Slot2").GetComponent<PlayerSlot>();

        slot1.SetLinkedHeadSlot(headSlot);
        slot2.SetLinkedHeadSlot(headSlot);

        Sprite characterPortrait = character.GetComponent<CharacterStatus>().GetPortrait;

        SlotChanger slotChanger = chooseSlotObj.GetComponentInChildren<SlotChanger>();
        slotChanger.Initialize(skillManager, characterPortrait, deckIndex);
    }

    public void SetAllCharactersSpeeds()
    {
        foreach(var character in characterList) 
        {
            character.GetComponent<CharacterStatus>().SetRandomSpeed();
        }
        foreach (var character in characterList)
        {
            character.GetComponent<CharacterStatus>().RequestPassingSpeedForUI();
        }
    }

    public void AddCharacterList(GameObject character)
    {
        characterList.Add(character);

        if (!characterSlotCounts.ContainsKey(character)) characterSlotCounts.Add(character, 1);
    }

    public void RemoveCharacterList(GameObject character)
    {
        characterList.Remove(character);
    }

    public void RequestSetSpeedAndSlot()
    {
        if (characterList == null) return;

        ClearPlayerControlPanel();

        SetAllCharactersSpeeds();

        int totalSlotCount = GetTotalSlotCount();

        // 첫 생성 이후부터 매 턴 하나씩 추가
        if (isActionSlotInitialized)
        {
            if (totalSlotCount < maxActionSlotCount) GiveNextBonusSlot();
        }
        else isActionSlotInitialized = true;

        CreateActionSlots();
    }

    private int GetTotalSlotCount()
    {
        int total = 0;

        foreach (GameObject character in characterList)
        {
            if (character == null) continue;

            if (characterSlotCounts.TryGetValue(character, out int count)) total += count;
        }

        return total;
    }

    private void GiveNextBonusSlot()
    {
        if (characterList.Count == 0) return;
        if (nextBonusCharacterIndex >= characterList.Count) nextBonusCharacterIndex = 0;

        GameObject character = characterList[nextBonusCharacterIndex];

        if (character != null) characterSlotCounts[character]++;

        nextBonusCharacterIndex++;

        if (nextBonusCharacterIndex >= characterList.Count) nextBonusCharacterIndex = 0;
    }

    private void CreateActionSlots()
    {
        List<GameObject> sortedCharacters =
            new List<GameObject>(characterList);

        sortedCharacters.Sort((a, b) =>
        {
            int speedCompare = b.GetComponent<CharacterStatus>().GetSpeed.CompareTo(a.GetComponent<CharacterStatus>().GetSpeed);

            if (speedCompare != 0) return speedCompare;

            return characterList.IndexOf(a).CompareTo(characterList.IndexOf(b));
        });

        foreach (GameObject character in sortedCharacters)
        {
            if (character == null) continue;

            if (!characterSlotCounts.TryGetValue(character, out int slotCount)) 
            {
                slotCount = 1;
                characterSlotCounts[character] = 1;
            }

            for (int deckIndex = 0; deckIndex < slotCount; deckIndex++) 
            {
                SlotSetting(character, deckIndex);
            }
        }
    }

    private void AdjustBonusIndexAfterDeath(int deadIndex)
    {
        if (characterList.Count == 0)
        {
            nextBonusCharacterIndex = 0;
            return;
        }

        if (deadIndex < nextBonusCharacterIndex) nextBonusCharacterIndex--;

        if (nextBonusCharacterIndex >= characterList.Count) nextBonusCharacterIndex = 0;
    }

    public void MentalBonusForAllCharactor(int mentalValue)
    {
        foreach(GameObject character in characterList)
        {
            CharacterStatus characterStatus = character.GetComponent<CharacterStatus>();
            characterStatus.MentalityValue += mentalValue;
        }
    }

    public void ShowControlPenal()
    {
        playerControlPenal.gameObject.SetActive(true);
    }

    public void HideControlPenal()
    {
        playerControlPenal.gameObject.SetActive(false);
    }

    public bool IsAllCharatersDead()
    {
        return characterList.Count == 0;
    }

    public void OnCharacterDead(GameObject deadCharacter)
    {
        int deadIndex = characterList.IndexOf(deadCharacter);

        RemoveCharacterList(deadCharacter);
        characterSlotCounts.Remove(deadCharacter);

        CharacterStatus status = deadCharacter.GetComponent<CharacterStatus>();

        if (status != null) status.ClearHeadSlots();
        AdjustBonusIndexAfterDeath(deadIndex);

        deadCharacter.SetActive(false);
    }

    public void ResetActionSlotProgress()
    {
        currentActionSlotCount = characterList.Count;
        nextBonusCharacterIndex = 0;
        isActionSlotInitialized = false;
    }

    private void ClearPlayerControlPanel()
    {  
        foreach(var chooseSlotObject in chooseSlotObjectsList)
        {
            if(chooseSlotObject != null)
            {
                Destroy(chooseSlotObject);
            }
        }
        chooseSlotObjectsList.Clear();

        foreach (GameObject character in characterList)
        {
            if (character == null) continue;

            CharacterStatus status = character.GetComponent<CharacterStatus>();

            if (status != null)
            {
                status.ClearHeadSlots();
            }
        }
    }

    private void Awake()
    {
        currentActionSlotCount = characterSpawner.GetSelectedCharacterCount;
        sideButtons = playerControlPenal.Find("ETCButton")?.GetComponent<Transform>();
    }
}
