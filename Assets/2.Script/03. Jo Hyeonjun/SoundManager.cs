using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    // 0. 싱글톤 적용
    public static SoundManager instance = null;
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
        BGMPlayer = transform.GetChild(0).GetComponent<AudioSource>();
        SFXPlayer = transform.GetChild(1).GetComponents<AudioSource>();
    }

    // 1. 변수 모음
    [Header("Audio Clip")]
    [SerializeField] private Sound[] BGM;
    [SerializeField] private Sound[] SFX;
    [Header("AudioSource")]
    [SerializeField] private AudioSource BGMPlayer;
    [SerializeField] private AudioSource[] SFXPlayer;

    // 2-1. BGM 재생 
    // BGM이 여러 개임을 대비하여 선택형으로.
    public void PlayBGM(string name = "Playing")
    {
        foreach (Sound s in BGM)
        {
            if (s.name == name)
            {
                BGMPlayer.clip = s.clip;
                BGMPlayer.Play();
                return;
            }
        }
        Debug.Log($"PlayBGM {name} 없음");
    }

    // 2-2. BGM 정지
    public void StopBGM() { BGMPlayer.Stop(); }

    // 3-1. 효과음 재생
    public void PlaySFX(string name)
    {
        foreach (Sound s in SFX)
        {
            if (s.name.Equals(name))
            {
                // 0부터 사용중이지 않은 SFXPlayer를 탐색하여 사용
                for (int i = 0; i < SFXPlayer.Length; i++)
                {
                    if (!SFXPlayer[i].isPlaying)
                    {
                        SFXPlayer[i].clip = s.clip;
                        SFXPlayer[i].Play();
                        return;
                    }
                }

                // 모두 사용중이라면, 하나 추가하고 거기에 재생
                transform.GetChild(1).gameObject.AddComponent<AudioSource>();
                SFXPlayer[SFXPlayer.Length - 1].clip = s.clip;
                SFXPlayer[SFXPlayer.Length - 1].Play();
                return;
            }
        }
        Debug.Log($"PlaySFX {name} 없음");
    }

    // 3-2. 효과음 모두 정지
    public void StopSFX()
    {
        for (int i = 0; i < SFXPlayer.Length; i++)
        {
            SFXPlayer[i].Stop();
        }
    }
}
