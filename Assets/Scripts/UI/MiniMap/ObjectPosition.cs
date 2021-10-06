using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPosition : MonoBehaviour
{
    //  TODO:座標から, マップのどこにいるか取得する
    // Vector2 grid = new Vector2();
    public Rect2D range = new Rect2D(0, 0, 0, 0);
    public Vector2Int RoomGrind;
    public Vector2 nextMovePosition;

    public Vector2Int Grid
    {
        get =>  new Vector2Int(GetGridX(transform.position.x), GetGridY(transform.position.y));
    }

    public void SetPosition(int i, int j)
    {
        RoomGrind = new Vector2Int(i, j);
    }

    public void SetRange(int left, int top, int width, int height)
    {
        range.left = left;
        range.top = top;
        range.right = left + width - 1;
        range.bottom = top + height - 1;
    }

    public int GetGridX(float x)
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        var spr = GetComponent<SpriteRenderer>();
        var sprW = spr.bounds.size.x;
        return Mathf.RoundToInt((-min.x + x - sprW / 2) / sprW);
    }

    public int GetGridY(float y)
    {
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        var spr = GetComponent<SpriteRenderer>();
        var sprH = spr.bounds.size.y;
        return Mathf.RoundToInt(-(-max.y + y + sprH / 2) / sprH);
    }


    public bool IsOverlapPoint(ObjectPosition target)
    {
        if (Vector2.Distance(nextMovePosition, target.nextMovePosition) < Mathf.Epsilon)
        {
            return true;
        }
        return false;
    }

    public ObjectPosition IsOverlapPointNextMove()
    {
        return DungeonGenerator.instance.IsOverlap(this);
    }

    public bool IsWall(Vector2 point)
    {
        return DungeonGenerator.instance.IsWall(point.x, point.y);
    }
}
[System.Serializable]
public class Rect2D
{
    public int left;
    public int top;
    public int right;
    public int bottom;
    public int width { get { return right - left + 1; } }
    public int height { get { return bottom - top + 1; } }

    public Rect2D(int l, int t, int r, int b)
    {
        left = l;
        top = t;
        right = r;
        bottom = b;
    }
}