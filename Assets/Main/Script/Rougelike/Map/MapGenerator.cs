using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("위치 간격")]
    [SerializeField] private float xSpacing = 3f;
    [SerializeField] private float ySpacing = 2f;
    [Header("맵 패턴")]
    [SerializeField] private int[] floorPattern = { 1, 0, 0, 0, 0, 0 };

    private List<List<RoguelikeMapNode>> columns = new List<List<RoguelikeMapNode>>();

    public List<List<RoguelikeMapNode>> GenerateMap()
    {
        columns.Clear();
        SetfloorPattern();

        for (int x = 0; x < floorPattern.Length; x++)
        {
            List<RoguelikeMapNode> column = new List<RoguelikeMapNode>();

            int roomCount = floorPattern[x];

            for (int y = 0; y < roomCount; y++)
            {
                RoguelikeMapNode node = new RoguelikeMapNode();
                node.X = x;
                node.Y = y;

                node.RoomType = x == 0 ? RoguelikeRoomType.Start : GetRandomNormalRoomType();

                node.WorldPos = GetNodePosition(x, y, roomCount);

                column.Add(node);
            }

            columns.Add(column);
        }

        AddSpecialColumn(RoguelikeRoomType.Shop);
        AddSpecialColumn(RoguelikeRoomType.Boss);

        ConnectNodes();

        return columns;
    }

    public void SetfloorPattern()
    {
        for (int x = 1; x < floorPattern.Length; x++)
        {
            floorPattern[x] = Random.Range(2, 4);
        }
    }

    private Vector2 GetNodePosition(int x, int y, int roomCount)
    {
        float centerOffset = (roomCount - 1) * ySpacing * 0.5f;

        return new Vector2(x * xSpacing, -y * ySpacing + centerOffset);
    }

    private RoguelikeRoomType GetRandomNormalRoomType()
    {
        int rand = Random.Range(0, 100);

        if (rand < 25) return RoguelikeRoomType.Battle_1;

        if (25 <=rand && rand < 50) return RoguelikeRoomType.Battle_2;

        if (50 <= rand && rand < 75) return RoguelikeRoomType.Battle_3;

        return RoguelikeRoomType.Event;
    }

    private void AddSpecialColumn(RoguelikeRoomType type)
    {
        int x = columns.Count;

        RoguelikeMapNode node = new RoguelikeMapNode();
        node.X = x;
        node.Y = 0;
        node.RoomType = type;
        node.WorldPos = GetNodePosition(x, 0, 1);

        columns.Add(new List<RoguelikeMapNode>() { node });
    }

    private void ConnectNodes()
    {
        for (int x = 0; x < columns.Count - 1; x++)
        {
            List<RoguelikeMapNode> currentColumn = columns[x];
            List<RoguelikeMapNode> nextColumn = columns[x + 1];

            currentColumn.Sort((a, b) => a.Y.CompareTo(b.Y));
            nextColumn.Sort((a, b) => a.Y.CompareTo(b.Y));

            if (nextColumn.Count == 1)
            {
                foreach (RoguelikeMapNode current in currentColumn)
                {
                    AddConnection(current, nextColumn[0]);
                }

                continue;
            }

            int lastConnectedNextY = -1;

            foreach (RoguelikeMapNode current in currentColumn)
            {
                List<RoguelikeMapNode> candidates = new List<RoguelikeMapNode>();

                foreach (RoguelikeMapNode next in nextColumn)
                {
                    int yDiff = Mathf.Abs(current.Y - next.Y);

                    if (yDiff > 1) continue;

                    if (next.Y < lastConnectedNextY) continue;

                    candidates.Add(next);
                }

                if (candidates.Count == 0) continue;

                candidates.Sort((a, b) => Mathf.Abs(current.Y - a.Y).CompareTo(Mathf.Abs(current.Y - b.Y))
                );

                int connectCount = Mathf.Min(2, candidates.Count);

                for (int i = 0; i < connectCount; i++)
                {
                    RoguelikeMapNode next = candidates[i];

                    AddConnection(current, next);

                    if (next.Y > lastConnectedNextY) lastConnectedNextY = next.Y;
                }
            }

            foreach (RoguelikeMapNode next in nextColumn)
            {
                if (next.PrevNodes.Count > 0) continue;

                RoguelikeMapNode best = null;

                foreach (RoguelikeMapNode current in currentColumn)
                {
                    if (current.NextNodes.Count == 0) continue;

                    RoguelikeMapNode last = current.NextNodes[current.NextNodes.Count - 1];

                    if (last.Y <= next.Y)
                    {
                        best = current;
                    }
                }

                if (best == null) best = currentColumn[0];

                AddConnection(best, next);
            }
        }
    }

    private void AddConnection(RoguelikeMapNode from, RoguelikeMapNode to)
    {
        if (!from.NextNodes.Contains(to)) from.NextNodes.Add(to);

        if (!to.PrevNodes.Contains(from)) to.PrevNodes.Add(from);
    }

    private bool IsCrossingAny(RoguelikeMapNode from, RoguelikeMapNode to, List<(RoguelikeMapNode from, RoguelikeMapNode to)> connections)
    {
        foreach (var connection in connections)
        {
            if (IsCrossing(from, to, connection.from, connection.to)) return true;
        }

        return false;
    }

    private bool IsCrossing(RoguelikeMapNode aFrom, RoguelikeMapNode aTo, RoguelikeMapNode bFrom, RoguelikeMapNode bTo)
    {
        if (aFrom == bFrom || aTo == bTo) return false;

        bool realCross = (aFrom.Y < bFrom.Y && aTo.Y > bTo.Y) || (aFrom.Y > bFrom.Y && aTo.Y < bTo.Y);

        bool backwardOverlap = aFrom.Y > bFrom.Y && aTo.Y <= bTo.Y;

        return realCross || backwardOverlap;
    }

    private RoguelikeMapNode GetNearestNonCrossingNode(List<RoguelikeMapNode> nodes, RoguelikeMapNode target, List<(RoguelikeMapNode from, RoguelikeMapNode to)> connections)
    {
        RoguelikeMapNode best = null;
        int bestDiff = int.MaxValue;

        foreach (RoguelikeMapNode node in nodes)
        {
            int diff = Mathf.Abs(node.Y - target.Y);

            if (diff > 1) continue;

            if (IsCrossingAny(node, target, connections)) continue;

            if (diff < bestDiff)
            {
                bestDiff = diff;
                best = node;
            }
        }

        return best;
    }

    private RoguelikeMapNode GetNearestNode(List<RoguelikeMapNode> nodes, RoguelikeMapNode target)
    {
        RoguelikeMapNode nearest = nodes[0];
        int bestDiff = Mathf.Abs(nearest.Y - target.Y);

        foreach (RoguelikeMapNode node in nodes)
        {
            int diff = Mathf.Abs(node.Y - target.Y);

            if (diff < bestDiff)
            {
                bestDiff = diff;
                nearest = node;
            }
        }

        return nearest;
    }
}