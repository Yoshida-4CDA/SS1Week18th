using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]//Createメニューから作成できるようする属性
public class ParamsSO : ScriptableObject
{

    //MyScriptableObjectが保存してある場所のパス
    public const string PATH = "ScriptableObjects/ParamsSO";

    //MyScriptableObjectの実体
    private static ParamsSO _entity;
    public static ParamsSO Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<ParamsSO>(PATH);

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(PATH + " not found");
                }
            }

            return _entity;
        }
    }

    //保存されているデータ
    public PlayerStatus initPlayerStatus;
    // 敵の強さ
    public List<EnemyStatus> initEnemyStatusList = new List<EnemyStatus>();
    // 被ダメージエフェクトのカラー(プレイヤー)
    public Color playerDamageEffectColor;
    // 被ダメージエフェクトのカラー(敵)
    public Color enemyDamageEffectColor;
    public int healPointUsedHerb;
    public int healPointUsedHerbTea;
    public int statusUPAddHP;
    public int statusUPAddAT;
    public int enemyLevelUPAddHP;
    public int enemyLevelUPAddAT;
    public int rateHerb;
    public int rateHerbTea;
    public int rateEmpty;
}