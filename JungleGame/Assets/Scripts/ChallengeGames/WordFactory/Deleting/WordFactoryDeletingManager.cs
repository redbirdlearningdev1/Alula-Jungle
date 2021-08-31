using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordFactoryDeletingManager : MonoBehaviour
{
    public static WordFactoryDeletingManager instance;

    public Polaroid polaroid; // main polarid used in this game

    private List<ChallengeWord> globalWordList;
    private List<ChallengeWord> unusedWordList;
    private List<ChallengeWord> usedWordList;

    private ChallengeWord currentWord;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        PregameSetup();
    }

    private void PregameSetup()
    {
        // set emerald head to be closed
        EmeraldHead.instance.animator.Play("PolaroidEatten");

        // set winner cards to be inactive
        WinCardsController.instance.ResetCards();

        // set tiger cards to be inactive
        TigerController.instance.ResetCards();

        // get global challenge word list
        ChallengeWordDatabase.InitCreateGlobalList(true);
        globalWordList = ChallengeWordDatabase.globalChallengeWordList;
        // unused list init
        usedWordList = new List<ChallengeWord>();
        // used list init
        unusedWordList = new List<ChallengeWord>();
        unusedWordList.AddRange(globalWordList);



        // start game
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        // init game delay
        yield return new WaitForSeconds(0.5f);

        // open emerald head
        EmeraldHead.instance.animator.Play("OpenMouth");
        yield return new WaitForSeconds(1.5f);

        // choose challenge word + play enter animation
        currentWord = GetNewChallengeWord();
        polaroid.SetPolaroid(currentWord);
        yield return new WaitForSeconds(1f);

        TigerController.instance.tigerAnim.Play("TigerSwipe");
        EmeraldHead.instance.animator.Play("EnterPolaroid");
    }

    private ChallengeWord GetNewChallengeWord()
    {
        // restock unused list
        if (unusedWordList.Count <= 0)
        {
            unusedWordList = new List<ChallengeWord>();
            unusedWordList.AddRange(globalWordList);

            usedWordList.Clear();
        }   
        
        // get random word
        int index = Random.Range(0, unusedWordList.Count);
        ChallengeWord word = unusedWordList[index];

        // update lists
        usedWordList.Add(word);
        unusedWordList.Remove(word);

        return word;
    }
}
