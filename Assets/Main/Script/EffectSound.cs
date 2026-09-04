using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class EffectSound : MonoBehaviour
{
    [Header("피격음")]
    [SerializeField] AudioClip damgedClip;
    [Header("공격음")]
    [SerializeField] AudioClip attackClip;
    [Header("합 음")]
    [SerializeField] AudioClip clashClip;
    [Header("사망음")]
    [SerializeField] AudioClip deadClip;

    private AudioSource audioSource;

    public void PlayDamagedClip()
    {
        if (damgedClip == null) return;
        audioSource.clip = damgedClip;
        audioSource.Play();
    }

    public void PlayAttackClip()
    {
        if (attackClip == null) return;
        audioSource.clip = attackClip;
        audioSource.Play();
    }

    public void PlayClashClip()
    {
        if (clashClip == null) return;
        audioSource.clip = clashClip;
        audioSource.Play();
    }

    public void PlayDeadClip()
    {
        if (deadClip == null) return;
        audioSource.clip = deadClip;
        audioSource.Play();
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
