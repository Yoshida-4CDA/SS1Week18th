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
        CheckLevelUP,
        StatusUPSelection,
        GameOver,
        End,
    }

    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] MessageUI messageUI;
    [SerializeField] StatusUPSelection statusUPSelection;
    [SerializeField] DungeonGenerator dungeonGenerator;
    [SerializeField] PlayerStatusUI playerStatusUI;
    [SerializeField] GoalMessage goalMessageUI;

    Inventory inventory;

    [SerializeField] GameState state;

    int currentItemSlot;
    int currentStatusUPIndex;
    int currentResult;

    Player player;
    List<Enemy> enemies;

    void Start()
    {
        inventory = GameData.instance.GetComponent<Inventory>();
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
        player.OnItem += OnItem;


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
            case GameState.CheckLevelUP:
                HandleUpdateCheckLevelUP();
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
            case GameState.GameOver:
                HandleUpdateGameOver();
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
        Debug.Log("PlayerAttack");
    }
    void HandleUpdatePlayerMove()
    {
        Debug.Log("PlayerMove");
    }

    void HandleUpdateCheckLevelUP()
    {
        if (state != GameState.Busy)
        {
            if (player.Status.IsLevelUP)
            {
                state = GameState.StatusUPSelection;
                player.LevelUP();
                playerStatusUI.SetData(player.Status);
                OpenStatusSelectionUI();
            }
            else
            {
                state = GameState.EnemyTurn;
            }
        }
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

    void HandleUpdateGameOver()
    {
        // 方向キーでアイテム選択
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentResult++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentResult--;
        }
        currentResult = Mathf.Clamp(currentResult, 0, 1);
        goalMessageUI.HandleUpdateSelection(currentResult);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // TODO:ランキング中に処理されそう
            if (currentResult == 0)
            {
                ResetStart();
            }
            else if(currentResult == 1)
            {
                GoToTitle();
            }
        }
    }

    void PlayerEnd()
    {
        // Debug.Log("PlayerEnd");
        // GameState.BusyだったらBusyのまま, そうじゃないなら敵のターン
        switch (state)
        {
            case GameState.Busy:
            case GameState.GameOver:
                break;
            default:
                state = GameState.CheckLevelUP;
                break;
        }
    }

    void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        player.AddExp(enemy.Exp);
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
        while (ChechEnemyTurnEnd())
        {
            yield return null;
        }
        state = GameState.End;
    }

    bool ChechEnemyTurnEnd()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy.isMoving)
            {
                return true;
            }
        }
        if (player.IsMoving)
        {
            return true;
        }
        return false;
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

        if (Input.GetKeyDown(KeyCode.Space) && inventory.List.Count > 0)
        {
            // 決定
            Item selectedItem = inventory.List[currentItemSlot];
            messageUI.SetMessage($"羊は{selectedItem.Name}を使った!");
            selectedItem.Use(player);
            inventory.List.Remove(selectedItem);
            playerStatusUI.SetData(player.Status);
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
        currentStatusUPIndex = Mathf.Clamp(currentStatusUPIndex, 0, statusUPSelection.StatusUPCards.Length - 1);
        Debug.Log("HandleStatusUPSelection" + currentStatusUPIndex);
        statusUPSelection.UpdateCardSelection(currentStatusUPIndex);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StatusUPCard selectedCard = statusUPSelection.StatusUPCards[currentStatusUPIndex];
            selectedCard.UseCard(player);
            CloseStatusSelectionUI();
        }
    }

    void OpenStatusSelectionUI()
    {
        Debug.Log("OpenStatusSelectionUI");
        state = GameState.StatusUPSelection;
        currentStatusUPIndex = 0;
        // UI表示
        statusUPSelection.gameObject.SetActive(true);
        statusUPSelection.UpdateCardSelection(currentStatusUPIndex);
    }
    void CloseStatusSelectionUI()
    {
        state = GameState.EnemyTurn;
        statusUPSelection.gameObject.SetActive(false);
    }

    void OnItem(ItemObj itemObj)
    {
        if (inventory.List.Count >= Inventory.MAX)
        {
            messageUI.SetMessage("持ち物がいっぱいだ");
        }
        else
        {
            inventory.List.Add(itemObj.Item);
            itemObj.gameObject.SetActive(false);
        }
    }
    void GameOver()
    {
        // enabled = false;
        state = GameState.GameOver;
        StartCoroutine(DelayGameOver());
    }
    IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(1f);
        goalMessageUI.ShowResult(player.Status.currentStage);
    }

    void Restart()
    {
        state = GameState.Busy;
        StartCoroutine(DelayRestart());
    }

    void ResetStart()
    {
        Destroy(GameData.instance.gameObject);
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    void GoToTitle()
    {
        Destroy(GameData.instance.gameObject);
        SceneManager.LoadScene("Title");
    }

    IEnumerator DelayRestart()
    {
        yield return new WaitForSeconds(1f);
        goalMessageUI.SetSleepTime(player.Status.currentStage);
        yield return new WaitForSeconds(2f);
        string currentScene = SceneManager.GetActiveScene().name;
        player.Status.currentStage++;
        SceneManager.LoadScene(currentScene);
    }
}
