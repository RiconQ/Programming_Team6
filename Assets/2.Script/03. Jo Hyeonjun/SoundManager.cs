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
    // 0. �̱��� ����
    public static SoundManager instance = null;
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
        BGMPlayer = transform.GetChild(0).GetComponent<AudioSource>();
        SFXPlayer = transform.GetChild(1).GetComponents<AudioSource>();
    }

    // 1. ���� ����
    [Header("Audio Clip")]
    [SerializeField] private Sound[] BGM;
    [SerializeField] private Sound[] SFX;
    [Header("AudioSource")]
    [SerializeField] private AudioSource BGMPlayer;
    [SerializeField] private AudioSource[] SFXPlayer;

    // 2-1. BGM ��� 
    // BGM�� ���� ������ ����Ͽ� ����������.
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
        Debug.Log($"PlayBGM {name} ����");
    }

    // 2-2. BGM ����
    public void StopBGM() { BGMPlayer.Stop(); }

    // 3-1. ȿ���� ���
    public void PlaySFX(string name)
    {
        foreach (Sound s in SFX)
        {
            if (s.name.Equals(name))
            {
                // 0���� ��������� ���� SFXPlayer�� Ž���Ͽ� ���
                for (int i = 0; i < SFXPlayer.Length; i++)
                {
                    if (!SFXPlayer[i].isPlaying)
                    {
                        SFXPlayer[i].clip = s.clip;
                        SFXPlayer[i].Play();
                        return;
                    }
                }

                // ��� ������̶��, �ϳ� �߰��ϰ� �ű⿡ ���
                transform.GetChild(1).gameObject.AddComponent<AudioSource>();
                SFXPlayer[SFXPlayer.Length - 1].clip = s.clip;
                SFXPlayer[SFXPlayer.Length - 1].Play();
                return;
            }
        }
        Debug.Log($"PlaySFX {name} ����");
    }

    // 3-2. ȿ���� ��� ����
    public void StopSFX()
    {
        for (int i = 0; i < SFXPlayer.Length; i++)
        {
            SFXPlayer[i].Stop();
        }
    }
}
