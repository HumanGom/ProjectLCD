[System.Serializable]
public class BossSlotCommand
{
    public string slotID;
    public SkillType skillType;
    public bool canAct = true;
    public bool useOverrideSpeed = false;
    public int overrideSpeed = 0;
    public bool isBroken = false;
    public bool ignoreBrokenThisAction = false;
}