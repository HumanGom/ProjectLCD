using UnityEngine;
using UnityEngine.UI;

public class MapLine : MonoBehaviour
{
    [SerializeField] private Image lineImage;

    private RectTransform rect;

    public void Init(Vector2 startPos, Vector2 endPos)
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();

        if (lineImage == null)
            lineImage = GetComponent<Image>();

        Vector2 dir = endPos - startPos;
        float distance = dir.magnitude;

        rect.anchoredPosition = startPos + dir * 0.5f;
        rect.sizeDelta = new Vector2(distance, rect.sizeDelta.y);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rect.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}