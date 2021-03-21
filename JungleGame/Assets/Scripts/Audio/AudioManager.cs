using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static bool useMic = false;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource fxSource;
    [SerializeField] private AudioSource talkSource;

    [SerializeField] private MusicDatabase musicDatabase;

    /* 
    ################################################
    #   MUSIC SOURCE
    ################################################
    */

    public void PlaySong(Song song)
    {
        musicSource.Stop();
        AudioClip clip = musicDatabase.GetSongFromEnum(song);

        if (clip == musicSource.clip)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;
    }

    /* 
    ################################################
    #   FX SOURCE
    ################################################
    */

    /* 
    ################################################
    #   TALK SOURCE
    ################################################
    */

    public void PlayTalk(AudioClip _clip)
    {
        talkSource.Stop();
        talkSource.clip = _clip;
        talkSource.loop = false;
        talkSource.Play();
    }
}

public static class AudioHelper
{
    private static AudioManager am;

    /* 
    ################################################
    #   MUSIC SOURCE
    ################################################
    */

    public static void PlaySong(Song song)
    {
        FindAudioManager();
        am.PlaySong(song);
    }

    public static void StopMusic()
    {
        FindAudioManager();
        am.StopMusic();
    }

    /* 
    ################################################
    #   FX SOURCE
    ################################################
    */

    /* 
    ################################################
    #   TALK SOURCE
    ################################################
    */

    public static void PlayTalk(AudioClip clip)
    {
        FindAudioManager();
        am.PlayTalk(clip);
    }

    /* 
    ################################################
    #   FIND AUDIO MANAGER
    ################################################
    */

    private static void FindAudioManager()
    {
        GameObject audioManagerObject;

        if (am == null)
        {
            audioManagerObject = GameObject.Find("AudioManager");

            if (!audioManagerObject)
            {
                GameManager.instance.SendError(new Object(), "AudioHelper could not find AudioManager object");
                return;
            }

            am = audioManagerObject.GetComponent<AudioManager>();
        }
    }
}
