using System.Collections;
using UnityEngine;

public class BossPart : MonoBehaviour
{
    [SerializeField] private string partID;
    [SerializeField] private BossPartType partType;
    [SerializeField] private int maxHp = 100;

    [Header("부위 체력바")]
    [SerializeField] private GameObject hpBarObject;
    [SerializeField] private BossHPBarUI hpBarUI;
    [SerializeField] private float showHpBarTime = 1.5f;

    private int shield;
    private int hp;
    private BossController boss;
    private Coroutine hpBarRoutine;

    public string PartID => partID;
    public bool IsCore => partType == BossPartType.Core;
    public bool IsBroken => hp <= 0;
    public int MaxHp => maxHp;

    public int ShieldValue { get { return shield; } set { shield = Mathf.Max(0, value); } }
    public int HpValue { get { return hp; } set { hp = Mathf.Clamp(value , 0, maxHp); } }
    public void Initialize(BossController controller)
    {
        shield = 0;
        boss = controller;
        hp = maxHp;
        if (hpBarUI != null) hpBarUI.Initialize(this);
    }

    public void TakeDamage(int damage)
    {
        if (!IsCore && hpBarObject != null) ShowHpBarTemporary();

        int resultDamage = damage;

        if (shield >= resultDamage) 
        {
            shield -= resultDamage;
        }
        else
        {
            resultDamage -= shield;
        }

        if (!IsCore && IsBroken)
        {
            boss.DamageCore(resultDamage);
            return;
        }

        hp = Mathf.Max(0, hp - resultDamage);

        if (hpBarUI != null) hpBarUI.Refresh();

        if (hp <= 0)
        {
            BuffDebuffManager manager = GetComponent<BuffDebuffManager>();
            if (manager != null) manager.ClearAllEffects();
            boss.OnPartBroken(this);
        }
    }

    private void ShowHpBarTemporary()
    {
        if (hpBarObject == null) return;

        if (hpBarRoutine != null) StopCoroutine(hpBarRoutine);

        hpBarRoutine = StartCoroutine(ShowHpBarRoutine());
    }

    private IEnumerator ShowHpBarRoutine()
    {
        hpBarObject.SetActive(true);

        yield return new WaitForSeconds(showHpBarTime);

        if (!IsCore)
            hpBarObject.SetActive(false);

        hpBarRoutine = null;
    }

    public void HealFull()
    {
        hp = maxHp;
    }
}