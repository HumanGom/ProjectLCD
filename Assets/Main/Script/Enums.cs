using UnityEngine;

[System.Serializable]
public enum ElementType
{
    Fire,
    Water,
    Earth,
    Electric,
    Ice,
    Wind,
    None
}

[System.Serializable]
public enum AttackType
{
    Cut,
    Punch,
    Pierce,
    None
}

[System.Serializable]
public enum SkillType
{
    Defense,
    Skill1,
    Skill2, 
    Skill3,
    Skill1_1,
    Skill2_1,
    Skill3_1,
    Skill1_2,
    Skill2_2,
    Skill3_2,
    None
}

[System.Serializable]
public enum BattleSide
{
    Player,
    Enemy,
    None
}

[System.Serializable]
public enum RoguelikeRoomType
{
    Start,
    Battle_1,
    Battle_2,
    Battle_3,
    Event,
    Shop,
    Boss,
    Null
}

[System.Serializable]
public enum ItemKeyword
{
    None,
    Fire,
    Water,
    Earth,
    Electric,
    Ice,
    Wind,
    Cut,
    Punch,
    Pierce
}

[System.Serializable]
public enum BossPartType
{
    Core,
    Head,
    LeftHand,
    RightHand,
    LeftLeg,
    RightLeg,
    LeftEye,
    RightEye,
}

[System.Serializable]
public enum EffectTurnSet
{
    This,
    Next,
    Both
}
[System.Serializable]
public enum EffectTargetSet
{
    Target,
    Caster
}