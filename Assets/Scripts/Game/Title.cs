using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] Fade fade;
    bool onStart;
    void Start()
    {
        SoundManager.instance.PlayBGM(SoundManager.BGM.Title);
    }

    void Update()
    {
        if (onStart)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            onStart = true;
            SoundManager.instance.PlaySE(SoundManager.SE.GameStart);
            fade.FadeIn(2.5f, () => SceneManager.LoadScene("Main"));  // ?t?F?[?h?C?????o
        }
    }
}
