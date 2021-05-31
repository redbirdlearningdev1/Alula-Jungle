using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Song
{
    JungleGameTestSong,
    COUNT
}

public class AudioDatabase : MonoBehaviour
{
    /* 
    ################################################
    #   MUSIC DATABASE
    ################################################
    */

    [Header("Music Database")]
    [SerializeField] private AudioClip JungleGameTestSong;

    public AudioClip GetSongFromEnum(Song song)
    {
        switch (song)
        {
            default:
            case Song.JungleGameTestSong:
                return JungleGameTestSong;
        }
    }
}