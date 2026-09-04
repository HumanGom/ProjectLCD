using UnityEngine;

[System.Serializable]
public class BleedEffect : BuffDebuffEffect
{
    private int bleedPower;
    private int bleedCount;

    public override string effectCodeName => "Bleed";
    public override string effectNickName => "ÃâÇ÷";
    public override int effectPower => bleedPower;
    public override int effectCount => bleedCount;

    public BleedEffect(int power, int count)
    {
        bleedPower = Mathf.Clamp(power, 0, 99);
        bleedCount = Mathf.Clamp(count, 0, 99);

        if (bleedCount > 0 && bleedPower <= 0)
        {
            bleedPower = 1;
        }
    }

    public override void Merge(BuffDebuffEffect other)
    {
        BleedEffect otherBleed = other as BleedEffect;

        if (otherBleed == null) return;

        bleedPower = Mathf.Clamp(bleedPower + otherBleed.bleedPower, 0, 99);
        bleedCount = Mathf.Clamp(bleedCount + otherBleed.bleedCount, 0, 99);

        if (bleedCount > 0 && bleedPower <= 0)
        {
            bleedPower = 1;
        }

        Debug.Log($"ÃâÇ÷ º´ÇÕ / À§·Â:{bleedPower}, È½¼ö:{bleedCount}");
    }

    public override void OnAfterClash(BuffDebuffManager owner)
    {
        TriggerBleed(owner);
    }

    public override void OnAfterAttack(BuffDebuffManager owner)
    {
        TriggerBleed(owner);
    }

    private void TriggerBleed(BuffDebuffManager owner)
    {
        if (bleedPower <= 0 || bleedCount <= 0) return;

        owner.TakeFixedDamage(bleedPower);
        bleedCount--;

        Debug.Log($"{owner.name} ÃâÇ÷ ÇÇÇØ {bleedPower} / ³²Àº È½¼ö {bleedCount}");

        if (bleedCount <= 0)
        {
            bleedPower = 0;
            bleedCount = 0;
        }
    }

    public override bool IsExpired()
    {
        return bleedCount <= 0;
    }
}