using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HilightText : MonoBehaviour
{
    Text text;
    [SerializeField] Color hilightColor;
    Color defaultColor;
    private void Awake()
    {
        text = GetComponent<Text>();
        defaultColor = text.color;
    }

    public void SetData(string text)
    {
        this.text.text = text;
    }

    public void SetSelection(bool selected)
    {
        if (selected)
        {
            text.color = hilightColor;
        }
        else
        {
            text.color = defaultColor;
        }
    }
}

