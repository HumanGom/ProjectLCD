using UnityEngine;

public class IncreaseAttackLevel : BuffDebuffEffect
{
    private int stack = 0;
    private bool isExpired = false;

    public override string effectCodeName => "IncreaseAttackLevel";
    public override string effectNickName => "공격레벨증가";
    public override int effectPower => -1;
    public override int effectCount => stack;


    public int GetIncreseAttackLevelStack { get { return stack; } }

    public void SetIncreaseAttackLevelStack(int value)
    {
        stack = Mathf.Clamp(value, 0, 10);
    }

    public override void Merge(BuffDebuffEffect other)
    {
        IncreaseAttackLevel otherEffect = other as IncreaseAttackLevel;

        if (otherEffect != null)
        {
            stack = Mathf.Clamp(stack + otherEffect.stack, 0, 10);
        }
        Debug.Log($"공격레벨 증가량{stack}");
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
