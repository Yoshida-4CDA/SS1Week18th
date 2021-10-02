using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool playerTurn = true;
    public bool enemyTurn = false;

    Enemy enemy;

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

        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
    }

    void Update()
    {
        if (playerTurn || enemyTurn)
        {
            return;
        }
        StartCoroutine(MoveEnemies());
    }

    IEnumerator MoveEnemies()
    {
        enemyTurn = true;
        yield return new WaitForSeconds(0.1f);

        enemy.MoveEnemy();
        yield return new WaitForSeconds(0.1f);

        enemyTurn = false;
        playerTurn = true;
    }
}
