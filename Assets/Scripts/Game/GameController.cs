using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    enum GameState
    {
        Idle,
        OpenInventory,
        UseItem,
        PlayerAttack,
        PlayerMove,
        EnemyTurn,
        Busy,
        StatusUPSelection,
        End,
    }

    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] MessageUI messageUI;
    [SerializeField] StatusUPSelection statusUPSelection;
    [SerializeField] DungeonGenerator dungeonGenerator;
    [SerializeField] PlayerStatusUI playerStatusUI;

    [SerializeField] Inventory inventory;

    GameState state;

    int currentItemSlot;
    int currentStatusUPIndex;

    Player player;
    List<Enemy> enemies;

    void Start()
    {
        dungeonGenerator.Init();
        player = dungeonGenerator.Player.GetComponent<Player>();
        player.Init();
        
        enemies = new List<Enemy>();
        foreach (ObjectPosition enemyObj in dungeonGenerator.Enemys)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.OnDestroyEnemy += RemoveEnemy;
            enemies.Add(enemy);
        }

        player.OnPlayerTurnEnd += PlayerEnd;
        player.OnGameOver += GameOver;
        player.OnGoal += Restart;

        state = GameState.Idle;
        statusUPSelection.gameObject.SetActive(false);
        inventoryUI.gameObject.SetActive(false);
        playerStatusUI.SetData(player.Status);
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
            case GameState.PlayerAttack:
                HandleUpdatePlayerAttack();
                break;
            case GameState.PlayerMove:
                HandleUpdatePlayerMove();
                break;
            case GameState.EnemyTurn:
                HandleUpdateEnemyTurn();
                break;
            case GameState.UseItem:
                HandleUpdateUseItem();
                break;
            case GameState.End:
                HandleUpdateEnd();
                break;
        }
    }

    // Idle時の処理
    void HandleUpdateIdle()
    {

        player.HandleUpdate();

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

    void HandleUpdatePlayerAttack()
    {
    }
    void HandleUpdatePlayerMove()
    {
    }
    void HandleUpdateEnemyTurn()
    {
        state = GameState.Busy;
        StartCoroutine(MoveEnemies());
    }
    void HandleUpdateUseItem()
    {
    }
    void HandleUpdateEnd()
    {
        state = GameState.Idle;
    }

    void PlayerEnd()
    {
        // GameState.BusyだったらBusyのまま, そうじゃないなら敵のターン
        if (state != GameState.Busy)
        {
            state = GameState.EnemyTurn;
        }
    }

    void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        player.AddExp(enemy.enemyExp);
    }

    // TODO:Playerと重なるバグ修正
    IEnumerator MoveEnemies()   
    {
        // yield return new WaitForSeconds(0.1f);

        if (enemies.Count == 0)
        {
            // yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            bool attacked = enemies[i].MoveEnemy();
            if (attacked)
            {
                yield return new WaitForSeconds(0.1f);
            }
            playerStatusUI.SetData(player.Status);
        }
        state = GameState.End;
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

    void GameOver()
    {
        Debug.Log("ゲームオーバー");
        // enabled = false;
        state = GameState.Busy;
    }
    void Restart()
    {
        state = GameState.Busy;
        StartCoroutine(DelayRestart());
    }

    IEnumerator DelayRestart()
    {
        yield return new WaitForSeconds(1f);
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
}
