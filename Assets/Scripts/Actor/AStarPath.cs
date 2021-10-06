using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStarPath
{
    DungeonMapData2D map;                     // 移動範囲
    private List<Node> nodeInfoList;    // 調査セルを記憶しておくリスト
    private bool exitFlg;

    public Vector2Int AstarSearchPathFinding(DungeonMapData2D map, Vector2Int start, Vector2Int end)
    {
        this.map = map;
        nodeInfoList = new List<Node>();
        Vector2Int lastPath = start;
        // ゴールはプレイヤーの位置情報
        // goal = player.transform.position;

        // スタートの情報を設定する(スタートは敵)
        Node startNode = new Node();
        startNode.Grid = start;// enemy.transform.position; // 開始地点 
        startNode.Cost = 0;
        startNode.Heuristic = Vector2.Distance(start, end);
        startNode.SumCost = startNode.Cost + startNode.Heuristic;
        startNode.ParentPosition = new Vector2Int(-9999, -9999);    // スタート時の親の位置はありえない値にしておきます
        startNode.IsOpen = true;
        nodeInfoList.Add(startNode);

        exitFlg = false;

        // オープンが存在する限りループ
        while (nodeInfoList.Where(x => x.IsOpen == true).Select(x => x).Count() > 0 && exitFlg == false)
        {
            // 最小コストのノードを探す
            Node minNode = nodeInfoList.Where(x => x.IsOpen == true).OrderBy(x => x.SumCost).Select(x => x).First();
            OpenSurround(minNode, ref lastPath, end);

            // 中心のノードを閉じる
            minNode.IsOpen = false;
        }

        // Debug.Log(string.Join("\n", path));
        return lastPath;
    }

    void OpenSurround(Node center, ref Vector2Int path, Vector2Int goal)
    {
        // ポジションをVector3Intへ変換
        Vector2Int centerPos = center.Grid;

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // 上下左右のみ可とする、かつ、中心は除外
                if (((i != 0 && j == 0) || (i == 0 && j != 0)) && !(i == 0 && j == 0))
                {
                    Vector2Int posInt = new Vector2Int(centerPos.x + i, centerPos.y + j);
                    if (!map.IsOutOfRange(posInt.x, posInt.y) && map.Get(posInt.x, posInt.y)!=1 && !(i == 0 && j == 0))
                    {
                        // リストに存在しないか探す
                        Vector2Int pos = posInt;
                        if (nodeInfoList.Where(x => x.Grid == pos).Select(x => x).Count() == 0)
                        {
                            // リストに追加
                            Node node = new Node();
                            node.Grid = pos;
                            node.Cost = center.Cost + 1;
                            node.Heuristic = Vector2.Distance(pos, goal);
                            node.SumCost = node.Cost + node.Heuristic;
                            node.ParentPosition = center.Grid;
                            node.IsOpen = true;
                            nodeInfoList.Add(node);

                            // ゴールの位置と一致したら終了
                            if (goal == pos)
                            {
                                Node preNode = node;
                                while (preNode.ParentPosition != new Vector2Int(-9999, -9999))
                                {
                                    path = preNode.Grid;
                                    // map.SetTile(map.WorldToCell(preCell.pos), replaceTile);
                                    preNode = nodeInfoList.Where(x => x.Grid == preNode.ParentPosition).Select(x => x).First();
                                }

                                exitFlg = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
