using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour
{
    [SerializeField]
    private AudioSource backgroundMusic;
    
    [SerializeField]
    private AudioSource hitSound;
    
    [SerializeField]
    private AudioSource chooseSound;

    [SerializeField] private AudioSource messageSendSound;
    
    [SerializeField]
    private AudioSource pickSound;
    
    [SerializeField]
    private AudioSource shipHitSound;
    
    [SerializeField]
    private AudioSource putWallSound;
    
    [SerializeField]
    private List<AudioSource> weaponsSound;

    private bool _backgroundMusicEnabled = true;

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic.playOnAwake) return;
        
        backgroundMusic.Play();
    }
    public void StopBackgroundMusic()
    {
        backgroundMusic.Stop();
    }
    
    public void DisableBackgroundMusic()
    {
        backgroundMusic.Stop();
        _backgroundMusicEnabled = false;
    }
    public void EnableBackgroundMusic()
    {
        backgroundMusic.Play();
        _backgroundMusicEnabled = false;
    }

    public void PlayHitSound()
    {
        hitSound.Play();
    }

    public void PlayChooseSound()
    {
        chooseSound.Play();
    }

    public void PlayMessageSendSound()
    {
        messageSendSound.Play();
    }

    public void PlayPickSound()
    {
        pickSound.Play();
    }

    public void PlayShipHitSound()
    {
        shipHitSound.Play();
    }

    public void PlayPutWallSound()
    {
        putWallSound.Play();
    }

    public void PlayWeaponSound(int index)
    {
        weaponsSound[index].Play();
    }
    
    public bool IsBackgroundMusicEnabled()
    {
        return _backgroundMusicEnabled;
    }
}
