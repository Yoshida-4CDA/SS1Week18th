using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] Text text;

    public void SetResult(int time)
    {
        text.text = $"睡眠時間... {time} 時間経過";

    }
}
