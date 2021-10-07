using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // BGMを列挙
    public enum BGM
    {
        Title,
        Main,
    }

    // SEを列挙
    public enum SE
    {
        GameStart,
        GameOver,
        Attack,
        Damage,
        GetItem,
        InventoryMax,
        OpenInventory,
        Cursor,
        Cancel,
        HPHeal,
        SleepPointHeal,
        LevelUP,
        HPUP,
        ATUP,
        Stairs,
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

    public void PlayBGM(BGM bgm)
    {
        StopBGM();
        int index = (int)bgm;
        audioSourceBGM.clip = audioClipBGM[index];
        audioSourceBGM.Play();
    }

    public void PlaySE(SE se)
    {
        int index = (int)se;
        AudioClip clip = audioClipSE[index];
        audioSourceSE.PlayOneShot(clip);
    }
}
