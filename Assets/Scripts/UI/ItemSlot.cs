using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    Text itemText;
    [SerializeField] Color hilightColor;
    Color defaultColor;
    private void Awake()
    {
        itemText = GetComponent<Text>();
        defaultColor = itemText.color;
    }

    public void SetData(string text)
    {
        itemText.text = text;
    }

    public void SetSelection(bool selected)
    {
        if (selected)
        {
            itemText.color = hilightColor;
        }
        else
        {
            itemText.color = defaultColor;
        }
    }
}
