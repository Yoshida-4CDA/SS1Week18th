using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // BGMを列挙
    public enum IndexBGM
    {
        Bgm1,
        Bgm2,
        Bgm3,
    }

    // SEを列挙
    public enum IndexSE
    {
        Se1,
        Se2,
        Se3,
        Se4,
        Se5,
        Se6,
        Se7,
        Se8,
        Se9,
    }

    public static SoundManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // BGM
    [SerializeField] AudioSource audioSourceBGM;
    [SerializeField] AudioClip[] audioClipBGM;

    // SE
    [SerializeField] AudioSource audioSourceSE;
    [SerializeField] AudioClip[] audioClipSE;

    public void StopBGM()
    {
        audioSourceBGM.Stop();
    }

    public void PlayBGM(int index)
    {
        StopBGM();
        audioSourceBGM.clip = audioClipBGM[index];
        audioSourceBGM.Play();
    }

    public void PlaySE(int index)
    {
        audioSourceSE.PlayOneShot(audioClipSE[index]);
    }
}
