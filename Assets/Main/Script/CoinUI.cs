using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [Header("앞면")]
    [SerializeField] private Sprite frontCoin;

    [Header("뒷면")]
    [SerializeField] private Sprite backCoin;

    [Header("코인 스프라이트")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void SetCoinSide(bool isFront)
    {
        spriteRenderer.sprite = isFront ? frontCoin : backCoin;
    }
}
