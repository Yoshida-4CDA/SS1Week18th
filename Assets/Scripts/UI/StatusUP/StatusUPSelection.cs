using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusUPSelection : MonoBehaviour
{
    // ステータスUP時に表示されるパネルの制御
    // どのステータスUPを選択しているのか色をつける
    [SerializeField] StatusUPCard[] statusUPCards;

    // 他ファイルに公開用
    public StatusUPCard[] StatusUPCards { get => statusUPCards; }

    private void Awake()
    {
        statusUPCards = GetComponentsInChildren<StatusUPCard>();
    }

    public void UpdateCardSelection(int selectedItem)
    {
        for (int i = 0; i < statusUPCards.Length; i++)
        {
            statusUPCards[i].SetSelection(selectedItem == i);
        }
    }
}
