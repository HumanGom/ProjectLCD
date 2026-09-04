using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] private BattleAnimationPlayer battleAnimationPlayer;

    public void OnAttackHit()
    {
        battleAnimationPlayer?.OnAttackHit();
    }
}