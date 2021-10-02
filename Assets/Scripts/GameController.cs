using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    enum GameState
    {
        Idle,
        OpenInventory,
    }

    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] MessageUI messageUI;

    [SerializeField] Inventory inventory;

    GameState state;

    int currentItemSlot;

    void Start()
    {
        state = GameState.Idle;
    }

    void Update()
    {
        switch (state)
        {
            case GameState.Idle:
                HandleUpdateIdle();
                break;
            case GameState.OpenInventory:
                HandleUpdateInventory();
                break;
        }

    }

    void HandleUpdateIdle()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            OpenInventory();
        }
    }

    void HandleUpdateInventory()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentItemSlot--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentItemSlot++;
        }
        currentItemSlot = Mathf.Clamp(currentItemSlot, 0, inventory.List.Count - 1);

        inventoryUI.UpdateInventorySelection(currentItemSlot);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            messageUI.SetMessage($"羊は{inventory.List[currentItemSlot].Name}を使った!");
            inventory.Use(currentItemSlot);
            CloseInventory();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            CloseInventory();
        }
    }

    void OpenInventory()
    {
        state = GameState.OpenInventory;
        currentItemSlot = 0;
        inventoryUI.gameObject.SetActive(true);
        inventoryUI.SetInventorySlots(inventory);
    }
    void CloseInventory()
    {
        state = GameState.Idle;
        inventoryUI.gameObject.SetActive(false);
    }
}
