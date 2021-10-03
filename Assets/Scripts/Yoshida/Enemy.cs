using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("移動中かどうかを判別する変数")]
    [SerializeField] bool isMoving;

    BoxCollider2D boxCollider2D;
    [SerializeField] LayerMask blockingLayer;
    Transform target;       // プレイヤー(target)の座標
    // bool skipMove;          // 敵のターンをスキップさせる変数

    [SerializeField] int enemyHp;   // EnemyのHP
    [SerializeField] Text hpText;   // EnemyのHPテキスト
    [SerializeField] int enemyAt;   // EnemyのAT
    [SerializeField] Text atText;   // EnemyのATテキスト

    void Start()
    {
        GameManager.instance.AddEnemy(this);

        boxCollider2D = GetComponent<BoxCollider2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;   // プレイヤーの位置情報を取得

        // hpText.text = $"HP：{enemyHp}";
        // atText.text = $"AT：{enemyAt}";
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        // Playerと同じx軸にいるかどうかを判定
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            // y軸を動かす(PlayerがEnemyより高い位置にいるなら上/低い位置にいるなら下に移動する)
            if (target.transform.position.y > transform.position.y)
            {
                yDir = 1;
            }
            else
            {
                yDir = -1;
            }
        }
        else
        {
            // x軸を動かす(PlayerがEnemyより高い位置にいるなら右/低い位置にいるなら左に移動する)
            if (target.transform.position.x > transform.position.x)
            {
                xDir = 1;
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                xDir = -1;
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        ATMove(xDir, yDir);
    }

    public void ATMove(int x, int y)
    {
        RaycastHit2D hit;

        // Move関数を呼んでRayを飛ばす => あとあと使うかも?
        bool canMove = Move(x, y, out hit);

        // Rayにぶつかるものが無ければ移動できる
        if (hit.transform == null)
        {
            return;
        }

        Player hitComponent = hit.transform.GetComponent<Player>();

        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
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

    IEnumerator Movement(Vector3 endPos)
    {
        isMoving = true;

        // 移動したい位置との距離の2乗を計算
        float remainingDistance = (transform.position - endPos).sqrMagnitude;

        while (remainingDistance > float.Epsilon)
        {
            // プレイヤーの現在位置を変更(現在位置, 移動したい先の位置, 変化量)
            transform.position = Vector3.MoveTowards(this.gameObject.transform.position, endPos, 10f * Time.deltaTime);

            // 移動したい位置との距離の2乗を再度計算
            remainingDistance = (transform.position - endPos).sqrMagnitude;

            yield return null;
        }
        // 目的地に移動させる
        transform.position = endPos;

        isMoving = false;
    }

    void OnCantMove(Player player)
    {
        Debug.Log("Enemyの攻撃");
        player.PlayerDamage(enemyAt);
    }

    public void EnemyDamage(int damage)
    {
        enemyHp -= damage;
        // hpText.text = $"HP：{enemyHp}";

        if (enemyHp <= 0)
        {
            Debug.Log("Enemyを倒した");
            GameManager.instance.DestroyEnemyToList(this);
            gameObject.SetActive(false);
            // hpText.text = "";
            // atText.text = "";
        }
    }
}
/*
         if (!skipMove)
        {
            skipMove = true;    // 次のターンは動けないようにする
            int xDir = 0;
            int yDir = 0;

            // Playerと同じx軸にいるかどうかを判定
            if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            {
                // y軸を動かす(PlayerがEnemyより高い位置にいるなら上/低い位置にいるなら下に移動する)
                if (target.transform.position.y > transform.position.y)
                {
                    yDir = 1;
                }
                else
                {
                    yDir = -1;
                }
            }
            else
            {
                // x軸を動かす(PlayerがEnemyより高い位置にいるなら右/低い位置にいるなら左に移動する)
                if (target.transform.position.x > transform.position.x)
                {
                    xDir = 1;
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    xDir = -1;
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
            ATMove(xDir, yDir);
        }
        else
        {
            skipMove = false;   // 次のターンは動けるようにする
        }
*/