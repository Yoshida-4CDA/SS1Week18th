using UnityEngine;
using System.Collections;

/// 2次元レイヤー
public class DungeonMapData2D
{

    int _width;
    int _height;
    int _outOfRange = -1; // 領域外を指定した時の値
    int[,] _values = null; // マップデータ

    public int Width { get => _width; }
    public int Height { get => _height; }

    public DungeonMapData2D(int width = 0, int height = 0)
    {
        if (width > 0 && height > 0)
        {
            Create(width, height);
        }
    }

    public void Create(int width, int height)
    {
        _width = width;
        _height = height;
        _values = new int[Width, Height];
    }


    public bool IsOutOfRange(int x, int y)
    {
        if (x < 0 || x >= Width) { return true; }
        if (y < 0 || y >= Height) { return true; }

        return false;
    }

    /// <returns>指定の座標の値（領域外を指定したら_outOfRangeを返す）</returns>
    public int Get(int x, int y)
    {
        if (IsOutOfRange(x, y))
        {
            return _outOfRange;
        }

        return _values[x, y];
    }

    public void Set(int x, int y, int v)
    {
        if (IsOutOfRange(x, y))
        {
            // 領域外を指定した
            return;
        }

        _values[x, y] = v;
    }

    // すべてのセルを特定の値で埋める
    public void Fill(int val)
    {
        for (int j = 0; j < Height; j++)
        {
            for (int i = 0; i < Width; i++)
            {
                Set(i, j, val);
            }
        }
    }

    /// 矩形領域を指定の値で埋める((x,y)からw,h幅)
    public void FillRect(int x, int y, int w, int h, int val)
    {
        for (int j = 0; j < h; j++)
        {
            for (int i = 0; i < w; i++)
            {
                int px = x + i;
                int py = y + j;
                Set(px, py, val);
            }
        }
    }

    // 矩形領域を指定の値で埋める（4点指定)
    public void FillRectLTRB(int left, int top, int right, int bottom, int val)
    {
        FillRect(left, top, right - left, bottom - top, val);
    }

    // デバッグ出力
    public void Dump()
    {
        Debug.Log("[Layer2D] (w,h)=(" + Width + "," + Height + ")");
        for (int y = 0; y < Height; y++)
        {
            string s = "";
            for (int x = 0; x < Width; x++)
            {
                s += Get(x, y) + ",";
            }
            Debug.Log(s);
        }
    }
}
