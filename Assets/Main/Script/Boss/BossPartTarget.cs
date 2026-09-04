using UnityEngine;

public class BossPartTarget : MonoBehaviour
{
    [SerializeField] private BossPart bossPart;

    public BossPart BossPart => bossPart;
}