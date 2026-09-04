using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI nodeName = null;

    private RoguelikeMapNode node;
    private MapManager mapManager;

    public void Init(RoguelikeMapNode node, MapManager manager)
    {
        this.node = node;
        mapManager = manager;
        node.SetNodeRewards();
        SetRoomProit();
        Refresh();
    }

    public void Refresh()
    {
        SetRoomName();
        bool canMove = mapManager.CanMoveTo(node);
        button.interactable = canMove;

        Color color = image.color;
        color.a = canMove ? 1f : 0.5f;
        image.color = color;
    }

    private void SetRoomProit()
    {
        Sprite nodeSprite = mapManager.FindEnemySpriteFromRoomType(node);

        if (nodeSprite != null)
        {
            image.sprite = nodeSprite;
            image.color = Color.white;
        }
        else
        {
            image.sprite = null;
            image.color = SetRoomColor();
        }
    }

    private void SetRoomName()
    {
        if(image.sprite != null) nodeName.text = "";
        else nodeName.text = node.RoomType.ToString();

    }

    private Color SetRoomColor()
    {
        switch (node.RoomType)
        {
            case RoguelikeRoomType.Start:
                return Color.white;

            case RoguelikeRoomType.Battle_1:
            case RoguelikeRoomType.Battle_2:
            case RoguelikeRoomType.Battle_3:
                return Color.red;

            case RoguelikeRoomType.Event:
                return Color.green;

            case RoguelikeRoomType.Shop:
                return Color.yellow;

            case RoguelikeRoomType.Boss:
                return Color.magenta;
        }

        return Color.white;
    }

    public void OnClick()
    {
        mapManager.SelectNode(node);
    }
}