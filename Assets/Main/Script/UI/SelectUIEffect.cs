using UnityEngine;
using UnityEngine.EventSystems;

public class TitleButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private GameObject selectEffect;

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectEffect.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selectEffect.SetActive(false);
    }

    private void Start()
    {
        selectEffect.SetActive(false);
    }
}