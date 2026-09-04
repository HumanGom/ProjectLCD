using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonActions : MonoBehaviour
{
    TurnManager turnManager;

    public void ClickResetButton()
    {
        TargetConflictManager.Instance.ResetPlayerSelectionAndRestoreEnemyTargets();
    }

    public void ClickStartButton()
    {
        if (turnManager == null) 
        {
            turnManager = FindFirstObjectByType<TurnManager>();
        }
        
        turnManager.StartTurn();

    }
}