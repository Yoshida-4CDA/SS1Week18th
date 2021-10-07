using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] Fade fade;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fade.FadeIn(1.5f, () => SceneManager.LoadScene("Main"));  // フェードイン演出
        }
    }
}
