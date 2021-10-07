using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUPCard : MonoBehaviour
{
    public enum Type
    {
        HPUP,
        ATUP,
        SLPUP,
    }
    [SerializeField] Text text;


    [SerializeField] GameObject hilightPanel;
    [SerializeField] Type type;
    int amount; // 増加量


    private void Awake()
    {
        switch (type)
        {
            case Type.HPUP:
                amount = ParamsSO.Entity.statusUPAddHP;
                text.text = $"最大HP ＋{amount}";
                break;
            case Type.ATUP:
                amount = ParamsSO.Entity.statusUPAddAT;
                text.text = $"AT ＋{amount}";
                break;
            case Type.SLPUP:
                text.text = $"安眠度100%";
                break;
        }

    }

    public void UseCard(Player player)
    {
        switch (type)
        {
            case Type.HPUP:
                player.StatusUpMaxHP(amount);
                break;
            case Type.ATUP:
                player.StatusUpAT(amount);
                break;
            case Type.SLPUP:
                player.HealSLP(100);
                break;
        }
    }

    // 選択中なら色をつける
    public void SetSelection(bool selected)
    {
        hilightPanel.SetActive(selected);
        if (selected)
        {
            transform.localScale = Vector3.one * 1.1f;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
}