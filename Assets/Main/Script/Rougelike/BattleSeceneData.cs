using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class BattleSceneData
{
    public static List<GameObject> EnemyPrefabs = new List<GameObject>();
    public static List<GameObject> PlayerPrefabs = new List<GameObject>();
    public static List<CharacterSaveData> CharactersData = new List<CharacterSaveData>();
    public static List<CharacterSaveData> BeforeBattleCharactersData = new List<CharacterSaveData>();
    public static RoguelikeRoomType roguelikeRoomType;

    public static bool isFirstPlayers = true;

    public static void SaveBeforeBattle(List<CharacterSaveData> data)
    {
        BeforeBattleCharactersData.Clear();

        foreach (CharacterSaveData character in data)
        {
            BeforeBattleCharactersData.Add(new CharacterSaveData
            {
                CharacterName = character.CharacterName,
                Hp = character.Hp,
                Mentality = character.Mentality
            });
        }
    }

    public static void RestoreBeforeBattle()
    {
        CharactersData.Clear();

        foreach (CharacterSaveData character in BeforeBattleCharactersData)
        {
            CharactersData.Add(new CharacterSaveData
            {
                CharacterName = character.CharacterName,
                Hp = character.Hp,
                Mentality = character.Mentality
            });
        }
    }

    public static void SetEnemies(List<GameObject> enemies, RoguelikeRoomType roomType)
    {
        EnemyPrefabs.Clear();
        if(enemies != null) EnemyPrefabs.AddRange(enemies);
        roguelikeRoomType = roomType;
    }

    public static void SetPlayerCharacter(List<GameObject> characters)
    {
        PlayerPrefabs.Clear();
        if (characters != null) PlayerPrefabs.AddRange(characters);
    }

    public static void CharactersClear()
    {
        CharactersData.Clear();
    }

    public static void EnemysClear()
    {
        EnemyPrefabs.Clear();
        roguelikeRoomType = RoguelikeRoomType.Null;
    }
}
