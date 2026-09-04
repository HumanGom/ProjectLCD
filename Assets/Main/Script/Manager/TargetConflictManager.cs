using System.Collections.Generic;
using UnityEngine;

public class TargetConflictManager : MonoBehaviour
{
    public static TargetConflictManager Instance { get; private set; }

    private readonly Dictionary<EnemySlot, HeadSlot> conflictMap =
        new Dictionary<EnemySlot, HeadSlot>();

    public void ResolvePlayerTarget(HeadSlot headSlot, EnemySlot newEnemySlot)
    {
        EnemySlot oldEnemySlot = headSlot.GetTargetEnemySlot;

        // 기존 대립 중이던 HeadSlot이 다른 적으로 이동한 경우
        if (oldEnemySlot != null && oldEnemySlot != newEnemySlot)
        {
            RemoveConflictByHeadSlot(headSlot);

            oldEnemySlot.RemoveHeadSlotHistory(headSlot);

            RebuildEnemyConflictOrDefault(oldEnemySlot);
        }

        newEnemySlot.RegisterHeadSlotHistory(headSlot);
        headSlot.SetTargetEnemySlot(newEnemySlot);

        RebuildEnemyConflictOrDefault(newEnemySlot);

        if (!IsHeadSlotInConflict(headSlot))
        {
            headSlot.DrawNormalLine();
        }
    }

    public void ResolveEnemyTarget(EnemySlot enemySlot, HeadSlot headSlot)
    {
        enemySlot.SetTargetHeadSlot(headSlot);

        RebuildEnemyConflictOrDefault(enemySlot);
    }

    private void RebuildEnemyConflictOrDefault(EnemySlot enemySlot)
    {
        if (enemySlot == null) return;

        HeadSlot bestHeadSlot = enemySlot.FindFirstConflictableHeadSlot();

        if (bestHeadSlot != null)
        {
            SetConflict(enemySlot, bestHeadSlot);
            return;
        }

        ClearConflict(enemySlot);
        enemySlot.RestoreDefaultTarget();
    }

    private void SetConflict(EnemySlot enemySlot, HeadSlot headSlot)
    {
        ClearConflict(enemySlot);

        conflictMap[enemySlot] = headSlot;

        enemySlot.SetTargetHeadSlot(headSlot);

        float curveHeight = 1f;

        Vector3 centerPos = headSlot.GetArrowSplineMesh.GetCurveCenterWorld(headSlot.transform.position, enemySlot.transform.position, curveHeight);


        headSlot.DrawLineToCenter(centerPos, curveHeight);
        enemySlot.DrawLineToCenter(centerPos, curveHeight);
    }

    public void ClearConflict(EnemySlot enemySlot)
    {
        if (enemySlot == null) return;

        if (conflictMap.TryGetValue(enemySlot, out HeadSlot oldHeadSlot))
        {
            if (oldHeadSlot != null) oldHeadSlot.DrawNormalLine();

            enemySlot.DrawNormalLine();

            conflictMap.Remove(enemySlot);
        }
    }

    private void RemoveConflictByHeadSlot(HeadSlot headSlot)
    {
        EnemySlot removeEnemySlot = null;

        foreach (var pair in conflictMap)
        {
            if (pair.Value == headSlot)
            {
                removeEnemySlot = pair.Key;
                break;
            }
        }

        if (removeEnemySlot != null)
        {
            conflictMap.Remove(removeEnemySlot);
            removeEnemySlot.DrawNormalLine();
        }
    }

    private bool IsHeadSlotInConflict(HeadSlot headSlot)
    {
        foreach (var pair in conflictMap)
        {
            if (pair.Value == headSlot) return true;
        }
        return false;
    }

    public void ResetPlayerSelectionAndRestoreEnemyTargets()
    {
        HeadSlot[] headSlots = FindObjectsByType<HeadSlot>(FindObjectsSortMode.None);

        foreach (HeadSlot headSlot in headSlots)
        {
            if (headSlot == null) continue;

            headSlot.ResetSlot();
        }

        EnemySlot[] enemySlots = FindObjectsByType<EnemySlot>(FindObjectsSortMode.None);

        foreach (EnemySlot enemySlot in enemySlots)
        {
            if (enemySlot == null) continue;

            enemySlot.RestoreTurnDefaultTarget();
        }
    }

    public void RedrawEnemyDefaultTargetsOnly()
    {
        conflictMap.Clear();

        EnemySlot[] enemySlots = FindObjectsByType<EnemySlot>(FindObjectsSortMode.None);

        foreach (EnemySlot enemySlot in enemySlots)
        {
            if (enemySlot == null) continue;

            enemySlot.RestoreTurnDefaultTarget();
        }
    }

    public void ClearAllLineAndConflicts()
    {
        ClearAllConflicts();

        //Debug.Log("버튼눌림");
        List<HeadSlot> playeHeadrSlotList = new List<HeadSlot>(FindObjectsByType<HeadSlot>(FindObjectsSortMode.None));
        foreach (HeadSlot playerHeadSlot in playeHeadrSlotList)
        {
            playerHeadSlot.ResetSlot();
        }

        List<EnemySlot> enemySlotList = new List<EnemySlot>(FindObjectsByType<EnemySlot>(FindObjectsSortMode.None));
        foreach (EnemySlot enemySlot in enemySlotList)
        {
            enemySlot.DrawNormalLine();
        }
    }

    public void ClearAllConflicts()
    {
        foreach (var pair in conflictMap)
        {
            if (pair.Value != null) pair.Value.DrawNormalLine();

            if (pair.Key != null) pair.Key.DrawNormalLine();
        }

        conflictMap.Clear();
    }

    private void Awake()
    {
        Instance = this;
    }
}