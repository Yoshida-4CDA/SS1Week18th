using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool playerTurn = true;
    public bool enemyTurn = false;

    public int initPlayerHp;    // Playerの初期HP
    public int itemPoint;       // Itemの回復量

    List<Enemy> enemies;        // Enemyを管理するリスト

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        enemies = new List<Enemy>();
    }

    void Start()
    {
        enemies.Clear();
    }

    void Update()
    {
        if (playerTurn || enemyTurn)
        {
            return;
        }
        StartCoroutine(MoveEnemies());
    }

    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void DestroyEnemyToList(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    IEnumerator MoveEnemies()
    {
        enemyTurn = true;
        yield return new WaitForSeconds(0.1f);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(0.1f);
        }

        enemyTurn = false;
        playerTurn = true;
    }
}
