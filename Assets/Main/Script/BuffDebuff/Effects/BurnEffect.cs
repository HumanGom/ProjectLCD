using UnityEngine;

[System.Serializable]
public class BurnEffect : BuffDebuffEffect
{
    private int burnPower;
    private int burnCount;

    public override string effectCodeName => "Burn";
    public override string effectNickName => "화상";
    public override int effectPower => burnPower;
    public override int effectCount => burnCount;

    public void SetBurnEffect(int power, int count)
    {
        burnPower = Mathf.Clamp(power, 0, 99);
        burnCount = Mathf.Clamp(count, 0, 99);

        if (burnCount > 0 && burnPower <= 0)
        {
            burnPower = 1;
        }

        if (burnPower > 0 && burnCount <= 0)
        {
            burnCount = 1;
        }
    }

    public override void Merge(BuffDebuffEffect other)
    {
        BurnEffect otherBurn = other as BurnEffect;

        if (otherBurn == null) return;

        burnPower = Mathf.Clamp(burnPower + otherBurn.burnPower, 0, 99);
        burnCount = Mathf.Clamp(burnCount + otherBurn.burnCount, 0, 99);

        if (burnCount > 0 && burnPower <= 0)
        {
            burnPower = 1;
        }

        //Debug.Log($"화상 병합 / 위력:{burnPower}, 횟수:{burnCount}");
    }

    public override void OnEndTurn(BuffDebuffManager owner)
    {
        TriggerBurn(owner);
    }

    private void TriggerBurn(BuffDebuffManager owner)
    {
        
        if (burnPower <= 0 || burnCount <= 0) return;

        owner.TakeFixedDamage(burnPower);
        burnCount--;

        //Debug.Log($"{owner.name} 화상 피해 {burnPower} / 남은 횟수 {burnCount}");

        if (burnCount <= 0)
        {
            burnPower = 0;
            burnCount = 0;
        }
    }

    public override bool IsExpired()
    {
        return burnCount <= 0;
    }
}
