using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Roguelike/PlayerPool")]
public class PlayerPoolOS : ScriptableObject
{
    [SerializeField] private List<GameObject> PlayerPrefab = new List<GameObject>();


    public List<GameObject> GetGetPlayerCharacterList()
    {
        return new List<GameObject>(PlayerPrefab);
    }
}
