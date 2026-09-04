using UnityEngine;

public class IncreaseCoinPower : BuffDebuffEffect
{
    private int stack = 0;
    private bool isExpired = false;

    public override string effectCodeName => "IncreaseCoinPower";
    public override string effectNickName => "코인위력증가";
    public override int effectPower => -1;
    public override int effectCount => stack;

    public int GetIncreseCoinPowerStack { get { return stack; } }

    public void SetIncreaseCoinPowerStack(int value)
    {
        stack = Mathf.Clamp(value, 0, 99);
    }

    public override void Merge(BuffDebuffEffect other)
    {
        IncreaseCoinPower otherEffect = other as IncreaseCoinPower;

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
