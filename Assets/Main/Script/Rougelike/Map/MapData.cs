using System.Collections.Generic;

public static class MapData
{
    public static bool HasMap;
    public static List<List<RoguelikeMapNode>> SavedMap;

    public static RoguelikeMapNode CurrentNode;

    public static void SaveMap(List<List<RoguelikeMapNode>> map)
    {
        HasMap = true;
        SavedMap = map;
    }

    public static void SaveCurrentNode(RoguelikeMapNode node)
    {
        CurrentNode = node;
    }



    public static void Clear()
    {
        HasMap = false;
        SavedMap = null;
        CurrentNode = null;
    }
}
