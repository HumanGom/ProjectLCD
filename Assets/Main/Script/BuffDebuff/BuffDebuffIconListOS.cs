using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "BuffDebuffIconListOS")]
public class BuffDebuffIconListOS : ScriptableObject
{
    [Header("효과 아이콘 리스트")]
    [SerializeField] private List<Sprite> sprites;

    public List<Sprite> GetIconList {  get { return sprites; } }
}
