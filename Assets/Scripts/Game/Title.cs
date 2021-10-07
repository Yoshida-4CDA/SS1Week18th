using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] Fade fade;

    void Start()
    {
        SoundManager.instance.PlayBGM(SoundManager.BGM.Title);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SoundManager.instance.PlaySE(SoundManager.SE.GameStart);
            fade.FadeIn(2.5f, () => SceneManager.LoadScene("Main"));  // フェードイン演出
        }
    }
}
