using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuffUIManager : MonoBehaviour
{
    [Header("버프/디버프 매니저")]
    [SerializeField] private BuffDebuffManager buffDebuffManager;
    [Header("UI 생성 Root")]
    [SerializeField] private Transform uiRoot;
    [Header("UI Prefab")]
    [SerializeField] private GameObject uiPrefab;
 
    private TurnManager turnManager;
    private readonly Dictionary<BuffDebuffEffect, BuffDebuffUI> uiMap = new Dictionary<BuffDebuffEffect, BuffDebuffUI>();


    private void Awake()
    {
        if (buffDebuffManager == null) buffDebuffManager = GetComponentInParent<BuffDebuffManager>();
        if (turnManager == null) turnManager = FindFirstObjectByType<TurnManager>();
    }

    private void OnEnable()
    {
        if (buffDebuffManager == null) return;

        buffDebuffManager.OnEffectAdded += AddEffectUI;
        buffDebuffManager.OnEffectChanged += RefreshEffectUI;
        buffDebuffManager.OnEffectRemoved += RemoveEffectUI;
    }

    private void OnDisable()
    {
        if (buffDebuffManager == null) return;

        buffDebuffManager.OnEffectAdded -= AddEffectUI;
        buffDebuffManager.OnEffectChanged -= RefreshEffectUI;
        buffDebuffManager.OnEffectRemoved -= RemoveEffectUI;
    }

    private void AddEffectUI(BuffDebuffEffect effect)
    {
        if (uiMap.ContainsKey(effect)) return;

        GameObject uiObj = Instantiate(uiPrefab, uiRoot);
        BuffDebuffUI ui = uiObj.GetComponent<BuffDebuffUI>();
        ui.Initialize(effect, FindIcon(effect.effectCodeName));

        uiMap.Add(effect, ui);
    }

    private void RefreshEffectUI(BuffDebuffEffect effect)
    {
        if (uiMap.TryGetValue(effect, out BuffDebuffUI ui))
        {
            ui.Refresh();
        }
    }

    private void RemoveEffectUI(BuffDebuffEffect effect)
    {
        if (!uiMap.TryGetValue(effect, out BuffDebuffUI ui)) return;

        Destroy(ui.gameObject);
        uiMap.Remove(effect);
    }

    private Sprite FindIcon(string effectName)
    {
        if (turnManager == null) return null;
        List<Sprite> sprites = turnManager.GetBuffDebuffImageList.GetIconList;
        foreach (Sprite icon in sprites)
        {
            Debug.Log($"{icon.name}");
            if (icon.name == effectName) return icon;
        }
        return null;
    }
}