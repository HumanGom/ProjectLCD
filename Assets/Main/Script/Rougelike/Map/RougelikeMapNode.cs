using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RoguelikeMapNode
{
    public int X;
    public int Y;
    public RoguelikeRoomType RoomType;
    public Vector2 WorldPos;
    public List<RoguelikeMapNode> NextNodes = new List<RoguelikeMapNode>();
    public List<RoguelikeMapNode> PrevNodes = new List<RoguelikeMapNode>();
    public bool IsReachable;
    public bool IsCleared;
    public Rewards nodeRewards;

    public void SetNodeRewards()
    {

        switch (RoomType)
        {
            case RoguelikeRoomType.Start:
                nodeRewards = null;
                break;

            case RoguelikeRoomType.Battle_1:
            case RoguelikeRoomType.Battle_2:
            case RoguelikeRoomType.Battle_3:
                nodeRewards = new Rewards();
                nodeRewards.gold = 120;
                break;

            case RoguelikeRoomType.Event:
                nodeRewards = null;
                break;

            case RoguelikeRoomType.Shop:
                nodeRewards = null;
                break;

            case RoguelikeRoomType.Boss:
                nodeRewards = new Rewards();
                nodeRewards.gold = 240;
                break;
        }
    }
}