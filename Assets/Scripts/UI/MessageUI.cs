using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    [SerializeField] Text messageText = default;

    public static MessageUI instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void DelayMessage(string message)
    {
        StopAllCoroutines();
        StartCoroutine(Delay(message));
    }

    IEnumerator Delay(string message)
    {
        messageText.text = "";
        yield return null;
        SetMessage(message);
    }

    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}
