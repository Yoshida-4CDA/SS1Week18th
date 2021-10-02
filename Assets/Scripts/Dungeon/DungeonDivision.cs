using UnityEngine;
using System.Collections;

// ダンジョン区画情報
public class DungeonDivision
{
    public DungeonRect Outer; // 外周の矩形情報 TODO:これ何?
    public DungeonRect Room;  // 区画内に作ったルーム情報TODO:これ何?
    public DungeonRect Road;  // 通路情報

    public DungeonDivision()
    {
        Outer = new DungeonRect(); // 大きさ0の矩形?
        Room = new DungeonRect();
        Road = null; // 通路はなし
    }

    // 通路が存在するかどうか
    public bool HasRoad()
    {
        return Road != null;
    }

    // 通路を作成する:(太さ1の矩形を作成)
    public void CreateRoad(int left, int top, int right, int bottom)
    {
        Road = new DungeonRect(left, top, right, bottom);
    }

    // デバッグ出力
    public void Dump()
    {
        Outer.Dump();
        Room.Dump();
        if (Road != null)
        {
            Road.Dump();
        }
    }
}

