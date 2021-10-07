using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    enum GameState
    {
        Idle,
        PlayerAttack,
        PlayerTurn,
        OpenInventory,
        UseItem,
        EnemyTurn,
        Busy,
        CheckLevelUP,
        StatusUPSelection,
        GameOver,
        Goal,
        End,
    }

    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] MessageUI messageUI;
    [SerializeField] StatusUPSelection statusUPSelection;
    [SerializeField] DungeonGenerator dungeonGenerator;
    [SerializeField] PlayerStatusUI playerStatusUI;
    [SerializeField] GoalMessage goalMessageUI;
    [SerializeField] CameraManager cameraManager;

    Inventory inventory;

    [SerializeField] GameState state;

    int currentItemSlot;
    int currentStatusUPIndex;
    int currentResult;

    Player player;
    List<Enemy> enemies;

    [SerializeField] Fade fade;

    void Start()
    {
        messageUI.SetMessage($"ここはある人の夢の世界\n<color=#FFAC00>下に向かうほど長く眠れる</color>と言われている");
        SoundManager.instance.PlayBGM(SoundManager.BGM.Main);

        fade.FadeOut(1.5f);   // フェードアウト演出

        inventory = GameData.instance.GetComponent<Inventory>();
        dungeonGenerator.Init();
        player = dungeonGenerator.Player.GetComponent<Player>();
        player.Init();
        cameraManager.SetTarget(player.transform);

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
        player.OnPlayerAttack += OnPlayerAttack;


        state = GameState.Idle;
        statusUPSelection.gameObject.SetActive(false);
        inventoryUI.gameObject.SetActive(false);
        playerStatusUI.SetData(player.Status);
    }

    // ゲームの状態に応じて、入力処理をかえる
    // switch文をif文に変えたことで、1フレームの間に処理できることが増えた=>敵とPlayerが一緒に動く
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // 左シフト押してると早く動ける:バグるかも
            Time.timeScale = 4f;
        }
        else
        {
            Time.timeScale = 1;
        }
        if (state == GameState.GameOver)
        {
            HandleUpdateGameOver();
            return;
        }
        // 入力待ち
        if (state == GameState.Idle)
        {
            HandleUpdateIdle();
        }

        // 入力後はPlayerの処理
        if (state == GameState.PlayerTurn)
        {
            HandlePlayerTurn();
        }
        else if(state == GameState.OpenInventory)
        {
            HandleUpdateInventory();
        }

        if (state == GameState.UseItem)
        {
            HandleUpdateUseItem();
        }

        // レベルアップのチェック
        if (state == GameState.CheckLevelUP)
        {
            HandleUpdateCheckLevelUP();
        }
        if (state == GameState.StatusUPSelection)
        {
            HandleStatusUPSelection();
        }

        if (state == GameState.EnemyTurn)
        {
            HandleUpdateEnemyTurn();
        }
        if (state == GameState.End)
        {
            HandleUpdateEnd();
        }
    }

    // Idle時の処理
    void HandleUpdateIdle()
    {
        if (Input.GetKey(KeyCode.RightArrow)|| Input.GetKey(KeyCode.LeftArrow)|| Input.GetKey(KeyCode.UpArrow)|| Input.GetKey(KeyCode.DownArrow))
        {
            state = GameState.PlayerTurn;
        }

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

    void HandlePlayerTurn()
    {
        player.HandleUpdate();
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
        int min = 0;
        int max = 1;

        if (naichilab.RankingLoader.Instance.IsOpeningRanking)
        {
            return;
        }
        // 方向キーでアイテム選択
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentResult++;
            if (currentResult <= max)
            {
                SoundManager.instance.PlaySE(SoundManager.SE.Cursor);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentResult--;
            if (currentItemSlot >= min)
            {
                SoundManager.instance.PlaySE(SoundManager.SE.Cursor);
            }
        }
        currentResult = Mathf.Clamp(currentResult, min, max);
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

    void OnPlayerAttack()
    {   
        state = GameState.PlayerAttack;
    }

    void PlayerEnd()
    {
        // Debug.Log("PlayerEnd");
        // GameState.BusyだったらBusyのまま, そうじゃないなら敵のターン
        switch (state)
        {
            case GameState.Busy:
            case GameState.Goal:
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

    // 移動するやつが全部移動してから, 攻撃する

    IEnumerator MoveEnemies()
    {
        if (enemies.Count == 0)
        {
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].gameObject.activeSelf)
            {
                continue;
            }
            if (enemies[i].CheckAttack())
            {
                while (IsMovingPlayer())
                {
                    yield return null;
                }
            }
            bool attacked = enemies[i].MoveEnemy();
            if (attacked)
            {
                yield return new WaitForSeconds(0.5f);
            }
            playerStatusUI.SetData(player.Status);
        }
        while (ChechEnemyTurnEnd())
        {
            yield return null;
        }
        switch (state)
        {
            case GameState.Goal:
            case GameState.GameOver:
                break;
            default:
                state = GameState.End;
                break;
        }
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
        if (IsMovingPlayer())
        {
            return true;
        }
        return false;
    }

    bool IsMovingPlayer()
    {
        return player.IsMoving;
    }

    // インベントリを開いてる時の処理
    void HandleUpdateInventory()
    {
        int min = 0;
        int max = inventory.List.Count - 1;

        // 方向キーでアイテム選択
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentItemSlot--;
            if (currentItemSlot >= min)
            {
                SoundManager.instance.PlaySE(SoundManager.SE.Cursor);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentItemSlot++;
            if (currentItemSlot <= max)
            {
                SoundManager.instance.PlaySE(SoundManager.SE.Cursor);
            }
        }
        currentItemSlot = Mathf.Clamp(currentItemSlot, min, max);

        // 選んだものに色をつける
        inventoryUI.UpdateInventorySelection(currentItemSlot);
        if (inventory.List.Count > 0)
        {
            Item selectedItem = inventory.List[currentItemSlot];
            switch (selectedItem.Type)
            {
                case ItemType.HPHeal:
                    messageUI.SetMessage($"HPを{selectedItem.Amount}回復する");
                    break;
                case ItemType.SleepPointHeal:
                    messageUI.SetMessage($"安眠度を{selectedItem.Amount}回復する\n安眠度が0になるとHPが徐々に減る");
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && inventory.List.Count > 0)
        {
            // 決定
            // SoundManager.instance.PlaySE(SoundManager.SE.Heal);
            Item selectedItem = inventory.List[currentItemSlot];
            switch(selectedItem.Type)
            {
                case ItemType.HPHeal:
                    messageUI.SetMessage($"羊は {selectedItem.Name} を使った!\nHPが<color=#FFAC00>{selectedItem.Amount}</color>回復した");
                    break;
                case ItemType.SleepPointHeal:
                    messageUI.SetMessage($"羊は {selectedItem.Name} を使った!\n安眠度が<color=#FFAC00>{selectedItem.Amount}</color>回復した");
                    break;
            }
            selectedItem.Use(player);
            inventory.List.Remove(selectedItem);
            playerStatusUI.SetData(player.Status);
            StartCoroutine(UseItem());
            CloseInventory();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            // キャンセル
            SoundManager.instance.PlaySE(SoundManager.SE.Cancel);
            state = GameState.Idle;
            CloseInventory();
        }
    }

    IEnumerator UseItem()
    {
        yield return new WaitForSeconds(0.75f);
        state = GameState.EnemyTurn;
    }

    void OpenInventory()
    {
        SoundManager.instance.PlaySE(SoundManager.SE.OpenInventory);
        state = GameState.OpenInventory;
        currentItemSlot = 0;
        inventoryUI.gameObject.SetActive(true);
        inventoryUI.SetInventorySlots(inventory);
    }
    void CloseInventory()
    {
        inventoryUI.gameObject.SetActive(false);
    }

    // ステータスUPパネルの操作:インベントリと似たコード
    void HandleStatusUPSelection()
    {
        int min = 0;
        int max = statusUPSelection.StatusUPCards.Length - 1;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentStatusUPIndex++;
            if (currentStatusUPIndex <= max)
            {
                SoundManager.instance.PlaySE(SoundManager.SE.Cursor);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentStatusUPIndex--;
            if (currentStatusUPIndex >= min)
            {
                SoundManager.instance.PlaySE(SoundManager.SE.Cursor);
            }
        }
        currentStatusUPIndex = Mathf.Clamp(currentStatusUPIndex, min, max);
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
        SoundManager.instance.PlaySE(SoundManager.SE.OpenInventory);
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
            SoundManager.instance.PlaySE(SoundManager.SE.InventoryMax);
            messageUI.SetMessage("持ち物がいっぱいだ");
        }
        else
        {
            SoundManager.instance.PlaySE(SoundManager.SE.GetItem);
            inventory.List.Add(itemObj.Item);
            itemObj.gameObject.SetActive(false);
            switch (itemObj.Item.Type)
            {
                case ItemType.HPHeal:
                    messageUI.SetMessage("ハーブを手に入れた!\nHPの回復に使えるぞ");
                    break;
                case ItemType.SleepPointHeal:
                    messageUI.SetMessage("ハーブティーを手に入れた!\n安眠度の回復に使えるぞ");
                    break;
            }
        }
    }
    void GameOver()
    {
        // enabled = false;
        messageUI.SetMessage($"羊は眠りから覚めてしまった");
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
        messageUI.SetMessage($"より深く...より深く...");
        SoundManager.instance.PlaySE(SoundManager.SE.Stairs);
        state = GameState.Goal;
        Debug.Log("Restart");
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
