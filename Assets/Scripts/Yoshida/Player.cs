using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("移動中かどうかを判別する変数")]
    [SerializeField] bool isMoving;

    int axisX;
    int axisY;

    BoxCollider2D boxCollider2D;
    [SerializeField] LayerMask blockingLayer;

    int playerHp;                       // PlayerのHP
    [SerializeField] Text hpText;       // PlayerのHPテキスト
    [SerializeField] int playerAt;      // PlayerのAT
    [SerializeField] Text atText;       // PlayerのATテキスト

    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        playerHp = GameManager.instance.initPlayerHp;
        // hpText.text = $"HP：{playerHp}";
        // atText.text = $"AT：{playerAt}";
    }

    void Update()
    {
        if (!GameManager.instance.playerTurn)
        {
            return;
        }

        axisX = (int)Input.GetAxisRaw("Horizontal");
        axisY = (int)Input.GetAxisRaw("Vertical");

        if (axisX != 0)
        {
            axisY = 0;

            if (axisX < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (axisX > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else if (axisY != 0)
        {
            axisX = 0;
        }

        if (axisX != 0 || axisY != 0)
        {
            // 移動
            ATMove(axisX, axisY);
        }
    }

    public void ATMove(int x, int y)
    {
        RaycastHit2D hit;

        // Move関数を呼んでRayを飛ばす => あとあと使うかも?
        bool canMove = Move(x, y, out hit);

        // Rayにぶつかるものが無ければ移動できる
        if (hit.transform == null)
        {
            // プレイヤーのターン(移動)終了
            GameManager.instance.playerTurn = false;
            return;
        }

        Enemy hitComponent = hit.transform.GetComponent<Enemy>();

        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
        // プレイヤーのターン(攻撃)終了
        GameManager.instance.playerTurn = false;
    }

    public bool Move(int x, int y, out RaycastHit2D hit)
    {
        Vector2 startPos = transform.position;          // プレイヤーの現在位置
        Vector2 endPos = startPos + new Vector2(x, y);  // 移動したい位置

        // 自身のBoxCollider2DにRayが反応してしまわないように一旦falseにする
        boxCollider2D.enabled = false;

        // Rayを飛ばす(開始位置, 終了位置, 判定するLayer)
        hit = Physics2D.Linecast(startPos, endPos, blockingLayer);

        boxCollider2D.enabled = true;

        // 何もぶつかるものが無ければ移動できる
        if (!isMoving && hit.transform == null)
        {
            StartCoroutine(Movement(endPos));
            return true;
        }
        return false;
    }

    IEnumerator Movement(Vector3 targetPos)
    {
        isMoving = true;

        float distance = (transform.position - targetPos).sqrMagnitude; // 現在地と目的地との距離
        while (distance > float.Epsilon)
        {
            Vector3 currentPos = this.gameObject.transform.position;
            transform.position = Vector3.MoveTowards(currentPos, targetPos, 6f * Time.deltaTime);
            distance = (transform.position - targetPos).sqrMagnitude;
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }

    void OnCantMove(Enemy enemy)
    {
        Debug.Log("Playerの攻撃");
        enemy.EnemyDamage(playerAt);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            playerHp += GameManager.instance.itemPoint;
            // hpText.text = $"HP：{playerHp}";
            collision.gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("Finish"))
        {
            Invoke("Restart", 1f);
            enabled = false;
        }
    }

    public void PlayerDamage(int damage)
    {
        playerHp -= damage;
        // hpText.text = $"HP：{playerHp}";
    }

    void OnDisable()
    {
        GameManager.instance.initPlayerHp = playerHp;
    }

    public void Restart()
    {
        SceneManager.LoadScene(2);
    }
}
