using UnityEngine;

[System.Serializable]
public class DecreaseBasicPower : BuffDebuffEffect
{
    private int stack = 0;
    private bool isExpired = false;

    public override string effectCodeName => "DecreaseBasicPower";
    public override string effectNickName => "기본위력감소";
    public override int effectPower => -1;
    public override int effectCount => stack;

    public int GetDecreaseBasicPowerStack {  get { return stack; } }

    public void SetDecreaseBasicPowerStack(int value)
    {
        stack = Mathf.Clamp(value, 0, 99);
    }

    public override void Merge(BuffDebuffEffect other)
    {
        DecreaseBasicPower otherEffect = other as DecreaseBasicPower;

        if (otherEffect != null)
        {
            stack = Mathf.Clamp(stack + otherEffect.stack, 0, 99);
        }
        Debug.Log($"공격 위력 감소량{stack}");
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