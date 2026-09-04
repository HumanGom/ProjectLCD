using UnityEngine;

public class DecreaseCoinPower : BuffDebuffEffect
{
    private int stack = 0;
    private bool isExpired = false;

    public override string effectCodeName => "DecreaseCoinPower";
    public override string effectNickName => "코인위력감소";
    public override int effectPower => -1;
    public override int effectCount => stack;

    public int GetDecreseCoinPowerStack { get { return stack; } }

    public void SetDecreseCoinPowerStack(int value)
    {
        stack = Mathf.Clamp(value, 0, 99);
    }

    public override void Merge(BuffDebuffEffect other)
    {
        DecreaseCoinPower otherEffect = other as DecreaseCoinPower;

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
