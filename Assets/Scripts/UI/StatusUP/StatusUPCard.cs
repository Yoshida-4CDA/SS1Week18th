using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusUPCard : MonoBehaviour
{
    [SerializeField] GameObject hilightPanel;

    private void Awake()
    {
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
