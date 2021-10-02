using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    ItemSlot[] itemSlots;
    void Awake()
    {
        itemSlots = GetComponentsInChildren<ItemSlot>();
    }

    public void SetInventorySlots(Inventory inventory)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < inventory.List.Count)
            {
                itemSlots[i].SetData(inventory.List[i].Name);
            }
            else
            {
                itemSlots[i].SetData("");
            }
        }
    }


    public void UpdateInventorySelection(int selectedItem)
    {
        for (int i=0; i< itemSlots.Length; i++)
        {
            itemSlots[i].SetSelection(selectedItem == i);
        }
    }
}
