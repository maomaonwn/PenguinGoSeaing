using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : Singleton<AudioManager>
{
    public enum SoundEffectName
    {
        Player_Attack,
        Player_Dead,
        Player_Down,
        Player_Fall,
        Player_Hit,
        Player_Jump,
        Player_Run,
        Player_Rush,
        Player_Meditation
    }

    public enum BgmName
    {
        mainTheme,
        backGround,
        bossIntro,
        bossLoop,
        bossOutro,
        fightTheme
    }

    [Serializable]
    public struct SoundEffectItem
    {
        public SoundEffectName audioName;
        public AudioClip audioClip;
    }

    [Serializable]
    public struct BgmItem
    {
        public BgmName audioName;
        public AudioClip audioClip;
    }

    // �洢��Ϸ���õ���������Ч
    public SoundEffectItem[] soundEffectItems;
    // �洢��Ϸ���õ������б�����
    public BgmItem[] bgmItems;

    private Dictionary<SoundEffectName, AudioClip> soundEffectDict;
    private Dictionary<BgmName, AudioClip> bgmDict;

    // ���屳��BGM
    public AudioSource currentBGM;
    public AudioSource currentEffectMusic;

    void Start()
    {
        if (currentBGM == null || currentEffectMusic == null)
        {
            Debug.LogError("AudioSource lost");
        }
        soundEffectDict = new Dictionary<SoundEffectName, AudioClip>();
        bgmDict = new Dictionary<BgmName, AudioClip>();
        for (int i = 0; i < soundEffectItems.Length; i++)
        {
            soundEffectDict.Add(soundEffectItems[i].audioName, soundEffectItems[i].audioClip);
        }
        for (int i = 0; i < bgmItems.Length; i++)
        {
            bgmDict.Add(bgmItems[i].audioName, bgmItems[i].audioClip);
        }
        AudioClip newClip = bgmDict[BgmName.mainTheme];
        currentBGM.clip = newClip;
        currentBGM.loop = true;
        currentBGM.Play();
        //Debug.Log(soundEffectDict);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SwitchBgm(BgmName.fightTheme);
        }
    }

    public void PlaySoundEffect(SoundEffectName audioName, bool isLoop = false)
    {
        // ��ȡҪ���ŵ���Դ
        AudioClip audioClip = soundEffectDict[audioName];
        // �л���Ч
        currentEffectMusic.clip = audioClip;
        // ÿ�β���ǰ����������Ϊ���ֵ��������ÿ������������΢��Щ��ͬ
        currentEffectMusic.pitch = Random.Range(0.9f, 1.1f);
        currentEffectMusic.loop = isLoop;
        currentEffectMusic.Play();
    }

    public void SwitchBgm(BgmName bgmName)
    {
        AudioClip newClip = bgmDict[bgmName];
        if (currentBGM.clip == null || currentBGM.clip != newClip)
        {
            //currentBGM.clip = newClip;
            StartCoroutine(SwitchBgmWithFade(newClip, 1.0f));
        }
    }

    IEnumerator SwitchBgmWithFade(AudioClip newClip, float fadeDuration)
    {
        float timer = 0.0f;
        float startVolume = currentBGM.volume;

        while (timer < fadeDuration)
        {
            currentBGM.volume = Mathf.Lerp(startVolume, 0, timer / fadeDuration); // ���������ӵ�ǰ������0
            timer += Time.deltaTime;
            yield return null;
        }

        currentBGM.Stop();
        currentBGM.clip = newClip;
        currentBGM.Play();

        timer = 0.0f;
        while (timer < fadeDuration)
        {
            currentBGM.volume = Mathf.Lerp(0, startVolume, timer / fadeDuration); // ����������0��Ŀ������
            timer += Time.deltaTime;
            yield return null;
        }

        currentBGM.volume = startVolume; // ȷ���������ջָ���Ŀ������
    }

    public void PauseBGM()
    {
        currentBGM.Pause();
    }

    public void ResumeBGM()
    {
        currentBGM.Play();
    }
}
