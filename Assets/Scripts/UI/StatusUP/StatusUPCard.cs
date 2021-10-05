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
    }
    [SerializeField] Text text;


    [SerializeField] GameObject hilightPanel;
    [SerializeField] Type type;
    [SerializeField] int amount; // 増加量


    private void Awake()
    {
        switch (type)
        {
            case Type.HPUP:
                text.text = $"HP:{amount}UP";
                break;
            case Type.ATUP:
                text.text = $"AT:{amount}UP";
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