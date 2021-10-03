using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

// ダンジョンの自動生成モジュール
public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] DungeonPrefabs dungeonPrefabs;
    [SerializeField] AutoMapping autoMapping;
    [SerializeField] Transform rooms;

    const int WIDTH = 36;
    const int HEIGHT = 20;

    const int OUTER_MERGIN = 3;  // 区画と部屋の余白サイズ
    const int POS_MERGIN = 2;    // 部屋配置の余白サイズ

    const int MIN_ROOM = 3;     // 最小の部屋サイズ    
    const int MAX_ROOM = 10;    // 最大の部屋サイズ

    const int CHIP_ROAD = 0;    // 通路
    const int CHIP_WALL = 1;    // 壁

    DungeonMapData2D mapData2D = null;      // 2次元配列情報

    List<DungeonDivision> divisionList = null;   // 区画リスト

    public DungeonMapData2D MapData2D { get => mapData2D; }

    /// チップ上のX座標を取得する:整数値のデータを配置する座標に変換する
    float GetChipX(int i)
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        var spr = dungeonPrefabs.Wall.GetComponent<SpriteRenderer>();
        var sprW = spr.bounds.size.x;

        return min.x + (sprW * i) + sprW / 2;
    }

    float GetChipY(int j)
    {
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        var spr = dungeonPrefabs.Wall.GetComponent<SpriteRenderer>();
        var sprH = spr.bounds.size.y;

        return max.y - (sprH * j) - sprH / 2;
    }

    public int GetGridX(float x)
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        var spr = dungeonPrefabs.Wall.GetComponent<SpriteRenderer>();
        var sprW = spr.bounds.size.x;
        return Mathf.RoundToInt((-min.x + x - sprW / 2) / sprW);
    }

    public int GetGridY(float y)
    {
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        var spr = dungeonPrefabs.Wall.GetComponent<SpriteRenderer>();
        var sprH = spr.bounds.size.y;
        return Mathf.RoundToInt(-(-max.y + y + sprH / 2) / sprH);
    }


    void Start()
    {
        // ■1. 初期化
        // 2次元配列初期化
        mapData2D = new DungeonMapData2D(WIDTH, HEIGHT);

        // 区画リスト作成
        divisionList = new List<DungeonDivision>();

        // ■2. すべてを壁にする
        mapData2D.Fill(CHIP_WALL);

        // ■3. 最初の区画を作る
        CreateDivision(0, 0, WIDTH - 1, HEIGHT - 1);

        // ■4. 区画を分割する
        // 垂直 or 水平分割フラグの決定
        bool bVertical = (Random.Range(0, 2) == 0);
        SplitDivison(bVertical);


        // ■5. 区画に部屋を作る
        CreateRoom();

        // ■6. 部屋同士をつなぐ
        ConnectRooms();
        // Dump();
        InstantiateDungeon();
    }



    // デバッグ出力
    void Dump()
    {
        foreach (var div in divisionList)
        {
            div.Dump();
        }
        mapData2D.Dump();
    }

    void InstantiateDungeon()
    {
        autoMapping.Reset(mapData2D.Width, mapData2D.Height);

        // タイルを配置
        for (int j = 0; j < mapData2D.Height; j++)
        {
            for (int i = 0; i < mapData2D.Width; i++)
            {
                if (mapData2D.Get(i, j) == CHIP_WALL)
                {
                    // 壁生成
                    float x = GetChipX(i);
                    float y = GetChipY(j);
                    GameObject wallObj = Instantiate(dungeonPrefabs.Wall, new Vector3(x, y), Quaternion.identity);
                }
                else if (mapData2D.Get(i, j) == CHIP_ROAD)
                {
                    float x = GetChipX(i);
                    float y = GetChipY(j);
                    GameObject roadObj = Instantiate(dungeonPrefabs.Road, new Vector3(x, y), Quaternion.identity);
                    // roadObj.GetComponent<ObjectPosition>().SetRange(0, 0, 1, 1);// TODO:部屋の情報
                    // ObjectPosition objectPosition = Instantiate(dungeonPrefabs.Room, rooms);
                    // roadObj.GetComponent<ObjectPosition>().SetPosition(i, j);
                }
                // autoMapping.Mapping(i, j, mapData2D.Get(i, j) + 1);
            }
        }
    }

    // 最初の区画を作る
    void CreateDivision(int left, int top, int right, int bottom)
    {
        DungeonDivision div = new DungeonDivision();
        div.Outer.Set(left, top, right, bottom); // 外周を設定?
        divisionList.Add(div); // 作った矩形波リストに追加
    }

    // 区画を分割する
    // bVertical:垂直分割するかどうか
    void SplitDivison(bool bVertical)
    {
        // 末尾の要素を取り出し
        DungeonDivision parent = divisionList[divisionList.Count - 1];
        divisionList.Remove(parent);

        // 子となる区画を生成
        DungeonDivision child = new DungeonDivision();

        if (bVertical)
        {
            // ▼縦方向に分割する
            if (CheckDivisionSize(parent.Outer.Height) == false)
            {
                // 縦の高さが足りない
                // 親区画を戻しておしまい
                divisionList.Add(parent);
                return;
            }

            // 分割ポイントを求める
            int a = parent.Outer.Top + (MIN_ROOM + OUTER_MERGIN);
            int b = parent.Outer.Bottom - (MIN_ROOM + OUTER_MERGIN);
            // AB間の距離を求める
            int ab = b - a;
            // 最大の部屋サイズを超えないようにする
            ab = Mathf.Min(ab, MAX_ROOM);

            // 分割点を決める
            int p = a + Random.Range(0, ab + 1);

            // 子区画に情報を設定
            child.Outer.Set(
                parent.Outer.Left, p, parent.Outer.Right, parent.Outer.Bottom);

            // 親の下側をp地点まで縮める
            parent.Outer.Bottom = child.Outer.Top;
        }
        else
        {
            // ▼横方向に分割する
            if (CheckDivisionSize(parent.Outer.Width) == false)
            {
                // 横幅が足りない
                // 親区画を戻しておしまい
                divisionList.Add(parent);
                return;
            }

            // 分割ポイントを求める
            int a = parent.Outer.Left + (MIN_ROOM + OUTER_MERGIN);
            int b = parent.Outer.Right - (MIN_ROOM + OUTER_MERGIN);
            // AB間の距離を求める
            int ab = b - a;
            // 最大の部屋サイズを超えないようにする
            ab = Mathf.Min(ab, MAX_ROOM);

            // 分割点を求める
            int p = a + Random.Range(0, ab + 1);

            // 子区画に情報を設定
            child.Outer.Set(
                p, parent.Outer.Top, parent.Outer.Right, parent.Outer.Bottom);

            // 親の右側をp地点まで縮める
            parent.Outer.Right = child.Outer.Left;
        }

        // 次に分割する区画をランダムで決める
        if (Random.Range(0, 2) == 0)
        {
            // 子を分割する
            divisionList.Add(parent);
            divisionList.Add(child);
        }
        else
        {
            // 親を分割する
            divisionList.Add(child);
            divisionList.Add(parent);
        }

        // 分割処理を再帰呼び出し (分割方向は縦横交互にする)
        SplitDivison(!bVertical);
    }

    // 指定のサイズを持つ区画を分割できるかどうか
    bool CheckDivisionSize(int size)
    {
        // (最小の部屋サイズ + 余白)
        // 2分割なので x2 する
        // +1 して連絡通路用のサイズも残す
        int min = (MIN_ROOM + OUTER_MERGIN) * 2 + 1;

        return size >= min;
    }

    // 区画に部屋を作る
    void CreateRoom()
    {
        foreach (DungeonDivision div in divisionList)
        {
            // 基準サイズを決める
            int dw = div.Outer.Width - OUTER_MERGIN;
            int dh = div.Outer.Height - OUTER_MERGIN;

            // 大きさをランダムに決める
            int sw = Random.Range(MIN_ROOM, dw);
            int sh = Random.Range(MIN_ROOM, dh);

            // 最大サイズを超えないようにする
            sw = Mathf.Min(sw, MAX_ROOM);
            sh = Mathf.Min(sh, MAX_ROOM);

            // 空きサイズを計算 (区画 - 部屋)
            int rw = (dw - sw);
            int rh = (dh - sh);

            // 部屋の左上位置を決める
            int rx = Random.Range(0, rw) + POS_MERGIN;
            int ry = Random.Range(0, rh) + POS_MERGIN;

            int left = div.Outer.Left + rx;
            int right = left + sw;
            int top = div.Outer.Top + ry;
            int bottom = top + sh;

            // 部屋のサイズを設定
            div.Room.Set(left, top, right, bottom);
            float x = GetChipX(left);
            float y = GetChipY(top);

            ObjectPosition room = Instantiate(dungeonPrefabs.Room, new Vector3(x, y), Quaternion.identity, rooms);
            room.SetRange(left, top, sw, sh);
            room.SetPosition(left, top);

            // 部屋を通路にする
            FillDgRect(div.Room);
        }
    }

	// DgRectの範囲を塗りつぶす
    void FillDgRect(DungeonRect r)
    {
        mapData2D.FillRectLTRB(r.Left, r.Top, r.Right, r.Bottom, CHIP_ROAD);
    }

    // 部屋同士を通路でつなぐ
    void ConnectRooms()
    {
        for (int i = 0; i < divisionList.Count - 1; i++)
        {
            // リストの前後の区画は必ず接続できる
            DungeonDivision a = divisionList[i];
            DungeonDivision b = divisionList[i + 1];

            // 2つの部屋をつなぐ通路を作成
            CreateRoad(a, b);

            // 孫にも接続する
            for (int j = i + 2; j < divisionList.Count; j++)
            {
                DungeonDivision c = divisionList[j];
                if (CreateRoad(a, c, true))
                {
                    // 孫に接続できたらおしまい
                    break;
                }
            }
        }
    }

    // 指定した部屋の間を通路でつなぐ
	bool CreateRoad(DungeonDivision divA, DungeonDivision divB, bool bGrandChild = false)
    {
        if (divA.Outer.Bottom == divB.Outer.Top || divA.Outer.Top == divB.Outer.Bottom)
        {
            // 上下でつながっている
            // 部屋から伸ばす通路の開始位置を決める
            int x1 = Random.Range(divA.Room.Left, divA.Room.Right);
            int x2 = Random.Range(divB.Room.Left, divB.Room.Right);
            int y = 0;

            if (bGrandChild)
            {
                // すでに通路が存在していたらその情報を使用する
                if (divA.HasRoad()) { x1 = divA.Road.Left; }
                if (divB.HasRoad()) { x2 = divB.Road.Left; }
            }

            if (divA.Outer.Top > divB.Outer.Top)
            {
                // B - A (Bが上側)
                y = divA.Outer.Top;
                // 通路を作成
                divA.CreateRoad(x1, y + 1, x1 + 1, divA.Room.Top);
                divB.CreateRoad(x2, divB.Room.Bottom, x2 + 1, y);
            }
            else
            {
                // A - B (Aが上側)
                y = divB.Outer.Top;
                // 通路を作成
                divA.CreateRoad(x1, divA.Room.Bottom, x1 + 1, y);
                divB.CreateRoad(x2, y, x2 + 1, divB.Room.Top);
            }
            FillDgRect(divA.Road);
            FillDgRect(divB.Road);

            // 通路同士を接続する
            FillHLine(x1, x2, y);

            // 通路を作れた
            return true;
        }

        if (divA.Outer.Left == divB.Outer.Right || divA.Outer.Right == divB.Outer.Left)
        {
            // 左右でつながっている
            // 部屋から伸ばす通路の開始位置を決める
            int y1 = Random.Range(divA.Room.Top, divA.Room.Bottom);
            int y2 = Random.Range(divB.Room.Top, divB.Room.Bottom);
            int x = 0;

            if (bGrandChild)
            {
                // すでに通路が存在していたらその情報を使う
                if (divA.HasRoad()) { y1 = divA.Road.Top; }
                if (divB.HasRoad()) { y2 = divB.Road.Top; }
            }

            if (divA.Outer.Left > divB.Outer.Left)
            {
                // B - A (Bが左側)
                x = divA.Outer.Left;
                // 通路を作成
                divB.CreateRoad(divB.Room.Right, y2, x, y2 + 1);
                divA.CreateRoad(x + 1, y1, divA.Room.Left, y1 + 1);
            }
            else
            {
                // A - B (Aが左側)
                x = divB.Outer.Left;
                divA.CreateRoad(divA.Room.Right, y1, x, y1 + 1);
                divB.CreateRoad(x, y2, divB.Room.Left, y2 + 1);
            }
            FillDgRect(divA.Road);
            FillDgRect(divB.Road);

            // 通路同士を接続する
            FillVLine(y1, y2, x);

            // 通路を作れた
            return true;
        }


        // つなげなかった
        return false;
    }

    /// 水平方向に線を引く (左と右の位置は自動で反転する)
    void FillHLine(int left, int right, int y)
    {
        if (left > right)
        {
            // 左右の位置関係が逆なので値をスワップする
            int tmp = left;
            left = right;
            right = tmp;
        }
        mapData2D.FillRectLTRB(left, y, right + 1, y + 1, CHIP_ROAD);
    }

    /// 垂直方向に線を引く (上と下の位置は自動で反転する)
    void FillVLine(int top, int bottom, int x)
    {
        if (top > bottom)
        {
            // 上下の位置関係が逆なので値をスワップする
            int tmp = top;
            top = bottom;
            bottom = tmp;
        }
        mapData2D.FillRectLTRB(x, top, x + 1, bottom + 1, CHIP_ROAD);
    }
    public bool IsInRoomOf(ObjectPosition room, int xgrid, int zgrid)
    {
        return (room.RoomGrind.x <= xgrid && xgrid <= room.RoomGrind.x + room.range.width-1) &&
            (room.RoomGrind.y <= zgrid && zgrid <= room.RoomGrind.y + room.range.height-1);
        int sx = room.RoomGrind.x + room.range.left;
        int ex = sx + room.range.right;
        int sz = room.RoomGrind.y + room.range.top;
        int ez = sz + room.range.bottom;
        return xgrid >= sx && xgrid <= ex && zgrid >= sz && zgrid <= ez;
    }
    public ObjectPosition GetInRoom(int xgrid, int zgrid)
    {
        Debug.Log($"{xgrid}:{zgrid}");
        foreach (var room in rooms.GetComponentsInChildren<ObjectPosition>())
        {
            if (IsInRoomOf(room, xgrid, zgrid))
            {
                return room;
            }
        }
        return null;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(160, 160, 128, 32), "もう１回"))
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
    }
}

[System.Serializable]
public class DungeonPrefabs
{
    public GameObject Wall;
    public GameObject Road;
    public ObjectPosition Room;
}