using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemBase : ScriptableObject
{
    // インスペクターで設定用
    [SerializeField] new string name;
    [SerializeField] ItemType type;
    [SerializeField] int amount;

    // 公開用
    public string Name { get => name; }
    public ItemType Type { get => type; }
    public int Amount { get => amount; }
}

public enum ItemType
{
    HPHeal,
}
