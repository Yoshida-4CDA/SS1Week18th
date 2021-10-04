using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    enum GameState
    {
        Idle,
        OpenInventory,
        StatusUPSelection,
    }

    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] MessageUI messageUI;
    [SerializeField] StatusUPSelection statusUPSelection;

    [SerializeField] Inventory inventory;

    GameState state;

    int currentItemSlot;
    int currentStatusUPIndex;

    void Start()
    {
        state = GameState.Idle;
        statusUPSelection.gameObject.SetActive(false);
        inventoryUI.gameObject.SetActive(false);
    }

    // ゲームの状態に応じて、入力処理をかえる
    // Idel:通常時
    // OpenInventory:インベントリを開いているとき
    // StatusUPSelection:ステータスUPのUIが出てるとき
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
            case GameState.StatusUPSelection:
                HandleStatusUPSelection();
                break;
        }

    }

    // Idle時の処理
    void HandleUpdateIdle()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenInventory();
        }
        // TODOテスト用
        if (Input.GetKeyDown(KeyCode.O))
        {
            OpenStatusSelectionUI();
        }
    }

    // インベントリを開いてる時の処理
    void HandleUpdateInventory()
    {
        // 方向キーでアイテム選択
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentItemSlot--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentItemSlot++;
        }
        currentItemSlot = Mathf.Clamp(currentItemSlot, 0, inventory.List.Count - 1);

        // 選んだものに色をつける
        inventoryUI.UpdateInventorySelection(currentItemSlot);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 決定
            messageUI.SetMessage($"羊は{inventory.List[currentItemSlot].Name}を使った!");
            inventory.Use(currentItemSlot);
            CloseInventory();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            // キャンセル
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

    // ステータスUPパネルの操作:インベントリと似たコード
    void HandleStatusUPSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentStatusUPIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentStatusUPIndex--;
        }
        currentStatusUPIndex = Mathf.Clamp(currentStatusUPIndex, 0, inventory.List.Count - 1);

        statusUPSelection.UpdateCardSelection(currentStatusUPIndex);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StatusUPCard selectedCard = statusUPSelection.StatusUPCards[currentStatusUPIndex];
            // messageUI.SetMessage($"{inventory.List[currentItemSlot].Name}を使った!");
            // inventory.Use(currentItemSlot);
            CloseStatusSelectionUI();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            CloseStatusSelectionUI();
        }
    }

    void OpenStatusSelectionUI()
    {
        state = GameState.StatusUPSelection;
        currentStatusUPIndex = 0;
        // UI表示
        statusUPSelection.gameObject.SetActive(true);
        // inventoryUI.SetInventorySlots(inventory);
    }
    void CloseStatusSelectionUI()
    {
        state = GameState.Idle;
        statusUPSelection.gameObject.SetActive(false);
    }

}
