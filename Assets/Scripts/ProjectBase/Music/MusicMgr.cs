using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicMgr : BaseManager<MusicMgr>
{
    // Resources 路径（不含扩展名），与 PoolMgr 池键一致
    private const string SoundPoolPath = "Music/SoundPlayer";

    // 唯一的背景音乐对象
    private AudioSource bkMusic = null;
    // 背景音乐音量
    private float bkValue = 1;

    // 当前正在播放的音效（用于音量与播完回收）
    private List<AudioSource> soundList = new List<AudioSource>();
    // 音效音量
    private float soundValue = 1;

    public MusicMgr()
    {
        MonoMgr.GetInstance().AddUpdateListener(Update);
    }

    private void Update()
    {
        for (int i = soundList.Count - 1; i >= 0; i--)
        {
            AudioSource s = soundList[i];
            if (s == null)
            {
                soundList.RemoveAt(i);
                continue;
            }
            if (!s.isPlaying && !s.loop)
            {
                RecycleSound(s);
            }
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBKMusic(string name)
    {
        if(bkMusic == null)
        {
            GameObject obj = new GameObject("BKMusic");
            bkMusic = obj.AddComponent<AudioSource>();
        }
        // 异步加载资源 加载完成过后 播放
        ResourcesMgr.GetInstance().LoadAsync<AudioClip>("Music/BK/" + name, (audioClip) =>
        {
            bkMusic.clip = audioClip;
            bkMusic.loop = true;
            bkMusic.volume = bkValue;
            bkMusic.Play();
        });
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBKMusic()
    {
        if (bkMusic == null) return;
        bkMusic.Pause();
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBKMusic()
    {
        if (bkMusic == null) return;
        bkMusic.Stop();
    }

    /// <summary>
    /// 改变背景音乐音量
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBKValue(float v)
    {
        bkValue = v;
        if (bkMusic == null) return;
        bkMusic.volume = bkValue;
    }

    /// <summary>
    /// 暂停所有当前正在播放的音效（不归还对象池）
    /// </summary>
    public void PauseAllSounds()
    {
        for (int i = 0; i < soundList.Count; i++)
        {
            if (soundList[i] != null)
                soundList[i].Pause();
        }
    }

    /// <summary>
    /// 恢复所有已暂停的音效
    /// </summary>
    public void ResumeAllSounds()
    {
        for (int i = 0; i < soundList.Count; i++)
        {
            if (soundList[i] != null)
                soundList[i].UnPause();
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name"></param>
    public void PlaySound(string name, bool isLoop, UnityAction<AudioSource> callBack = null)
    {
        PoolMgr.GetInstance().GetObj(SoundPoolPath, (go) =>
        {
            AudioSource source = go.GetComponent<AudioSource>();
            if (source == null)
            {
                source = go.AddComponent<AudioSource>();
            }

            ResourcesMgr.GetInstance().LoadAsync<AudioClip>("Music/Sound/" + name, (audioClip) =>
            {
                if (audioClip == null)
                {
                    RecycleSound(source);
                    return;
                }

                source.clip = audioClip;
                source.loop = isLoop;
                source.volume = soundValue;
                source.Play();
                soundList.Add(source);

                if (callBack != null)
                {
                    callBack.Invoke(source);
                }
            });
        });
    }

    /// <summary>
    /// 停止音效并归还对象池
    /// </summary>
    public void StopSound(AudioSource source)
    {
        if (source == null) return;
        RecycleSound(source);
    }

    /// <summary>
    /// 回收音效
    /// </summary>
    /// <param name="source"></param>
    private void RecycleSound(AudioSource source)
    {
        if (source == null) return;
        
        int idx = soundList.IndexOf(source);
        if (idx >= 0)
        {
            soundList.RemoveAt(idx);
        }

        source.Stop();
        source.clip = null;
        PoolMgr.GetInstance().PushObj(SoundPoolPath, source.gameObject);
    }

    /// <summary>
    /// 改变音效音量
    /// </summary>
    /// <param name="v"></param>
    public void ChangeSoundValue(float v)
    {
        soundValue = v;

        for (int i = 0; i < soundList.Count; i++)
        {
            if (soundList[i] != null)
            {
                soundList[i].volume = soundValue;
            }
        }
    }
}
