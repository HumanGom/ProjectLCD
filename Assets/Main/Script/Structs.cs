using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeedStat 
{ 
    public int minSpeed;
    public int maxSpeed; 
}

[System.Serializable]
public class ElementResist
{
    public ElementType elementType;
    public float resistValue;
}

[System.Serializable]
public class AttackResist
{
    public AttackType attackType;
    public float resistValue;
}

[System.Serializable]
public class Rewards
{
    public int gold;
    public List<ItemObjectOS> items;
}

[System.Serializable]
public class BuffDebuffIconData
{
    public string effectName;
    public Sprite icon;
}