using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MentalUI : MonoBehaviour
{
    [Header("정신력 색상 UI")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("정신력 수치 UI")]
    [SerializeField] private TextMeshProUGUI textMeshPro;
  
    private SpriteRenderer circleSpriteRenderer;
    private CharacterStatus characterStatus;
    private EnemyStatus enemyStatus;
    private int mentalValue;

    [Header("정신력 색상")]
    [SerializeField] private Color negativeColor = Color.red;
    [SerializeField] private Color neutralColor = new Color(0.6f, 0.9f, 1f);
    [SerializeField] private Color positiveColor = new Color(0.1f, 0.3f, 1f);

    private void SetMentalUI(int mentality)
    {
        mentality = Mathf.Clamp(mentality, -45, 45);
        textMeshPro.text = mentality.ToString();
        Color resultColor;

        if (mentality < 0)
        {
            float t = Mathf.InverseLerp(-45, 0, mentality);
            resultColor = Color.Lerp(negativeColor, neutralColor, t);
        }
        else
        {
            float t = Mathf.InverseLerp(0, 45, mentality);
            resultColor = Color.Lerp(neutralColor, positiveColor, t);
        }

        if (circleSpriteRenderer == null)
        {
            circleSpriteRenderer = spriteRenderer;
        }

        circleSpriteRenderer.color = resultColor;
    }

    public void Initialize(CharacterStatus status)
    {
        characterStatus = status;
        mentalValue = status.MentalityValue;
        SetMentalUI(mentalValue);
    }

    public void Initialize(EnemyStatus status)
    {
        enemyStatus = status;
        mentalValue = status.MentalityValue;
        SetMentalUI(mentalValue);
    }

    public void ReFreshMentalUI()
    {
        if (characterStatus != null)
        {
            SetMentalUI(characterStatus.MentalityValue);
        }
        else if (enemyStatus != null)
        {
            SetMentalUI(enemyStatus.MentalityValue);
        }
    }

}
