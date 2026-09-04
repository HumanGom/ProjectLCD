using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [Header("생성기")]
    [SerializeField] private MapGenerator generator;
    [Header("UI")]
    [SerializeField] private RectTransform mapRoot;
    [SerializeField] private MapNode nodePrefab;
    [SerializeField] private RectTransform lineRoot;
    [SerializeField] private MapLine linePrefab;
    [Header("플레이어 말")]
    [SerializeField] private GameObject playerPiecePrefab;
    [SerializeField] private RectTransform pieceRoot;
    [Header("플레이어 케릭터 리스트 오브젝트")]
    [SerializeField] private List<PlayerPoolOS> playerPoolOs = new List<PlayerPoolOS>();
    [Header("임시 적 리스트 오브젝트")]
    [SerializeField] private List<EnemyPoolOS> enemyPools = new List<EnemyPoolOS>();


    private List<List<RoguelikeMapNode>> currentMap;
    private List<MapNode> nodeViews = new List<MapNode>();
    private RoguelikeMapNode currentNode;
    private RectTransform playerPiece;
   
    private string battleSceneName = "TestBattle";
    private string shopSceneName = "TestShop";

    public bool CanMoveTo(RoguelikeMapNode targetNode)
    {
        if (currentNode == null) return false;

        if (targetNode == currentNode && !currentNode.IsCleared) return true;

        if (!currentNode.IsCleared) return false;

        if (targetNode.IsCleared) return false;

        return currentNode.NextNodes.Contains(targetNode);
    }

    private void CreateMap()
    {
        if (MapData.HasMap)
        {
            currentMap = MapData.SavedMap;
            currentNode = MapData.CurrentNode;
        }
        else
        {
            currentMap = generator.GenerateMap();
            currentNode = currentMap[0][0];
            currentNode.IsCleared = true;

            MapData.SaveMap(currentMap);
            MapData.SaveCurrentNode(currentNode);
        }

        DrawLines(currentMap);
        DrawNodes(currentMap);
        SpawnPlayerPiece(currentNode);
        RefreshAllNodes();
    }

    private void SpawnPlayerPiece(RoguelikeMapNode startNode)
    {
        GameObject obj = Instantiate(playerPiecePrefab, pieceRoot);
        playerPiece = obj.GetComponent<RectTransform>();

        playerPiece.anchoredPosition = startNode.WorldPos;
    }

    private void DrawNodes(List<List<RoguelikeMapNode>> map)
    {
        foreach (List<RoguelikeMapNode> column in map)
        {
            foreach (RoguelikeMapNode node in column)
            {
                MapNode view = Instantiate(nodePrefab, mapRoot);
                view.GetComponent<RectTransform>().anchoredPosition = node.WorldPos;
                view.Init(node, this);
                nodeViews.Add(view);
            }
        }
    }

    private void DrawLines(List<List<RoguelikeMapNode>> map)
    {
        foreach (List<RoguelikeMapNode> column in map)
        {
            foreach (RoguelikeMapNode node in column)
            {
                foreach (RoguelikeMapNode next in node.NextNodes)
                {
                    MapLine line = Instantiate(linePrefab, lineRoot);
                    line.Init(node.WorldPos, next.WorldPos);
                }
            }
        }
    }

    public void SelectNode(RoguelikeMapNode node)
    {
        if (!CanMoveTo(node)) return;

        currentNode = node;

        MapData.SaveCurrentNode(currentNode);

        MovePlayerPiece(node);

        RefreshAllNodes();

        EnterRoom(node);
    }

    public Sprite FindEnemySpriteFromRoomType(RoguelikeMapNode node)
    {
        if (node != null)
        {
            foreach( EnemyPoolOS enemyPool in enemyPools)
            {
                if (node.RoomType == enemyPool.RoomType)
                {
                    List<GameObject> enemyOBJs = enemyPool.GetEnemiePool();
                    foreach (GameObject enemyOBJ in enemyOBJs)
                    {
                        if (enemyOBJ == null) continue;
                        Sprite foundSprite = enemyOBJ.GetComponentInChildren<EnemyStatus>()?.GetPortrait;
                        if (foundSprite != null) return foundSprite;
                    }
                    
                }
            }
        }
        return null;
    }

    private void RefreshAllNodes()
    {
        foreach (MapNode view in nodeViews)
        {
            view.Refresh();
        }
    }

    private void MovePlayerPiece(RoguelikeMapNode node)
    {
        if (playerPiece == null) return;

        playerPiece.anchoredPosition = node.WorldPos;
    }

    private void EnterRoom(RoguelikeMapNode node)
    {
        List<GameObject> enemies;
        EnemyPoolOS pool;
        switch (node.RoomType)
        {
            case RoguelikeRoomType.Start:
                Debug.Log("시작 지점");
                break;

            case RoguelikeRoomType.Event:
                Debug.Log("이벤트방 입장");
                testNodeSetting(node);
                break;

            case RoguelikeRoomType.Shop:
                Debug.Log("상점 입장");
                BattleSceneData.SetEnemies(null, node.RoomType);

                SceneManager.LoadScene(shopSceneName);
                break;

            case RoguelikeRoomType.Battle_1:
            case RoguelikeRoomType.Battle_2:
            case RoguelikeRoomType.Battle_3:
            case RoguelikeRoomType.Boss:
                Debug.Log($"{node.RoomType} 입장");

                MapData.SaveCurrentNode(node);
                BattleSceneData.SaveBeforeBattle(BattleSceneData.CharactersData);

                pool = GetEnemyPool(node.RoomType);

                if (pool == null)
                {
                    Debug.LogError($"{node.RoomType}에 맞는 EnemyPool이 없음");
                    return;
                }

                enemies = pool.GetEnemiePool();
                BattleSceneData.SetEnemies(enemies, node.RoomType);
                SceneManager.LoadScene(battleSceneName);
                break;
        }
    }

    private EnemyPoolOS GetEnemyPool(RoguelikeRoomType roomType)
    {
        foreach (EnemyPoolOS pool in enemyPools)
        {
            if (pool != null && pool.RoomType == roomType) return pool;
        }

        return null;
    }

    private void testNodeSetting(RoguelikeMapNode node)
    {
        node.IsCleared = true;

        RefreshAllNodes();
    }

    private void Start()
    {
        if (BGMManager.Instance != null) BGMManager.Instance.RequestChangeBGM(true);
        
        if(BGMManager.Instance != null && BattleSceneData.isFirstPlayers == true)
        {
            BattleSceneData.SetPlayerCharacter(playerPoolOs[0].GetGetPlayerCharacterList());
            BattleSceneData.isFirstPlayers = false;
        }
        CreateMap();
    }
}