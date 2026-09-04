using UnityEngine;

public class DecreaseFinalCoinPower : BuffDebuffEffect
{
    private int stack = 0;
    private bool isExpired = false;


    public override string effectCodeName => "DecreaseFinalCoinPower";
    public override string effectNickName => "최종위력감소";
    public override int effectPower => -1;
    public override int effectCount => stack;

    public int GetDecreseFinalCoinPowerStack { get { return stack; } }

    public void SetDecreaseFinalCoinPowerStack(int value)
    {
        stack = Mathf.Clamp(value, 0, 99);
    }

    public override void Merge(BuffDebuffEffect other)
    {
        DecreaseFinalCoinPower otherEffect = other as DecreaseFinalCoinPower;

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

