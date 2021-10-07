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

    public UnityAction OnPlayerAttack;
    public UnityAction OnPlayerTurnEnd;
    public UnityAction OnGameOver;
    public UnityAction OnGoal;
    public UnityAction<ItemObj> OnItem;

    // 目的地
    ObjectPosition objectPositionTool;
    Animator animator;


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
        animator = GetComponentInChildren<Animator>();
        // Debug.Log($"PlayerのHP：{status.hp}　AT：{status.at}　経験値：{status.exp}");
    }

    public void HandleUpdate()
    {
        axisX = (int)Input.GetAxisRaw("Horizontal");
        axisY = (int)Input.GetAxisRaw("Vertical");

        if (axisX != 0)
        {
            axisY = 0;
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
            animator.SetFloat("InputX", axisX);
            animator.SetFloat("InputY", axisY);
        }
    }


    void LateUpdate()
    {
        Vector2 input = Vector2.zero;
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Speed", input.sqrMagnitude);
    }

    public void ATMove(int x, int y)
    {
        // Move関数を呼んでRayを飛ばす
        bool canMove = Move(x, y);
        bool isAttack = false;


        Enemy hitComponent = null;
        if (objectPositionTool.IsOverlapPointNextMove() != null)
        {
            hitComponent = objectPositionTool.IsOverlapPointNextMove().GetComponent<Enemy>();
        }
        if (!canMove && hitComponent != null)
        {
            isAttack = true;
            // 攻撃する
            OnCantMove(hitComponent);
            objectPositionTool.nextMovePosition = objectPositionTool.Grid;
        }
        else if(!canMove)
        {
            objectPositionTool.nextMovePosition = objectPositionTool.Grid;
        }

        if (isAttack)
        {
            // プレイヤーのターン(攻撃)終了
            CheckHP();
            StartCoroutine(WaitAttackAnimation());
        }
        else
        {
            OnPlayerTurnEnd();
        }
    }

    //// 攻撃中ってのを取得した方がいい
    IEnumerator WaitAttackAnimation()
    {
        OnPlayerAttack();
        yield return new WaitForSeconds(0.5f);
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
        objectPositionTool.nextMovePosition = objectPositionTool.Grid;
    }

    void OnCantMove(Enemy enemy)
    {
        Debug.Log("Playerの攻撃");
        enemy.EnemyDamage(status.at);
        animator.SetTrigger("Attack");
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
        animator.SetTrigger("Damage");
    }

    void SpawnCanvasPrefab(Vector2 position, int damage)
    {
        GameObject effectObj = Instantiate(damageCanvasPrefab, position, Quaternion.identity);
        DamageEffect damageEffect = effectObj.GetComponent<DamageEffect>();
        damageEffect.text.color = ParamsSO.Entity.playerDamageEffectColor;
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
            animator.SetTrigger("GameOver");
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
