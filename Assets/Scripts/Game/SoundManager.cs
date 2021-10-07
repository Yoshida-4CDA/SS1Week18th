using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // BGMを列挙
    public enum IndexBGM
    {
        Title,
        Main,
    }

    // SEを列挙
    public enum IndexSE
    {
        GameOver,
        Attack,
        Damage,
        Heal,
        GetItem,
        LevelUp,
        Cursor,
        Decision,
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

    [Tooltip("0 = Title, 1 = Main")]
    [SerializeField] AudioClip[] audioClipBGM;

    // SE
    [SerializeField] AudioSource audioSourceSE;
    [SerializeField] AudioClip[] audioClipSE;

    // SEのピッチ調整用変数
    float low = .95f;
    float high = 1.05f;

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

    // 
    public void PlayRandomSE(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(low, high);

        audioSourceSE.pitch = randomPitch;
        audioSourceSE.clip = clips[randomIndex];

        audioSourceSE.PlayOneShot(audioSourceSE.clip);
    }
}
