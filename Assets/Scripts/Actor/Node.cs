using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int Grid { get; set; }
    public float Cost { get; set; }
    public float Heuristic { get; set; }
    public float SumCost { get; set; }
    public Vector2Int ParentPosition { get; set; }
    public bool IsOpen { get; set; }

}
