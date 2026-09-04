using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Null Item")]

public class NullItem : ItemObjectOS
{
    private void Awake()
    {
        SetItemInfo($"테스트용 무효과 아이템");
    }
}
