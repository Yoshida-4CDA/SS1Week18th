using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] Text hpText = default;
    [SerializeField] Text atText = default;
    [SerializeField] Text expText = default;
    [SerializeField] Text levelText = default;
    [SerializeField] Text stageText = default;
    [SerializeField] Text sleepPointText = default;

    public void SetData(PlayerStatus playerStatus)
    {
        hpText.text = $"HP:{playerStatus.hp}/{playerStatus.maxHP}";
        atText.text = $"AT:{playerStatus.at}";
        expText.text = $"EXP:{playerStatus.exp}";
        levelText.text = $"LV:{playerStatus.level}";
        stageText.text = $"睡眠時間:{playerStatus.currentStage-1}";
        if (playerStatus.sleepPoint <= 0)
        {
            hpText.text = $"HP:<color=#E74B68>{playerStatus.hp}</color>/{playerStatus.maxHP}";
            sleepPointText.text = $"安眠度:<color=#E74B68>{playerStatus.sleepPoint}</color>%";
        }
        else
        {
            hpText.text = $"HP:{playerStatus.hp}/{playerStatus.maxHP}";
            sleepPointText.text = $"安眠度:{playerStatus.sleepPoint}%";
        }
    }
}
