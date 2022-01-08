using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIntroDatabase : MonoBehaviour
{
    public static GameIntroDatabase instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    /* 
    ################################################
    #   MINIGAME AUDIO DATABASE
    ################################################
    */

    [Header("Frogger Game")]
    public AudioClip froggerIntro1;
    public AudioClip froggerIntro2;
    public AudioClip froggerIntro3;
    public AudioClip froggerIntro4;
    public AudioClip froggerIntro5;

    public AudioClip froggerReminder1;
    public AudioClip froggerReminder2;

    public List<AudioClip> froggerEncouragementClips;

    [Header("Pirate Game")]
    public AudioClip pirateIntro1;
    public AudioClip pirateIntro2;
    public AudioClip pirateIntro3;
    public AudioClip pirateIntro4;
    public AudioClip pirateIntro5;

    public AudioClip pirateReminder1;
    public AudioClip pirateReminder2;

    public List<AudioClip> pirateEncouragementClips;

    [Header("Seashell Game")]
    public AudioClip seashellIntro1;
    public AudioClip seashellIntro2;
    public AudioClip seashellIntro3;
    public AudioClip seashellIntro4;
    public AudioClip seashellIntro5;

    public AudioClip seashellReminder1;
    public AudioClip seashellReminder2;

    public List<AudioClip> seashellEncouragementClips;

    [Header("Turntables Game")]
    public AudioClip turntablesIntro1;
    public AudioClip turntablesIntro2;
    public AudioClip turntablesIntro3;
    public AudioClip turntablesIntro4;

    public AudioClip turntablesReminder1;
    public AudioClip turntablesReminder2;
    public AudioClip turntablesReminder3;

    public List<AudioClip> turntablesEncouragementClips;

    [Header("Rummage Game")]
    public AudioClip rummageIntro1;
    public AudioClip rummageIntro2;
    public AudioClip rummageIntro3;
    public AudioClip rummageIntro4;
    public AudioClip rummageIntro5;

    public AudioClip rummageReminder1;
    public AudioClip rummageReminder2;
    public AudioClip rummageReminder3;

    public List<AudioClip> rummageEncouragementClips;

    [Header("Spiderwebs Game")]
    public AudioClip spiderwebsIntro1;
    public AudioClip spiderwebsIntro2;
    public AudioClip spiderwebsIntro3;
    public AudioClip spiderwebsIntro4;
    public AudioClip spiderwebsIntro5;

    public AudioClip spiderwebsReminder1;
    public AudioClip spiderwebsReminder2;

    public List<AudioClip> spiderwebsEncouragementClips;

    /* 
    ################################################
    #   CHALLENGE GAME AUDIO DATABASE
    ################################################
    */

    [Header("Word Factory Blending Game")]
    // tutorial clips
    public AudioClip blendingIntro1;
    public AudioClip blendingIntro2;
    public AudioClip blendingIntro3;
    public AudioClip blendingIntro4;
    public AudioClip blendingIntro5;
    public AudioClip blendingIntro6;
    public AudioClip blendingIntro7;
    public AudioClip blendingIntro8;
    public AudioClip blendingIntro9;
    public AudioClip blendingIntro10;
    public AudioClip blendingIntro11;

    // non-tutorial clips
    public AudioClip blendingStart1;
    public AudioClip blendingStart2;

    public List<AudioClip> blendingReminderClips;

    [Header("Word Factory Substituting Game")]
    // tutorial clips
    public AudioClip substitutingIntro1;
    public AudioClip substitutingIntro2;
    public AudioClip substitutingIntro3;
    public AudioClip substitutingIntro4;
    public AudioClip substitutingIntro5;
    public AudioClip substitutingIntro6;
    public AudioClip substitutingIntro7;
    public AudioClip substitutingIntro8;
    public AudioClip substitutingIntro9;
    public AudioClip substitutingIntro10;
    public AudioClip substitutingIntro11;
    public AudioClip substitutingIntro12;
    public AudioClip substitutingIntro13;

    public List<AudioClip> substitutingReminderClips;

    [Header("Word Factory Deleting Game")]
    // tutorial clips
    public AudioClip deletingIntro1;
    public AudioClip deletingIntro2;
    public AudioClip deletingIntro3;
    public AudioClip deletingIntro4;
    public AudioClip deletingIntro5;

    // reminders chapters 1-4
    public List<AudioClip> deletingReminderClipsChapters1_4;
    // reminders chapters 5
    public List<AudioClip> deletingReminderClipsChapter5;

    [Header("Word Factory Building Game")]
    // tutorial clips
    public AudioClip buildingIntro1;
    public AudioClip buildingIntro2;
    public AudioClip buildingIntro3;
    public AudioClip buildingIntro4;
    public AudioClip buildingIntro5;

    // reminders chapters 1-4
    public List<AudioClip> buildingReminderClipsChapters1_4;
    // reminders chapters 5
    public List<AudioClip> buildingReminderClipsChapter5;

    [Header("Tiger Paw Coin - Find the Sound")]
    // tutorial clips
    public AudioClip tigerPawCoinIntro1;
    public AudioClip tigerPawCoinIntro2;
    public AudioClip tigerPawCoinIntro3;
    public AudioClip tigerPawCoinIntro4;
    public AudioClip tigerPawCoinIntro5;
    public AudioClip tigerPawCoinIntro6;
    public AudioClip tigerPawCoinIntro7;

    public AudioClip tigerPawCoinStart;

    // new photo
    public List<AudioClip> tigerPawCoinNewPhotosChapters1_4;
    public List<AudioClip> tigerPawCoinNewPhotosChapter5;

    // julius wins
    public List<AudioClip> tigerPawCoinJuliusWinChapters1_4;
    public List<AudioClip> tigerPawCoinJuliusWinChapter5;

    // julius lose
    public List<AudioClip> tigerPawCoinJuliusLoseChapters1_4;
    public List<AudioClip> tigerPawCoinJuliusLoseChapter5;

    // final julius win
    public AudioClip tigerPawCoinFinalJuliusWinChapters1_4; 
    public AudioClip tigerPawCoinFinalWinChapter5;

    // final julius lose
    public AudioClip tigerPawCoinFinalJuliusLoseChapters1_4; 
    public AudioClip tigerPawCoinFinalLoseChapter5;

    [Header("Tiger Paw Photos - Find the Word")]
    // tutorial clips
    public AudioClip tigerPawPhotosIntro1;
    public AudioClip tigerPawPhotosIntro2;
    public AudioClip tigerPawPhotosIntro3;
    public AudioClip tigerPawPhotosIntro4;
    public AudioClip tigerPawPhotosIntro5;
    public AudioClip tigerPawPhotosIntro6;
    public AudioClip tigerPawPhotosIntro7;

    [Header("Password")]
    // tutorial clips
    public AudioClip passwordIntro1;
    public AudioClip passwordIntro2;
    public AudioClip passwordIntro3;
    public AudioClip passwordIntro4;
    public AudioClip passwordIntro5;
    public AudioClip passwordIntro6;
    public AudioClip passwordIntro7;
    public AudioClip passwordIntro8;
    public AudioClip passwordIntro9;
    public AudioClip passwordIntro10;
    public AudioClip passwordIntro11;
    public AudioClip passwordIntro12;

    // start clips
    public AudioClip passwordStart1;
    public AudioClip passwordStart2;
    public AudioClip passwordStart3;

    // new photo
    public List<AudioClip> passwordNewPhoto;
}
