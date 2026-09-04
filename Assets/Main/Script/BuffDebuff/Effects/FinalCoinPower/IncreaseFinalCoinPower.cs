using UnityEngine;

public class IncreaseFinalCoinPower : BuffDebuffEffect
{
    private int stack = 0;
    private bool isExpired = false;

    public override string effectCodeName => "IncreaseFinalCoinPower";
    public override string effectNickName => "최종위력감소";
    public override int effectPower => -1;
    public override int effectCount => stack;

    public int GetIncreseFinalCoinPowerStack { get { return stack; } }

    public void SetIncreaseFinalCoinPowerStack(int value)
    {
        stack = Mathf.Clamp(value, 0, 99);
    }

    public override void Merge(BuffDebuffEffect other)
    {
        IncreaseFinalCoinPower otherEffect = other as IncreaseFinalCoinPower;

        if (otherEffect != null)
        {
            stack = Mathf.Clamp(stack + otherEffect.stack, 0, 99);
        }
        Debug.Log($"코인파워 증가량{stack}");
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

