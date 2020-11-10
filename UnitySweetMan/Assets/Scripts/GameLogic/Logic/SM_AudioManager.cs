using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//声音结构点//
public struct AudioNode
{
    public AudioSource audioSource;          //声音池//
    public int volumeAdd;                    //声音变化，+1则递增,-1则递减//
    public float durationTime;               //渐变时间//
    
    //初始化构造函数//
    public AudioNode(AudioSource source,AudioClip m_clip,float m_initVolume,int m_volumeAdd,float m_durationTime)
    {
        this.audioSource = source;
        this.audioSource.playOnAwake = false;
        this.audioSource.volume = m_initVolume;
        this.audioSource.clip = m_clip;
        this.volumeAdd = m_volumeAdd;
        this.durationTime = m_durationTime;
    }
}

public class SM_AudioManager : Bubble_MonoSingle<SM_AudioManager>
{
    /// <summary>
    /// 背景音乐音乐源
    /// </summary>
    private AudioSource _bgmAudioSource;

    public void Init()
    {
        _bgmAudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 播放BGM
    /// </summary>
    /// <param name="bgm"></param>
    public void PlayBGM(AudioClip bgm)
    {
        AudioNode audioNode=new AudioNode(_bgmAudioSource,bgm,0,1,4);

        StartCoroutine(nameof(AudioSourceVolume),audioNode);
    }
    
    
    //声音渐变迭代器//
    IEnumerator AudioSourceVolume(AudioNode audioNode)
    {
        float initVolume = audioNode.audioSource.volume;
        float preTime = 1.0f/audioNode.durationTime;
        if (!audioNode.audioSource.isPlaying) audioNode.audioSource.Play();
        while(true)
        {
            initVolume += audioNode.volumeAdd * Time.deltaTime *preTime;
            if (initVolume >1 || initVolume <0)
            {
                initVolume = Mathf.Clamp01(initVolume);
                audioNode.audioSource.volume = initVolume;
                if (initVolume ==0) audioNode.audioSource.Stop();
                break;
            }else
            {
                audioNode.audioSource.volume = initVolume;
            }
            yield return 1;
        }
    }
}
