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
        Nightmare,
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
    int currentBGM;
    public void StopBGM()
    {
        audioSourceBGM.Stop();
    }

    public void PlayBGM(BGM bgm)
    {
        int index = (int)bgm;

        if (currentBGM == index && audioSourceBGM.isPlaying)
        {
            return;
        }
        currentBGM = index;
        StopBGM();
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
