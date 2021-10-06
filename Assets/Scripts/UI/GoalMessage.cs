using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalMessage : MonoBehaviour
{
    [SerializeField] Text message;
    [SerializeField] HilightText[] hilightTexts;

    private void Awake()
    {
        message = GetComponentInChildren<Text>();
        hilightTexts = GetComponentsInChildren<HilightText>();
        foreach (var obj in hilightTexts)
        {
            obj.gameObject.SetActive(false);
        }
    }

    public void SetSleepTime(int time)
    {
        gameObject.SetActive(true);
        this.message.text = $"眠り始めて... {time} 時間経過";
    }
    public void ShowResult(int time)
    {
        gameObject.SetActive(true);
        this.message.text = $"GAME OVER\n\n 合計睡眠時間... {time} 時間";
        naichilab.RankingLoader.Instance.IsOpeningRanking = true;
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(time);
    }

    public void HandleUpdateSelection(int selected)
    {
        for (int i=0; i<hilightTexts.Length; i++)
        {
            hilightTexts[i].gameObject.SetActive(true);
            hilightTexts[i].SetSelection(selected == i);
        }
    }
}
