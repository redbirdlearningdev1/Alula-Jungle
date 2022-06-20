using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
    public AssetReference froggerIntro1;
    public AssetReference froggerIntro2;
    public AssetReference froggerIntro3;
    public AssetReference froggerIntro4;
    public AssetReference froggerIntro5;

    public AssetReference froggerReminder1;
    public AssetReference froggerReminder2;

    public List<AssetReference> froggerEncouragementClips;

    [Header("Pirate Game")]
    public AssetReference pirateIntro1;
    public AssetReference pirateIntro2;
    public AssetReference pirateIntro3;
    public AssetReference pirateIntro4;
    public AssetReference pirateIntro5;

    public AssetReference pirateReminder1;
    public AssetReference pirateReminder2;

    public List<AssetReference> pirateEncouragementClips;

    [Header("Seashell Game")]
    public AssetReference seashellIntro1;
    public AssetReference seashellIntro2;
    public AssetReference seashellIntro3;
    public AssetReference seashellIntro4;
    public AssetReference seashellIntro5;

    public AssetReference seashellReminder1;
    public AssetReference seashellReminder2;

    public List<AssetReference> seashellEncouragementClips;

    [Header("Turntables Game")]
    public AssetReference turntablesIntro1;
    public AssetReference turntablesIntro2;
    public AssetReference turntablesIntro3;
    public AssetReference turntablesIntro4;

    public AssetReference turntablesReminder1;
    public AssetReference turntablesReminder2;
    public AssetReference turntablesReminder3;

    public List<AssetReference> turntablesEncouragementClips;

    [Header("Rummage Game")]
    public AssetReference rummageIntro1;
    public AssetReference rummageIntro2;
    public AssetReference rummageIntro3;
    public AssetReference rummageIntro4;
    public AssetReference rummageIntro5;

    public AssetReference rummageReminder1;
    public AssetReference rummageReminder2;
    public AssetReference rummageReminder3;

    public List<AssetReference> rummageEncouragementClips;

    [Header("Spiderwebs Game")]
    public AssetReference spiderwebsIntro1;
    public AssetReference spiderwebsIntro2;
    public AssetReference spiderwebsIntro3;
    public AssetReference spiderwebsIntro4;
    public AssetReference spiderwebsIntro5;

    public AssetReference spiderwebsReminder1;
    public AssetReference spiderwebsReminder2;

    public AssetReference spiderwebsOmNomNom;

    public List<AssetReference> spiderwebsEncouragementClips;

    /* 
    ################################################
    #   CHALLENGE GAME AUDIO DATABASE
    ################################################
    */

    [Header("Word Factory Blending Game")]
    // tutorial clips
    public AssetReference blendingIntro1;
    public AssetReference blendingIntro2;
    public AssetReference blendingIntro3;
    public AssetReference blendingIntro4;
    public AssetReference blendingIntro5;
    public AssetReference blendingIntro6;
    public AssetReference blendingIntro7;
    public AssetReference blendingIntro8;
    public AssetReference blendingIntro9;
    public AssetReference blendingIntro10;
    public AssetReference blendingIntro11;

    // non-tutorial clips
    public AssetReference blendingStart1;
    public AssetReference blendingStart2;

    // non-tutorial clips
    public AssetReference blendingEnd1;
    public AssetReference blendingEnd2;

    public List<AssetReference> blendingReminderClips;

    [Header("Word Factory Substituting Game")]
    // tutorial clips
    public AssetReference substitutingIntro1;
    public AssetReference substitutingIntro2;
    public AssetReference substitutingIntro3;
    public AssetReference substitutingIntro4;
    public AssetReference substitutingIntro5;
    public AssetReference substitutingIntro6;
    public AssetReference substitutingIntro7;
    public AssetReference substitutingIntro8;
    public AssetReference substitutingIntro9;
    public AssetReference substitutingIntro10;
    public AssetReference substitutingIntro11;
    public AssetReference substitutingIntro12;
    public AssetReference substitutingIntro13;

    // non-tutorial clips
    public AssetReference substitutingStart1;
    public AssetReference substitutingStart2;

    public List<AssetReference> substitutingReminderClips;

    [Header("Word Factory Deleting Game")]
    // tutorial clips
    public AssetReference deletingIntro1;
    public AssetReference deletingIntro2;
    public AssetReference deletingIntro3;
    public AssetReference deletingIntro4;
    public AssetReference deletingIntro5;

    public AssetReference deletingStart1;
    public AssetReference deletingStart2Chapters1_3;
    public AssetReference deletingStart2Chapters4_5;

    // reminders chapters 1-4
    public List<AssetReference> deletingReminderClipsChapters1_4;
    // reminders chapters 5
    public List<AssetReference> deletingReminderClipsChapter5;

    [Header("Word Factory Building Game")]
    // tutorial clips
    public AssetReference buildingIntro1;
    public AssetReference buildingIntro2;
    public AssetReference buildingIntro3;
    public AssetReference buildingIntro4;
    public AssetReference buildingIntro5;

    public AssetReference buildingStart1;
    public AssetReference buildingStart2Chapters1_3;
    public AssetReference buildingStart2Chapters4_5;

    // reminders chapters 1-4
    public List<AssetReference> buildingReminderClipsChapters1_4;
    // reminders chapters 5
    public List<AssetReference> buildingReminderClipsChapter5;

    [Header("Tiger Paw Coin - Find the Sound")]
    // tutorial clips
    public AssetReference tigerPawCoinIntro1;
    public AssetReference tigerPawCoinIntro2;
    public AssetReference tigerPawCoinIntro3;
    public AssetReference tigerPawCoinIntro4;
    public AssetReference tigerPawCoinIntro5;
    public AssetReference tigerPawCoinIntro6;
    public AssetReference tigerPawCoinIntro7;

    public AssetReference tigerPawCoinStart;

    // new photo
    public List<AssetReference> tigerPawCoinNewPhotosChapters1_4;
    public List<AssetReference> tigerPawCoinNewPhotosChapter5;

    // julius wins
    public List<AssetReference> tigerPawCoinJuliusWinChapters1_4;
    public List<AssetReference> tigerPawCoinJuliusWinChapter5;

    // julius lose
    public List<AssetReference> tigerPawCoinJuliusLoseChapters1_4;
    public List<AssetReference> tigerPawCoinJuliusLoseChapter5;

    // final julius win
    public AssetReference tigerPawCoinFinalJuliusWinChapters1_4; 
    public AssetReference tigerPawCoinFinalWinChapter5;

    // final julius lose
    public AssetReference tigerPawCoinFinalJuliusLoseChapters1_4; 
    public AssetReference tigerPawCoinFinalLoseChapter5;

    [Header("Tiger Paw Photos - Find the Word")]
    // tutorial clips
    public AssetReference tigerPawPhotosIntro1;
    public AssetReference tigerPawPhotosIntro2;
    public AssetReference tigerPawPhotosIntro3;
    public AssetReference tigerPawPhotosIntro4;
    public AssetReference tigerPawPhotosIntro5;
    public AssetReference tigerPawPhotosIntro6;
    public AssetReference tigerPawPhotosIntro7;
    public AssetReference tigerPawPhotosIntro8;

    public AssetReference tigerPawPhotosStart;

    [Header("Password")]
    // tutorial clips
    public AssetReference passwordIntro1;
    public AssetReference passwordIntro2;
    public AssetReference passwordIntro3;
    public AssetReference passwordIntro4;
    public AssetReference passwordIntro5;
    public AssetReference passwordIntro6;
    public AssetReference passwordIntro7;
    public AssetReference passwordIntro8;
    public AssetReference passwordIntro9;
    public AssetReference passwordIntro10;
    public AssetReference passwordIntro11;
    public AssetReference passwordIntro12;

    // start clips
    public AssetReference passwordStart1;
    public AssetReference passwordStart2;
    public AssetReference passwordStart3;

    // new photo
    public List<AssetReference> passwordNewPhoto;
}
