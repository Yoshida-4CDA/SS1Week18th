using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] Text hpText = default;
    [SerializeField] Text atText = default;

    public void SetData(PlayerStatus playerStatus)
    {
        hpText.text = $"HP:{playerStatus.hp}";
        atText.text = $"AT:{playerStatus.at}";
    }
}
