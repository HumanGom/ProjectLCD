using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CharacterSpawner : MonoBehaviour
{
    [Header("선택된 캐릭터")]
    [SerializeField] private List<GameObject> characters = new List<GameObject>();
    [Header("기본스폰위치")]
    [SerializeField] private Vector3 defaultPos = new Vector3(-1.7f, 2.5f, 0f);
    [Header("캐릭터 간격")]
    [SerializeField] private Vector3 offset = new Vector3(0.5f, 0f, 0f);
    [Header("캐릭터 크기 배율")]
    [SerializeField] private float characterSize = 1f;
    [Header("캐릭터 리스트")]
    [SerializeField] private Transform characterListRoot;
    [Header("라운드 매니저")]
    [SerializeField] private TurnManager roundManager;
    [Header("캐릭터 매니저")]
    [SerializeField] private CharactersManager charactersManager;

    public int GetSelectedCharacterCount { get { return characters.Count; } }

    public void CharacterSpawn()
    {
        List<GameObject> spawnList = BattleSceneData.PlayerPrefabs.Count > 0 ? BattleSceneData.PlayerPrefabs : characters;

        Vector3 pos = defaultPos;
        foreach (var character in spawnList)
        {
            character.name = character.GetComponent<CharacterStatus>().GetName;
            GameObject spawnedCharacter = Instantiate(character, pos, Quaternion.identity, characterListRoot);
            spawnedCharacter.transform.localScale = Vector3.one * characterSize;
            charactersManager.AddCharacterList(spawnedCharacter);
            pos += offset;

            CharacterStatus status = spawnedCharacter.GetComponent<CharacterStatus>();
            CharacterSaveData saveData =  BattleSceneData.CharactersData.Find( x => x.CharacterName == status.GetName);
            if (saveData != null)
            {
                status.HpValue = saveData.Hp;
                status.MentalityValue = saveData.Mentality;
            }
            status.UIConnect();
        }
    }
}
