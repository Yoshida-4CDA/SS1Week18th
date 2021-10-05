using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public const int MAX = 3;
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
    [SerializeField] ItemBase _base;
    public string Name { get => _base.Name; }

    public void Use(Player player)
    {
        switch (_base.Type)
        {
            case ItemType.HPHeal:
                player.Heal(_base.Amount);
                break;
        }
    }
}
