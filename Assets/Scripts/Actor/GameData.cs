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
    public int hp;
    public int at;
    public int currentStage;
}