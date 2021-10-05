using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [SerializeField] PlayerStatus playerStatus;
    public PlayerStatus PlayerStatus { get => playerStatus; set => playerStatus = value; } // 公開用


    public static GameData instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            playerStatus = ParamsSO.Entity.initPlayerStatus;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
public class PlayerStatus
{
    public int level;
    public int hp;
    public int maxHP;
    public int at;
    public int exp;
    public int currentStage;
    public int levelUPExp;   // この経験値以上になったらレベルアップ

    public bool IsLevelUP { get => exp >= levelUPExp; }
}