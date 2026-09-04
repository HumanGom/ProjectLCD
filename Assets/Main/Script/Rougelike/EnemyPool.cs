using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Roguelike/EnemyPool")]
public class EnemyPoolOS : ScriptableObject
{
    [SerializeField] private RoguelikeRoomType roomType;
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

    public RoguelikeRoomType RoomType => roomType;

    public List<GameObject> GetEnemiePool()
    {
        return new List<GameObject>(enemyPrefabs);
    }
}