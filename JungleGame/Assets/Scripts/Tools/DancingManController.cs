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

        isClicked = false;

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


    public void PlayUsingPhonemeEnum(ActionWordEnum word)
    {
        switch (word)
        {
            default:
            case ActionWordEnum.mudslide:
                PlayMudslide();
                break;
            case ActionWordEnum.listen:
                PlayListen();
                break;
            case ActionWordEnum.poop:
                PlayPoop();
                break;
            case ActionWordEnum.orcs:
                PlayOrcs();
                break;
            case ActionWordEnum.think:
                PlayThink();
                break;

            case ActionWordEnum.hello:
                PlayHello();
                break;
            case ActionWordEnum.spider:
                PlaySpider();
                break;
            case ActionWordEnum.explorer:
                PlayExplorer();
                break;
            case ActionWordEnum.scared:
                PlayScared();
                break;
            case ActionWordEnum.thatguy:
                PlayThatGuy();
                break;

            case ActionWordEnum.choice:
                PlayChoice();
                break;
            case ActionWordEnum.strongwind:
                PlayStrongwind();
                break;
            case ActionWordEnum.pirate:
                PlayPirate();
                break;
            case ActionWordEnum.gorilla:
                PlayGorilla();
                break;
            case ActionWordEnum.sounds:
                PlaySounds();
                break;
            case ActionWordEnum.give:
                PlayGive();
                break;

            case ActionWordEnum.backpack:
                PlayBackpack();
                break;
            case ActionWordEnum.frustrating:
                PlayFrustrating();
                break;
            case ActionWordEnum.bumphead:
                PlayBumphead();
                break;
            case ActionWordEnum.baby:
                PlayBaby();
                break;
        }
    }

    public void PlayBaby()
    {
        animator.Play("baby");
        textureAtlas.babyFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayBackpack()
    {
        animator.Play("backpack");
        textureAtlas.backpackFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayBumphead()
    {
        animator.Play("bumphead");
        textureAtlas.bumpheadFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayChoice()
    {
        animator.Play("choice");
        textureAtlas.choiceFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayExplorer()
    {
        animator.Play("explorer");
        textureAtlas.explorerFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayFrustrating()
    {
        animator.Play("frustrating");

        textureAtlas.choiceFace();

        textureAtlas.frustratingFace();

        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayGive()
    {
        animator.Play("give");
        textureAtlas.giveFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayGorilla()
    {
        animator.Play("gorilla");
        textureAtlas.gorillaFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayHello()
    {
        animator.Play("hello");
        textureAtlas.helloFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayListen()
    {
        animator.Play("listen");
        textureAtlas.listenFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayMudslide()
    {
        animator.Play("mudslide");
        textureAtlas.mudslideFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayMudslide2()
    {
        animator.Play("mudslide");
        textureAtlas.mudslide2Face();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayOrcs()
    {
        animator.Play("orcs");
        textureAtlas.orcsFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayPirate()
    {
        animator.Play("pirate");
        textureAtlas.pirateFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayPoop()
    {
        animator.Play("poop");
        textureAtlas.poopFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayScared()
    {
        animator.Play("scared");
        textureAtlas.scaredFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlaySounds()
    {
        animator.Play("sounds");
        textureAtlas.soundsFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlaySpider()
    {
        animator.Play("spider");
        textureAtlas.spider2Face();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlaySpider2()
    {
        animator.Play("spider");
        textureAtlas.spiderFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayStrongwind()
    {
        animator.Play("strongwind");
        textureAtlas.strongwindFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayThatGuy()
    {
        animator.Play("thatguy");
        textureAtlas.thatguyFace();
        StartCoroutine(TextureSwapRoutine());
    }

    public void PlayThink()
    {
        animator.Play("think");
        textureAtlas.thinkFace();
        StartCoroutine(TextureSwapRoutine());
    }


    private IEnumerator TextureSwapRoutine(float time = 1.5f)
    {
        yield return new WaitForSeconds(time);
        textureAtlas.returntobase();
    }
}
