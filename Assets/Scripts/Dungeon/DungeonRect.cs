using UnityEngine;
using System.Collections;

// 矩形管理
public class DungeonRect
{
    public int Left = 0; // 左
    public int Top = 0; // 上
    public int Right = 0; // 右
    public int Bottom = 0; // 下

    public DungeonRect(int left = 0, int top = 0, int right = 0, int bottom = 0)
    {
        Set(left, top, right, bottom);
    }

    public void Set(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
    public int Width { get => Right - Left; }
    public int Height { get => Bottom - Top; }

    // デバッグ出力
    public void Dump()
    {
        Debug.Log(string.Format("<Rect l,t,r,b = {0},{1},{2},{3}> w,h = {4},{5}", Left, Top, Right, Bottom, Width, Height));
    }
}