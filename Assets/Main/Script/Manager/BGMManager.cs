using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }


    [Header("오디오")]
    [SerializeField] private AudioSource audioSource;

    [Header("보스BGM")]
    [SerializeField] private AudioClip bossBattleClip;
    [Header("일반전투BGM")]
    [SerializeField] private AudioClip nomalBattleClip;
    [Header("상점BGM")]
    [SerializeField] private AudioClip shopClip;
    [Header("맵BGM")]
    [SerializeField] private AudioClip rougeLikeClip;


    public void RequestChangeBGM(bool isTurnOn)
    {
        if (isTurnOn) SetBGMByRoomType(BattleSceneData.roguelikeRoomType);
        else audioSource.Stop();
    }

    private IEnumerator PlayAudioAndWait(AudioSource audioSource, AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();

        while (audioSource.isPlaying)
        {
            yield return null;
        }


    }

    private void SetBGMByRoomType(RoguelikeRoomType roomType)
    {
        switch(roomType)
        {
            case RoguelikeRoomType.Shop:
                audioSource.clip = shopClip;
                break;
            case RoguelikeRoomType.Battle_1:
            case RoguelikeRoomType.Battle_2:
            case RoguelikeRoomType.Battle_3:
                audioSource.clip = nomalBattleClip;
                break;
            case RoguelikeRoomType.Boss:
                audioSource.clip = bossBattleClip;
                break;
            default:
                audioSource.clip = rougeLikeClip;
                break;
        }

        if (audioSource.clip == null) return;
        Debug.Log($"{audioSource.clip.name}재생중");
        audioSource.Play();
    }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
