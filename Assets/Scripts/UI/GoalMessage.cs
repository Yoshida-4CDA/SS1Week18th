using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalMessage : MonoBehaviour
{
    [SerializeField] Text message;
    [SerializeField] HilightText[] hilightTexts;

    bool IsOpenedRankingBoard;

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
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySE(SoundManager.SE.GameOver);

        gameObject.SetActive(true);
        this.message.text = $"GAME OVER\n\n 合計睡眠時間... {time} 時間";
        naichilab.RankingLoader.Instance.IsOpeningRanking = true;

        if (!IsOpenedRankingBoard)
        {
            StartCoroutine(ShowRankingBoard(time));
        }
    }

    IEnumerator ShowRankingBoard(int time)
    {
        IsOpenedRankingBoard = true;

        yield return new WaitForSeconds(1.5f);
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
