using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [Header("移動中かどうかを判別する変数")]
    public bool isMoving;

    Transform target;               // プレイヤー(target)の座標

    EnemyStatus status = new EnemyStatus();

    public UnityAction<Enemy> OnDestroyEnemy;

    public int HP { get => status.hp; }
    public int Exp { get => status.exp; }

    ObjectPosition objectPositionTool;


    void Start()
    {
        objectPositionTool = GetComponent<ObjectPosition>();
        status.Set(ParamsSO.Entity.initEnemyStatusList[0]);
        target = GameObject.FindGameObjectWithTag("Player").transform;   // プレイヤーの位置情報を取得
        Debug.Log($"EnemyのHP：{status.hp}　AT：{status.at}　経験値：{status.exp}");
        objectPositionTool.nextMovePosition = objectPositionTool.Grid;

    }

    // TODO:Enemyの移動修正
    /*
     PlayerとEnemyが・・・

        ＊同じX軸にいるなら => 上下どちらかにY軸を動かす
        ＊違うX軸にいるなら => 左右どちらかにX軸を動かす

        ＊同じY軸にいるなら => 左右どちらかにX軸を動かす
        ＊違うY軸にいるなら => 上下どちらかにY軸を動かす

        ということは・・・

            ＊X軸を動かすとき => 違うX軸 or 同じY軸
            ＊Y軸を動かすとき => 同じX軸 or 違うY軸
     */
    public bool MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        // Playerと同じx軸にいるかどうかを判定
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon ||
            Mathf.Abs(target.position.y - transform.position.y) >= float.Epsilon)
        {
            // y軸を動かす(PlayerがEnemyより高い位置にいるなら上/低い位置にいるなら下に移動する)
            yDir = target.transform.position.y > transform.position.y ? 1 : -1;

            // 上下移動が出来ないなら左右に移動する => 壁にぶつかってるかどうか判定する -> どうやって？
            // xDir = target.transform.position.x > transform.position.x ? 1 : -1;
        }
        else if (Mathf.Abs(target.position.x - transform.position.x) >= float.Epsilon ||
                 Mathf.Abs(target.position.y - transform.position.y) < float.Epsilon)
        {
            // x軸を動かす(PlayerがEnemyより高い位置にいるなら右/低い位置にいるなら左に移動する)
            xDir = target.transform.position.x > transform.position.x ? 1 : -1;

            // 左右移動が出来ないなら上下に移動する => 壁にぶつかってるかどうか判定する -> どうやって？
            // yDir = target.transform.position.y > transform.position.y ? 1 : -1;
        }

        switch (xDir)
        {
            case 1:
                transform.localScale = new Vector3(-1, 1, 1);
                break;
            case -1:
                transform.localScale = new Vector3(1, 1, 1);
                break;
        }
        return ATMove(xDir, yDir);
    }

    public bool ATMove(int x, int y)
    {

        // Move関数を呼んでRayを飛ばす
        bool canMove = Move(x, y);


        // 重なりがあるか?Playerか？
        ObjectPosition hitComponent = objectPositionTool.IsOverlapPointNextMove();
        if (!canMove && hitComponent != null && hitComponent.GetComponent<Player>())
        {
            OnCantMove(hitComponent.GetComponent<Player>());
            objectPositionTool.nextMovePosition = objectPositionTool.Grid;
            return true;
        }
        else if (!canMove)
        {
            objectPositionTool.nextMovePosition = objectPositionTool.Grid;
        }
        return false;
    }

    public bool Move(int x, int y)
    {
        Vector2 startPos = transform.position;          // プレイヤーの現在位置
        Vector2 endPos = startPos + new Vector2(x, y);  // 移動したい位置

        objectPositionTool.nextMovePosition = objectPositionTool.Grid + new Vector2Int(x, -y);

        if (!isMoving && !objectPositionTool.IsWall(endPos) && objectPositionTool.IsOverlapPointNextMove() == null)
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
            transform.position = Vector3.MoveTowards(this.gameObject.transform.position, endPos, 5f * Time.deltaTime);

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
        player.PlayerDamage(status.at);
    }

    public void EnemyDamage(int damage)
    {
        status.hp -= damage;
        Debug.Log($"EnemyのHP：{status.hp}");

        if (status.hp <= 0)
        {
            Debug.Log("Enemyを倒した");
            OnDestroyEnemy?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public class EnemyStatus
{
    public int level; // だんだん強くなるといいなぁ
    public string name;
    public int hp;
    public int at; 
    public int exp;

    public void Set(EnemyStatus status)
    {
        level = status.level;
        name = status.name;
        hp = status.hp;
        at = status.at; 
        exp = status.at;
    }
}