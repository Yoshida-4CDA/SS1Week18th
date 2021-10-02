using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    [SerializeField] Text messageText = default;
    void Start()
    {
        SetMessage("羊はバクに攻撃した");
    }

    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}
