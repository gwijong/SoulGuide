using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager:MonoBehaviour
{
    public AudioSource BgmPlayer;
    public AudioSource SfxPlayer;

    public float bgm;
    public float sfx;

    [SerializeField]
    AudioClip Button;
    [SerializeField]
    AudioClip Buy;
    [SerializeField]
    AudioClip Clear;
    [SerializeField]
    AudioClip Fail;
    [SerializeField]
    AudioClip Grow;
    [SerializeField]
    AudioClip PauseIn;
    [SerializeField]
    AudioClip PauseOut;
    [SerializeField]
    AudioClip Sell;
    [SerializeField]
    AudioClip Touch;
    [SerializeField]
    AudioClip Unlock;
    void Start()
    {
        GameManager.update.UpdateMethod -= OnUpdate;
        GameManager.update.UpdateMethod += OnUpdate;
    }
    public void PlaySfxPlayer(string audioClipName)
    {
        switch (audioClipName)
        {
            case "Button":
                SfxPlayer.clip = Button;
                break;
            case "Buy":
                SfxPlayer.clip = Buy;             
                break;
            case "Clear":
                SfxPlayer.clip = Clear;
                break;
            case "Fail":
                SfxPlayer.clip = Fail;
                break;
            case "Grow":
                SfxPlayer.clip = Grow;
                break;
            case "PauseIn":
                SfxPlayer.clip = PauseIn;
                break;
            case "PauseOut":
                SfxPlayer.clip = PauseOut;
                break;
            case "Sell":
                SfxPlayer.clip = Sell;
                break;
            case "Touch":
                SfxPlayer.clip = Touch;             
                break;
            case "Unlock":
                SfxPlayer.clip = Unlock;
                break;
        }
        SfxPlayer.Play();
    }

    void OnUpdate()
    {
        BgmPlayer.volume = bgm;
        SfxPlayer.volume = sfx;
    }
}
