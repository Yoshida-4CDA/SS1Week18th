using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<Item> list;

    public List<Item> List
    {
        get => list;
    }

    public void Use(int itemIndex)
    {
        list.RemoveAt(itemIndex);
    }
}

[System.Serializable]
public class Item
{
    public string Name;
}
