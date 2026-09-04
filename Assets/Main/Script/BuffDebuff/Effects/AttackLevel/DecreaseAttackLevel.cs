using UnityEngine;

public class DecreaseAttackLevel : BuffDebuffEffect
{
    private int stack = 0;
    private bool isExpired = false;

    public override string effectCodeName => "DecreaseAttackLevel";
    public override string effectNickName => "공격레벨감소";
    public override int effectPower => -1;
    public override int effectCount => stack;

    public int GetDecreaseAttackLevelStack { get { return stack; } }

    public void SetDecreaseAttackLevelStack(int value)
    {
        stack = Mathf.Clamp(value, 0, 99);
    }

    public override void Merge(BuffDebuffEffect other)
    {
        DecreaseAttackLevel otherEffect = other as DecreaseAttackLevel;

        if (otherEffect != null)
        {
            stack = Mathf.Clamp(stack + otherEffect.stack, 0, 99);
        }
        Debug.Log($"공격레벨 감소량{stack}");
    }


    public override void OnEndTurn(BuffDebuffManager owner)
    {
        isExpired = true;
    }

    public override bool IsExpired()
    {
        return isExpired;
    }
}

