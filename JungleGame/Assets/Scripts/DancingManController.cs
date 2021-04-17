using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DancingManController : MonoBehaviour
{
    [SerializeField] private TextureAtlas textureAtlas;
    [SerializeField] private Animator animator;

    public bool raycast;
    [HideInInspector] public bool isClicked;

    void Update() 
    {
        if (!raycast)
            return;

        if (Input.GetMouseButtonUp(0))
        {
            isClicked = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                print("clicked on: " + hit.collider.gameObject.name);

                if (hit.collider.gameObject.transform.CompareTag("DancingMan"))
                {
                    isClicked = true;
                }
            }
        }
    }


    public void PlayUsingPhonemeEnum(Phoneme phoneme)
    {
        switch (phoneme)
        {
            default:
            case Phoneme.a_Phoneme:
                PlayBaby();
                break;
            case Phoneme.aPhoneme:
                PlayBackpack();
                break;
            case Phoneme.airPhoneme:
                PlayBumphead();
                break;
            case Phoneme.arePhoneme:
                PlayChoice();
                break;
            case Phoneme.awPhoneme:
                PlayExplorer();
                break;
            case Phoneme.e_Phoneme:
                PlayFrustrating();
                break;
            case Phoneme.earPhoneme:
                PlayGive();
                break;
            case Phoneme.ePhoneme:
                PlayGorilla();
                break;
            case Phoneme.erPhoneme:
                PlayHello();
                break;
            case Phoneme.i_Phoneme:
                PlayListen();
                break;
            case Phoneme.iPhoneme:
                PlayMudslide();
                break;
            case Phoneme.o_Phoneme:
                PlayMudslide2();
                break;
            case Phoneme.oPhoneme:
                PlayOrc();
                break;
            case Phoneme.oiPhoneme:
                PlayPirate();
                break;
            case Phoneme.ooPhoneme:
                PlayPoop();
                break;
            case Phoneme.orPhoneme:
                PlayScared();
                break;
            case Phoneme.owPhoneme:
                PlaySounds();
                break;
            case Phoneme.u_Phoneme:
                PlaySpider();
                break;
            case Phoneme.uPhoneme:
                PlaySpider2();
                break;
            case Phoneme.yerPhoneme:
                PlayStrongwind();
                break;
        }
    }

    public void PlayBaby()
    {
        animator.Play("baby");
        textureAtlas.BabyFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayBackpack()
    {
        animator.Play("backpack");
        textureAtlas.BackpackFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayBumphead()
    {
        animator.Play("bumphead");
        textureAtlas.BumpheadFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayChoice()
    {
        animator.Play("choice");
        textureAtlas.ChoiceFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayExplorer()
    {
        animator.Play("explorer");
        textureAtlas.ExplorerFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayFrustrating()
    {
        animator.Play("choice");
        textureAtlas.ChoiceFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayGive()
    {
        animator.Play("give");
        textureAtlas.GiveFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayGorilla()
    {
        animator.Play("gorilla");
        textureAtlas.GorillaFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayHello()
    {
        animator.Play("hello");
        textureAtlas.HelloFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayListen()
    {
        animator.Play("listen");
        textureAtlas.ListenFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayMudslide()
    {
        animator.Play("mudslide");
        textureAtlas.MudslideFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayMudslide2()
    {
        animator.Play("mudslide");
        textureAtlas.Mudslide2Face();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayOrc()
    {
        animator.Play("orc");
        textureAtlas.OrcsFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayPirate()
    {
        animator.Play("pirate");
        textureAtlas.PirateFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayPoop()
    {
        animator.Play("poop");
        textureAtlas.PoopFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayScared()
    {
        animator.Play("scared");
        textureAtlas.ScaredFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlaySounds()
    {
        animator.Play("sounds");
        textureAtlas.SoundsFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlaySpider()
    {
        animator.Play("spider");
        textureAtlas.SpiderFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlaySpider2()
    {
        animator.Play("spider");
        textureAtlas.Spider2Face();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayStrongwind()
    {
        animator.Play("strongwind");
        textureAtlas.StrongwindFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayThatGuy()
    {
        animator.Play("thatguy");
        textureAtlas.ThatGuyFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayThink()
    {
        animator.Play("think");
        textureAtlas.ThinkFace();
        StartCoroutine(TextureSwapRoutine());
    }


    private IEnumerator TextureSwapRoutine(float time = 1.5f)
    {
        yield return new WaitForSeconds(time);
        textureAtlas.ReturnToBase();
    }
}
