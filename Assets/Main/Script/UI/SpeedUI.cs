using TMPro;
using UnityEngine;

public class SpeedUI : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;

    public void RequestSpeedForUi(int speed)
    {
        textMeshPro.text = speed.ToString();
    }

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }
}
