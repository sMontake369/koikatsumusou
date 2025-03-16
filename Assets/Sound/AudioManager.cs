using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource BGMSource;
    List<AudioSource> SESourceList;
    List<GameObject> SEObjectList;

    public AudioClip titleGBM;
    public AudioClip inGameBGM;
    public AudioClip endingBGM;
    public AudioClip sendMessage;
    public AudioClip receiveMessage;
    public AudioClip notification;
    public AudioClip clicked;
    public AudioClip typing;
    public AudioClip unSelect;
    public AudioClip select;
    public AudioClip soliloquy;
    public AudioClip getDeck;
    public AudioClip dropDeck;
    public AudioClip clickMessage;

    float BGMVolume = 0.5f;
    float SEVolume = 0.5f;

    int audioIndex = 0;
    // Start is called before the first frame update
    public void Init()
    {
        if (BGMSource == null) BGMSource = this.AddComponent<AudioSource>();
        else 
        {
            BGMSource.Stop();
            foreach (AudioSource audioSource in SESourceList) audioSource.Stop();
            foreach (GameObject obj in SEObjectList) Destroy(obj);
        }

        BGMSource.volume = BGMVolume;
        BGMSource.loop = true;

        SESourceList = new List<AudioSource>();
        SEObjectList = new List<GameObject>();
        for(int i = 0; i < 20; i++)
        {
            GameObject obj = new GameObject("AudioSource" + i);
            obj.transform.SetParent(this.transform);
            AudioSource audioSource = obj.AddComponent<AudioSource>();
            audioSource.volume = SEVolume;
            audioSource.playOnAwake = false;
            SEObjectList.Add(obj);
            SESourceList.Add(audioSource);
        }
    }

    public float GetBGMVolume()
    {
        return BGMVolume;
    }

    public float GetSEVolume()
    {
        return SEVolume;
    }

    public void SetBGMVolume(float volume)
    {
        BGMVolume = volume / 100;
        BGMSource.DOKill();
        BGMSource.volume = BGMVolume;
    }

    public void SetSEVolume(float volume)
    {
        SEVolume = volume / 100;
        foreach (AudioSource audioSource in SESourceList) audioSource.volume = SEVolume;
    }

    public void SetBGM(EBGM eBGM)
    {
        BGMSource.DOFade(endValue: 0, duration: 0.5f).WaitForCompletion();
        BGMSource.Stop();
        switch (eBGM)
        {
            case EBGM.Title:
                BGMSource.clip = titleGBM;
                break;
            case EBGM.Game:
                BGMSource.clip = inGameBGM;
                break;
            case EBGM.Ending:
                BGMSource.clip = endingBGM;
                break;
        }
        BGMSource.volume = math.min(BGMVolume, 0.05f);
        BGMSource.Play();
        BGMSource.DOFade(endValue: BGMVolume, duration: 3f);
    }

    public void PlaySound(AudioClip clip, float volume = 1)
    {
        if(clip == null) return;
        SESourceList[audioIndex].Stop();
        SESourceList[audioIndex].PlayOneShot(clip, math.min(SEVolume, volume));
        audioIndex = ++audioIndex % 20;
    }

    public void PlayNormalSound(NormalSound normalSound)
    {
        AudioClip clip = null;
        switch (normalSound)
        {
            case NormalSound.sendMessage:
                clip = sendMessage;
                break;
            case NormalSound.receiveMessage:
                clip = receiveMessage;
                break;
            case NormalSound.notification:
                clip = notification;
                break;
            case NormalSound.clicked:
                clip = clicked;
                break;
            case NormalSound.select:
                clip = select;
                break;
            case NormalSound.soliloquy:
                clip = soliloquy;
                break;
            case NormalSound.getDeck:
                clip = getDeck;
                break;
            case NormalSound.dropDeck:
                clip = dropDeck;
                break;
            case NormalSound.typing:
                clip = typing;
                break;
            case NormalSound.unSelect:
                clip = unSelect;
                break;
            case NormalSound.clickMessage:
                clip = clickMessage;
                break;
        }
        if (clip != null) PlaySound(clip);
    }
}

public enum NormalSound
{
    sendMessage,
    receiveMessage,
    notification,
    clicked,
    select,
    unselect,
    soliloquy,
    getDeck,
    dropDeck,
    typing,
    unSelect,
    clickMessage
}

public enum EBGM
{
    Title,
    Game,
    Ending
}