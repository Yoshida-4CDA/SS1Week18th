using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    [SerializeField] Fade fade;

    void Awake()
    {
        fade.FadeOut(1f);   // フェードアウト演出
    }
}
