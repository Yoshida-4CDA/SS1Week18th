using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] Text hpText = default;
    [SerializeField] Text atText = default;
    void Start()
    {
        SetData(20,12);
    }

    public void SetData(int hp, int at)
    {
        hpText.text = $"HP:{hp}";
        atText.text = $"AT:{at}";
    }
}
