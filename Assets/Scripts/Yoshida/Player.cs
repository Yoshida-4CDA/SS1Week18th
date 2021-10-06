﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [Header("移動中かどうかを判別する変数")]
    [SerializeField] bool isMoving;

    int axisX;
    int axisY;

    PlayerStatus status = new PlayerStatus();

    public UnityAction OnPlayerTurnEnd;
    public UnityAction OnGameOver;
    public UnityAction OnGoal;
    public UnityAction<ItemObj> OnItem;

    // 目的地
    ObjectPosition objectPositionTool;

    public PlayerStatus Status { get => status; }
    public bool IsMoving { get => isMoving; }

    [SerializeField] GameObject damageCanvasPrefab;

    public void Init()
    {
        status.level = GameData.instance.PlayerStatus.level;
        status.maxHP = GameData.instance.PlayerStatus.maxHP;
        status.hp = GameData.instance.PlayerStatus.hp;
        status.at = GameData.instance.PlayerStatus.at;
        status.exp = GameData.instance.PlayerStatus.exp;
        status.currentStage = GameData.instance.PlayerStatus.currentStage;
        status.levelUPExp = GameData.instance.PlayerStatus.levelUPExp;
        objectPositionTool = GetComponent<ObjectPosition>();
        objectPositionTool.nextMovePosition = objectPositionTool.Grid;
        // Debug.Log($"PlayerのHP：{status.hp}　AT：{status.at}　経験値：{status.exp}");
    }

    public void HandleUpdate()
    {
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
            objectPositionTool.nextMovePosition = objectPositionTool.Grid + new Vector2Int(axisX, -axisY);
            // 移動先
            ATMove(axisX, axisY);
        }
    }

    public void ATMove(int x, int y)
    {
        // Move関数を呼んでRayを飛ばす
        bool canMove = Move(x, y);


        Enemy hitComponent = null;
        if (objectPositionTool.IsOverlapPointNextMove() != null)
        {
            hitComponent = objectPositionTool.IsOverlapPointNextMove().GetComponent<Enemy>();
        }
        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
            objectPositionTool.nextMovePosition = objectPositionTool.Grid;
        }
        else if(!canMove)
        {
            objectPositionTool.nextMovePosition = objectPositionTool.Grid;
        }
        // プレイヤーのターン(攻撃)終了
        CheckHP();
        OnPlayerTurnEnd();
    }

    public bool Move(int x, int y)
    {
        Vector2 startPos = transform.position;          // プレイヤーの現在位置
        Vector2 endPos = startPos + new Vector2(x, y);  // 移動したい位置

        // 何もぶつかるものが無ければ移動できる
        if (!isMoving && !isMoving && !objectPositionTool.IsWall(endPos) && objectPositionTool.IsOverlapPointNextMove() == null)
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
            transform.position = Vector3.MoveTowards(currentPos, targetPos, 5f*Time.deltaTime);
            distance = (transform.position - targetPos).sqrMagnitude;
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
        CheckHP();
    }

    void OnCantMove(Enemy enemy)
    {
        Debug.Log("Playerの攻撃");
        enemy.EnemyDamage(status.at);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            // status.hp += GameData.instance.itemPoint;
            Debug.Log($"PlayerのHP：{status.hp}");
            ItemObj itemObj = collision.GetComponent<ItemObj>();
            OnItem?.Invoke(itemObj);
        }
        if (collision.gameObject.CompareTag("Finish"))
        {
            OnGoal?.Invoke();
        }
    }

    public void PlayerDamage(int damage)
    {
        status.hp -= damage;
        SpawnCanvasPrefab(transform.position, damage);

        CheckHP();
    }

    void SpawnCanvasPrefab(Vector2 position, int damage)
    {
        GameObject effectObj = Instantiate(damageCanvasPrefab, position, Quaternion.identity);
        DamageEffect damageEffect = effectObj.GetComponent<DamageEffect>();
        damageEffect.ShowDamage(damage);
    }

    void OnDisable()
    {
        GameData.instance.PlayerStatus = status;
    }

    void CheckHP()
    {
        if (status.hp <= 0)
        {
            OnGameOver();
        }
    }

    public void AddExp(int exp)
    {
        status.exp += exp;
    }

    public void LevelUP()
    {
        if (status.IsLevelUP)
        {
            status.exp -= status.levelUPExp;
            status.level++;
        }
    }

    public void StatusUpMaxHP(int amount)
    {
        status.maxHP += amount;
        status.hp += amount;
    }
    public void StatusUpAT(int amount)
    {
        status.at += amount;
    }
    public void Heal(int amount)
    {
        status.hp += amount;
        status.hp = Mathf.Clamp(status.hp, 0, status.maxHP);
    }

}
