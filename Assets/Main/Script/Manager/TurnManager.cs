using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TurnManager : MonoBehaviour
{
    [Header("캐릭터 매니저")]
    [SerializeField] private CharactersManager charactersManager;
    [Header("캐릭터 스포너")]
    [SerializeField] private CharacterSpawner characterSpawner;
    [Header("적 매니저")]
    [SerializeField] private EnemysManager enemysManager;
    [Header("적 스포너")]
    [SerializeField] private EnemySpawner enemySpawner;
    [Header("라운드 매니저")]
    [SerializeField] private RoundManager roundManager;
    [Header("전투 연출 매니저")]
    [SerializeField] private BattleAnimationManager battleAnimationManager;
    [Header("아군 캐릭터 데미지UI")]
    [SerializeField] private DamageUI damageUI;
    [Header("효과 아이콘 목록")]
    [SerializeField] private BuffDebuffIconListOS icons;
    [Header("전투 카메라 매니저")]
    [SerializeField] private BattleCameraManager battleCameraManager;
    [Header("전투 결과 UI 지속시간")]
    [SerializeField] private float second = 5f;


    public BuffDebuffIconListOS GetBuffDebuffImageList { get { return icons; } }

    public void NextTurn(List<BattleActionContext> battleActions)
    {
        StartCoroutine(NextTurnRoutine(battleActions));
    }

    private IEnumerator NextTurnRoutine(List<BattleActionContext> battleActions)
    {
        foreach (BattleActionContext action in battleActions)
        {
            TriggerEndTurn(action);
        }

        Debug.Log("--------다음턴--------");

        TargetConflictManager.Instance.ClearAllLineAndConflicts();

        charactersManager.ShowControlPenal();
        charactersManager.RequestSetSpeedAndSlot();

        yield return null; // HeadSlot 생성 반영 대기

        BossController boss = FindFirstObjectByType<BossController>();

        if (boss != null) boss.SetNextPattern();

        HeadSlot[] headSlots = FindObjectsByType<HeadSlot>(FindObjectsSortMode.None);

        foreach (HeadSlot headSlot in headSlots)
        {
            if (headSlot == null) continue;

            headSlot.ResetSlot();
        }

        enemysManager.SetAllEnemyTartget();

        //yield return null; // 적 타겟/선 그리기 반영 대기

        TargetConflictManager.Instance.ResetPlayerSelectionAndRestoreEnemyTargets();

        if (GoodsManager.Instance != null)
        {
            GoodsManager.Instance.OnBeforeTurnStart();
        }

        TurnOnAllHeadSlotAndEnemySlot();

        battleCameraManager.ResetCamera();
        battleCameraManager.SetManualControl(true);
    }
    public void StartTurn()
    {
        if (CheckCanStart())
        {
            battleCameraManager.SetManualControl(false);
            battleCameraManager.ResetCamera();

            charactersManager.HideControlPenal();
            TurnOffAllHeadSlotAndEnemySlot();
            StartCoroutine(StartTurnRoutine());
        }
    }

    private bool CheckCanStart()
    {
        List<HeadSlot> headSlots = new List<HeadSlot>();

        foreach(GameObject character in charactersManager.GetCharacterList)
        {
            if (character == null) continue;

            HeadSlot[] slots = character.GetComponentsInChildren<HeadSlot>();

            headSlots.AddRange(slots);
        }

        if (headSlots.Count == 0)
        {
            Debug.Log("시작 불가: 아군 슬롯이 없습니다.");
            return false;
        }

        foreach (HeadSlot headSlot in headSlots)
        {
            if (!headSlot.HasTarget())
            {
                Debug.Log("시작 불가: 아직 지정되지 않은 아군 슬롯이 있습니다.");
                return false;
            }
        }

        return true;
    }

    private void TurnOffAllHeadSlotAndEnemySlot()
    {
        HeadSlot[] headSlots = FindObjectsByType<HeadSlot>(FindObjectsSortMode.None);
        EnemySlot[] enemySlots = FindObjectsByType<EnemySlot>(FindObjectsSortMode.None);

        foreach(HeadSlot headSlot in headSlots)
        {
            headSlot.TurnOffImage();
        }
        foreach(EnemySlot enemySlot in enemySlots)
        {
            enemySlot.TurnOffImage();
        }
    }

    private void TurnOnAllHeadSlotAndEnemySlot()
    {
        HeadSlot[] headSlots = FindObjectsByType<HeadSlot>(FindObjectsSortMode.None);
        EnemySlot[] enemySlots = FindObjectsByType<EnemySlot>(FindObjectsSortMode.None);

        foreach (HeadSlot headSlot in headSlots)
        {
            headSlot.TurnOnImage();
        }
        foreach (EnemySlot enemySlot in enemySlots)
        {
            enemySlot.TurnOnImage();
        }
    }

    private IEnumerator StartTurnRoutine()
    {
        List<BattleActionContext> actions = CollectBattleActions();
        actions.Sort((a, b) => b.Speed.CompareTo(a.Speed));

        foreach (BattleActionContext action in actions)
        {
            TriggerStartTurn(action);
        }

        if(GoodsManager.Instance != null)
        {
            GoodsManager.Instance.OnTurnStart(actions);
        }

        foreach (BattleActionContext action in actions)
        {
            if (action.IsActed) continue;
            if (action.Skill.SkillType == SkillType.Defense) continue;

            BattleActionContext clashTarget = FindClashTarget(action, actions);

            if (clashTarget != null)
            {
                yield return ResolveClashRoutine(action, clashTarget, actions);

                action.IsActed = true;
                clashTarget.IsActed = true;
            }
            else
            {
                yield return ResolveOneSideAttackRoutine(action, actions);

                action.IsActed = true;
            }
        }

        if(charactersManager.IsAllCharatersDead())
        {
            OnBattleLose();
            yield break;
        }

        if (enemysManager.IsAllEnemyDead())
        {
            OnBattleWin();
            yield break;
        }

        NextTurn(actions);
    }

    private List<BattleActionContext> CollectBattleActions()
    {
        List<BattleActionContext> actions = new List<BattleActionContext>();

        HeadSlot[] headSlots = FindObjectsByType<HeadSlot>(FindObjectsSortMode.None);
        EnemySlot[] enemySlots = FindObjectsByType<EnemySlot>(FindObjectsSortMode.None);

        foreach (HeadSlot headSlot in headSlots)
        {
            if (!headSlot.HasSkill()) continue;

            CharacterSkillManager skillManager = headSlot.GetSkillManager;
            int usedSkillIndex = headSlot.GetUsedSkillIndex;

            if (skillManager == null || usedSkillIndex < 0) continue;

            SkillType usedSkillType = skillManager.UseSkill(usedSkillIndex, headSlot.GetUsedDeckIndex);
            SkillObjectOS skill = headSlot.GetSkillObject;
            CharacterStatus caster = headSlot.GetCharacterStatus;

            BattleActionContext action = new BattleActionContext();
            action.Side = BattleSide.Player;
            action.IsCasterBoss = false;
            action.HeadSlot = headSlot;
            action.Skill = skill;
            action.Speed = headSlot.GetCharacterSpeed();
            
            if (skill == null) continue;
            if (skill.GetSkillCoinList == null) continue;

            action.RemainingCoins = new List<SkillCoinOS>(skill.GetSkillCoinList);
            action.Context = new SkillContext();
            action.Context.CasterCharacter = caster;
            action.Context.HeadSlot = headSlot;
            action.Context.EnemySlot = headSlot.GetTargetEnemySlot;
            action.Context.CastingOBJ = caster.gameObject;
     
            if (headSlot.GetTargetEnemySlot != null)
            {
                action.Context.TargetEnemy = headSlot.GetTargetEnemySlot.GetComponentInParent<EnemyStatus>();
            }

            actions.Add(action);
        }

        foreach (EnemySlot enemySlot in enemySlots)
        {
            if (!enemySlot.CanActValue) continue;

            SkillObjectOS skill = enemySlot.GetSkillObject;
            EnemyStatus caster = enemySlot.GetEnemyStatus;

            if (skill == null || caster == null) continue;

            BattleActionContext action = new BattleActionContext();
            action.Side = BattleSide.Enemy;
            action.EnemySlot = enemySlot;
            action.Skill = skill;
            action.Speed = enemySlot.GetEnemySpeed();
            action.RemainingCoins = new List<SkillCoinOS>(skill.GetSkillCoinList);
            action.IgnoreBrokenThisAction = enemySlot.IgnoreBrokenThisActionValue;

            action.Context = new SkillContext();
            action.Context.CasterEnemy = caster;
            action.Context.EnemySlot = enemySlot;
            action.Context.HeadSlot = enemySlot.GetTargetHeadSlot;

            BossPartTarget bossPartTarget = enemySlot.GetComponent<BossPartTarget>();

            if (bossPartTarget == null) bossPartTarget = enemySlot.GetComponentInParent<BossPartTarget>();

            if (bossPartTarget != null && bossPartTarget.BossPart != null)
            {
                action.Context.CastingOBJ = bossPartTarget.BossPart.gameObject;
                action.IsCasterBoss = true;
            }
            else
            {
                action.Context.CastingOBJ = caster.gameObject;
                action.IsCasterBoss = false;
            }

            if (enemySlot.GetTargetHeadSlot != null)
            {
                action.Context.TargetCharacter = enemySlot.GetTargetHeadSlot.GetCharacterStatus;
            }

            actions.Add(action);
        }
        return actions;
    }

    private BattleActionContext FindClashTarget(BattleActionContext action, List<BattleActionContext> actions)
    {
        if (action.Skill.SkillType == SkillType.Defense) return null;
        if (!action.Skill.GetcanClash) return null;

        foreach (BattleActionContext other in actions)
        {
            if (other == action) continue;

            if (other.IsActed) continue;

            if (other.Skill.SkillType == SkillType.Defense) continue;

            if (!other.Skill.GetcanClash) continue;

            if (IsClashing(action, other)) return other;
        }

        return null;
    }

    private BattleActionContext FindDefenseActionForTarget(BattleActionContext attacker, List<BattleActionContext> actions)
    {
        foreach (BattleActionContext action in actions)
        {
            if (!(action.Skill.SkillType == SkillType.Defense)) continue;

            if (action.IsActed) continue;

            if (attacker.Context.TargetCharacter != null &&
                action.Context.CasterCharacter == attacker.Context.TargetCharacter)
                return action;

            if (attacker.Context.TargetEnemy != null &&
                action.Context.CasterEnemy == attacker.Context.TargetEnemy)
                return action;
        }

        return null;
    }

    private bool IsClashing(BattleActionContext a, BattleActionContext b)
    {
        if (a.Side == b.Side) return false;

        BattleActionContext playerAction = a.Side == BattleSide.Player ? a : b;
        BattleActionContext enemyAction = a.Side == BattleSide.Enemy ? a : b;

        if (playerAction.HeadSlot == null || enemyAction.EnemySlot == null) return false;

        bool playerTargetsEnemy = playerAction.HeadSlot.GetTargetEnemySlot == enemyAction.EnemySlot;
        bool enemyTargetsPlayer = enemyAction.EnemySlot.GetTargetHeadSlot == playerAction.HeadSlot;

        return playerTargetsEnemy && enemyTargetsPlayer;
    }
    

    private void MentalBounus(BattleActionContext battleActionContext)
    {
        if(battleActionContext.Context.CasterCharacter != null)
        {
            battleActionContext.Context.CasterCharacter.MentalityValue += 10;
        }
        else if (battleActionContext.Context.CasterEnemy != null)
        {
            battleActionContext.Context.CasterEnemy.MentalityValue += 10;
        }
    }

    private int RollSkillPower(BattleActionContext action, out List<bool> coinResults)
    {
        BuffDebuffManager buffDebuffManager = action.Context.CastingOBJ.GetComponent<BuffDebuffManager>();
        coinResults = new List<bool>();
        if (buffDebuffManager == null)
        {
            Debug.Log("버프매니저가 존재하지않음");
        }
        int totalPower = action.Skill.GetBasicSkillPower;

        if(buffDebuffManager.GetEffect<IncreaseBasicPower>() != null)
        {
            totalPower += buffDebuffManager.GetEffect<IncreaseBasicPower>().GetIncreaseBasicPowerStack;
        }

        if (buffDebuffManager.GetEffect<DecreaseBasicPower>() != null)
        {
            totalPower -= buffDebuffManager.GetEffect<DecreaseBasicPower>().GetDecreaseBasicPowerStack;
        }

        foreach (SkillCoinOS coin in action.RemainingCoins)
        {
            bool isFront = CoinToss(action.Context);
            coinResults.Add(isFront);

            if (isFront)
            {
                totalPower += action.Skill.GetcoinPower;
                if (buffDebuffManager.GetEffect<IncreaseCoinPower>() != null)
                {
                    totalPower += buffDebuffManager.GetEffect<IncreaseCoinPower>().GetIncreseCoinPowerStack;
                }

                if (buffDebuffManager.GetEffect<DecreaseCoinPower>() != null)
                {
                    totalPower -= buffDebuffManager.GetEffect<DecreaseCoinPower>().GetDecreseCoinPowerStack;
                }
            }
        }

        totalPower += action.Skill.GetfinalCoinPower; 

        if (buffDebuffManager.GetEffect<IncreaseFinalCoinPower>() != null)
        {
            totalPower += buffDebuffManager.GetEffect<IncreaseFinalCoinPower>().GetIncreseFinalCoinPowerStack;
        }

        if (buffDebuffManager.GetEffect<DecreaseFinalCoinPower>() != null)
        {
            totalPower -= buffDebuffManager.GetEffect<DecreaseFinalCoinPower>().GetDecreseFinalCoinPowerStack;
        }

        return totalPower;
    }

    private bool CoinToss(SkillContext context)
    {
        int casterMentality = 0;
        if (context.CasterCharacter != null)
        {
            casterMentality = context.CasterCharacter.MentalityValue;
        }
        else if (context.CasterEnemy != null)
        {
            casterMentality = context.CasterEnemy.MentalityValue;
        }

        float successChance = 50f + casterMentality;

        bool isCoinFront = Random.Range(0f, 100f) < successChance;

        return isCoinFront;
    }

    
    private IEnumerator ResolveClashRoutine(BattleActionContext a, BattleActionContext b, List<BattleActionContext> actions)
    {
        if (a.IsCasterDead || b.IsCasterDead) yield break;

        yield return battleAnimationManager.BeginFocus(a, b);

        while (a.RemainingCoins.Count > 0 && b.RemainingCoins.Count > 0)
        {
            TriggerBeforeClash(a);
            TriggerBeforeClash(b);

            List<bool> aCoinResults;
            List<bool> bCoinResults;

            int aPower = RollSkillPower(a, out aCoinResults);
            int bPower = RollSkillPower(b, out bCoinResults);

            yield return battleAnimationManager.PlayClash(a, b, aPower, bPower, aCoinResults, bCoinResults);

            TriggerAfterClash(a);
            TriggerAfterClash(b);

            if (a.IsCasterDead || b.IsCasterDead) yield break;

            if (aPower > bPower)
            {
                b.RemainingCoins.RemoveAt(0);
                yield return battleAnimationManager.PlayCoinBreakResult(a, b);
            }
            else if (bPower > aPower)
            {
                a.RemainingCoins.RemoveAt(0);
                yield return battleAnimationManager.PlayCoinBreakResult(b, a);
            }
        }

        if (a.IsCasterDead || b.IsCasterDead)
        {
            yield return battleAnimationManager.EndFocus();
            yield break;
        }

        if (a.RemainingCoins.Count > 0)
        {
            //Debug.Log($"{a.Side}가 {b.Side}를 이김(남은코인{a.RemainingCoins.Count}개)");
            MentalBounus(a);
            GetUsingSKillUIFromContext(b).Hide();
            yield return ExecuteAttackRoutine(a);
        }
        else if (b.RemainingCoins.Count > 0)
        {
            //Debug.Log($"{b.Side}가 {a.Side}를 이김(남은코인{b.RemainingCoins.Count}개)");
            MentalBounus(b);
            GetUsingSKillUIFromContext(a).Hide();
            yield return ExecuteAttackRoutine(b);
        }
        yield return battleAnimationManager.EndFocus();
    }

    private IEnumerator ResolveOneSideAttackRoutine(BattleActionContext action, List<BattleActionContext> actions)
    {
        if (action == null || action.IsCasterDead || action.IsTargetDead) yield break;

        BattleActionContext defenseAction = FindDefenseActionForTarget(action, actions);

        yield return battleAnimationManager.BeginAttackFocus(action);

        if (defenseAction != null)
        {
            yield return ExecuteDefenseRoutine(defenseAction);
            defenseAction.IsActed = true;
        }

        if (!action.IsCasterDead && !action.IsTargetDead) yield return ExecuteAttackRoutine(action);

        yield return battleAnimationManager.EndFocus();
    }

    private IEnumerator ExecuteAttackRoutine(BattleActionContext action)
    {
        //Debug.Log($"공격 루틴 진입: {action.Skill.SkillType}, 코인 수:{action.RemainingCoins.Count}");
        if (action == null || action.IsCasterDead || action.IsTargetDead) yield break;

        if (action.RemainingCoins == null || action.RemainingCoins.Count == 0) yield break;

        //Debug.Log($"공격 루틴 시작: {action.Context.CastingOBJ.name}, 코인 수 {action.RemainingCoins.Count}");
        BuffDebuffManager buffDebuffManager = action.Context.CastingOBJ.GetComponent<BuffDebuffManager>();
        if (buffDebuffManager == null)
        {
            Debug.Log("버프매니저가 존재하지않음");
        }

        TriggerBeforeAttack(action);

        if (action.Side == BattleSide.Player)
        {
            damageUI.BeginDamage();
        }

        int totalPower = action.Skill.GetBasicSkillPower;

        if (buffDebuffManager.GetEffect<IncreaseBasicPower>() != null)
        {
            totalPower += buffDebuffManager.GetEffect<IncreaseBasicPower>().GetIncreaseBasicPowerStack;
        }
        if (buffDebuffManager.GetEffect<DecreaseBasicPower>() != null)
        {
            totalPower -= buffDebuffManager.GetEffect<DecreaseBasicPower>().GetDecreaseBasicPowerStack;
        }

        SkillCoinOS coin = action.RemainingCoins[0];
        UsingSkillUI usingSkillUI = GetUsingSKillUIFromContext(action);
        bool isFront = false;

        for (int i = 0; i < action.RemainingCoins.Count - 1; i++)
        {
            coin = action.RemainingCoins[i];
            if (action.IsCasterDead)
            {
                usingSkillUI.Hide();
                if (action.Side == BattleSide.Player) damageUI.EndDamage();
                yield break;
            }
                

            isFront = CoinToss(action.Context);

            if (isFront)
            {
                totalPower += action.Skill.GetcoinPower;

                if (buffDebuffManager.GetEffect<IncreaseCoinPower>() != null)
                {
                    totalPower += buffDebuffManager.GetEffect<IncreaseCoinPower>().GetIncreseCoinPowerStack;
                }

                if (buffDebuffManager.GetEffect<DecreaseCoinPower>() != null)
                {
                    totalPower -= buffDebuffManager.GetEffect<DecreaseCoinPower>().GetDecreseCoinPowerStack;
                }
            }

            SkillCoinOS lastCoin = coin;
            int lastPower = totalPower;

            yield return battleAnimationManager.PlayCoinAction(
                action,
                lastCoin,
                lastPower,
                isFront,
                i,
                () => ExecuteCoinHit(action, lastCoin, lastPower)
            );

            if (action.IsTargetDead)
            {
                usingSkillUI.Hide();
                if (action.Side == BattleSide.Player) damageUI.EndDamage();
                yield break;
            }
            TriggerAfterAttack(action);
        }

        coin = action.RemainingCoins[action.RemainingCoins.Count - 1];

        if (action.IsCasterDead || action.IsTargetDead)
        {
            usingSkillUI.Hide();
            if (action.Side == BattleSide.Player) damageUI.EndDamage();
            yield break;
        }
           

        isFront = CoinToss(action.Context);

        if (isFront)
        {
            totalPower += action.Skill.GetcoinPower;

            if (buffDebuffManager.GetEffect<IncreaseCoinPower>() != null)
            {
                totalPower += buffDebuffManager.GetEffect<IncreaseCoinPower>().GetIncreseCoinPowerStack;
            }

            if (buffDebuffManager.GetEffect<DecreaseCoinPower>() != null)
            {
                totalPower -= buffDebuffManager.GetEffect<DecreaseCoinPower>().GetDecreseCoinPowerStack;
            }
        }

        totalPower += action.Skill.GetfinalCoinPower;

        if (buffDebuffManager.GetEffect<IncreaseFinalCoinPower>() != null)
        {
            totalPower += buffDebuffManager.GetEffect<IncreaseFinalCoinPower>().GetIncreseFinalCoinPowerStack;
        }

        if (buffDebuffManager.GetEffect<DecreaseFinalCoinPower>() != null)
        {
            totalPower -= buffDebuffManager.GetEffect<DecreaseFinalCoinPower>().GetDecreseFinalCoinPowerStack;
        }

        SkillCoinOS currentCoin = coin;
        int currentPower = totalPower;

        yield return battleAnimationManager.PlayCoinAction(
            action,
            currentCoin,
            currentPower,
            isFront,
            action.RemainingCoins.Count - 1,
            () => ExecuteCoinHit(action, currentCoin, currentPower)
        );

        //Debug.Log($"코인 실행: {coin.name}, 위력:{totalPower}, 대상:{action.Context.TargetCharacter}");
        usingSkillUI.Hide();

        if (action.IsTargetDead)
        {
            usingSkillUI.Hide();
            if (action.Side == BattleSide.Player) damageUI.EndDamage();
            yield break;
        }

        if (action.Side == BattleSide.Player)
        {
            yield return new WaitForSeconds(0.5f);
            damageUI.EndDamage();
        }

        TriggerAfterAttack(action);
    }

    private IEnumerator ExecuteDefenseRoutine(BattleActionContext action)
    {
        int totalPower = action.Skill.GetBasicSkillPower;

        UsingSkillUI usingSkillUI = GetUsingSKillUIFromContext(action);

        for (int i = 0; i < action.RemainingCoins.Count; i++)
        {
            SkillCoinOS coin = action.RemainingCoins[i];
            bool isFront = CoinToss(action.Context);

            if (isFront)
            {
                totalPower += action.Skill.GetcoinPower;
            }

            if (i == action.RemainingCoins.Count - 1)
            {
                totalPower += action.Skill.GetfinalCoinPower;
            }

            yield return battleAnimationManager.PlayDefenseAction(action, coin, totalPower, isFront, i, null);
        }

        if (usingSkillUI != null) usingSkillUI.Hide();
    }

    private void SaveCharacterState()
    {
        BattleSceneData.CharactersClear();

        foreach (GameObject character in charactersManager.GetCharacterList)
        {
            CharacterStatus status = character.GetComponent<CharacterStatus>();
            CharacterSaveData saveData = new CharacterSaveData();

            saveData.Status = status;
            saveData.CharacterName = status.GetName; 
            saveData.Hp = status.HpValue;
            saveData.Mentality = status.MentalityValue;
           

            BattleSceneData.CharactersData.Add(saveData);
        }
    }

    private void RequestGameResultUI(bool isWin, string sceneName)
    {
        GameResultManager gameResultManager = FindFirstObjectByType<GameResultManager>();
        if (gameResultManager != null) gameResultManager.OnGameResult(second, isWin, sceneName);
    }

    private void ExecuteCoinHit(BattleActionContext action, SkillCoinOS coin, int power)
    {
        int dealtDamage = coin.Execute(action, power);
        if (action.Side == BattleSide.Player)
        {
            damageUI.AddDamage(dealtDamage);
        }
    }

    public void OnBattleWin()
    {
        SaveCharacterState();

        if (MapData.SavedMap != null)
        {
            if (GoodsManager.Instance != null)
            {
                GoodsManager.Instance.SaveRewardsValue = MapData.CurrentNode.nodeRewards;
                GoodsManager.Instance.OnBattleEnd();
            }

            MapData.CurrentNode.IsCleared = true;
            if(MapData.CurrentNode.RoomType == RoguelikeRoomType.Boss)
            {
                Debug.Log("보스 전투 승리");
                MapData.Clear();
                RequestGameResultUI(true, "TitleScene");
                return;
            }

            foreach (RoguelikeMapNode next in MapData.CurrentNode.NextNodes)
            {
                next.IsReachable = true;
            }
        }

        RequestGameResultUI(true, "TestRougelike");
    }

    public void OnBattleLose()
    {
        BattleSceneData.RestoreBeforeBattle();
        if (GoodsManager.Instance != null)
        {
            GoodsManager.Instance.OnBattleEnd();
        }

        RequestGameResultUI(false, "TestRougelike");
    }

    /*----------------------------------------------------------------------*/

    private void TriggerBeforeClash(BattleActionContext action)
    {
        if (action.Context.CastingOBJ == null) return;

        BuffDebuffManager manager = action.Context.CastingOBJ.GetComponent<BuffDebuffManager>();

        if (manager != null) manager.OnBeforeClash();
    }

    private void TriggerBeforeAttack(BattleActionContext action)
    {
        if (action.Context.CastingOBJ == null) return;

        BuffDebuffManager manager = action.Context.CastingOBJ.GetComponent<BuffDebuffManager>();

        if (manager != null) manager.OnBeforeAttack();
    }

    private void TriggerAfterClash(BattleActionContext action)
    {
        if (action.Context.CastingOBJ == null) return;

        BuffDebuffManager manager = action.Context.CastingOBJ.GetComponent<BuffDebuffManager>();

        if (manager != null) manager.OnAfterClash();
    }

    private void TriggerAfterAttack(BattleActionContext action)
    {
        if (action.Context.CastingOBJ == null) return;

        BuffDebuffManager manager = action.Context.CastingOBJ.GetComponent<BuffDebuffManager>();

        if (manager != null) manager.OnAfterAttack();
    }

    private void TriggerStartTurn(BattleActionContext action)
    {
        if (action.Context.CastingOBJ == null) return;

        BuffDebuffManager manager = action.Context.CastingOBJ.GetComponent<BuffDebuffManager>();

        if (manager != null) manager.OnStartTurn();
    }

    private void TriggerEndTurn(BattleActionContext action)
    {
        if (action.Context.CastingOBJ == null) return;

        BuffDebuffManager manager = action.Context.CastingOBJ.GetComponent<BuffDebuffManager>();

        if (manager != null) manager.OnEndTurn();
    }

    private Transform GetActorTransform(BattleActionContext action)
    {
        if (action == null || action.Context == null || action.Context.CastingOBJ == null) return null;

        return action.Context.CastingOBJ.transform;
    }

    private Transform GetTargetTransform(BattleActionContext action)
    {
        if (action.Context.TargetCharacter != null) return action.Context.TargetCharacter.transform;

        if (action.Context.TargetEnemy != null) return action.Context.TargetEnemy.transform;

        return null;
    }

    private UsingSkillUI GetUsingSKillUIFromContext(BattleActionContext actionContext)
    {
        if (actionContext == null || actionContext.Context == null)
            return null;

        if (actionContext.Side == BattleSide.Player &&
            actionContext.Context.CasterCharacter != null)
        {
            return actionContext.Context.CasterCharacter.GetUsingSkillUI;
        }

        if (actionContext.Side == BattleSide.Enemy &&
            actionContext.Context.CasterEnemy != null)
        {
            return actionContext.Context.CasterEnemy.GetUsingSkillUI;
        }

        return null;
    }
}