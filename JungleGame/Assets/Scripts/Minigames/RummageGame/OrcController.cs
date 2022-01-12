using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcController : MonoBehaviour
{
    [SerializeField] public GameObject shadow;
    
    private Vector3 origin = new Vector3(-2.4f, 1.5f, 0f);
    private Vector3 pilePosition1 = new Vector3(-2.4f, 0f, 0f);
    private Vector3 pilePosition2 = new Vector3(.15f, 1.85f, 0f);
    private Vector3 pilePosition3 = new Vector3(-1.2f, 3.45f, 0f);
    private Vector3 pilePosition4 = new Vector3(-4.0f, 3.15f, 0f);
    private Vector3 pilePosition5 = new Vector3(-4.8f, 1.25f, 0f);

    private Coroutine current;
    // Start is called before the first frame update


    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = this.transform.position;
        pos.y -= 0.2f;
        shadow.transform.position = pos;
    }

    public int AtLocation()
    {
        if (pilePosition1 == transform.position)
        {
            return 1;
        }
        else if (pilePosition2 == transform.position)
        {
            return 2;
        }
        else if (pilePosition3 == transform.position)
        {
            return 3;
        }
        else if (pilePosition4 == transform.position)
        {
            return 4;
        }
        else if (pilePosition5 == transform.position)
        {
            return 5;
        }
        else
        {
            return 0;
        }

    }


    public void GoToPile1()
    {
        if (current != null)
        {
            StopCoroutine(current);
        }
        // play grass sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WalkGrass, 1f);
        current = StartCoroutine(GoToPile1Routine(pilePosition1));
    }
    

    private IEnumerator GoToPile1Routine(Vector3 target)
    {
        //Debug.Log("Here");
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * .25f;
            animator.Play("orcWalk");
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
                
                if (transform.position.x >= currStart.x)
                {
                    transform.rotation = new Quaternion(0,180,0,0);
                }
                else
                {
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                }
            }
            else
            {
                transform.position = target;

                yield break;
            }

            yield return null;
        }
    }
    public void GoToPile2()
    {
        if ( current != null)
        {
            StopCoroutine(current);
        }
        // play grass sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WalkGrass, 1f);
        current = StartCoroutine(GoToPile2Routine(pilePosition2));
    }

    private IEnumerator GoToPile2Routine(Vector3 target)
    {
        Debug.Log("Here");
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * .25f;
            animator.Play("orcWalk");
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
                
                if (transform.position.x >= currStart.x)
                {
                    transform.rotation = new Quaternion(0, 180, 0, 0);

                }
                else
                {
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                }
            }
            else
            {
                transform.position = target;

                yield break;
            }

            yield return null;
        }
    }
    public void GoToPile3()
    {
        if (current != null)
        {
            StopCoroutine(current);
        }
        // play grass sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WalkGrass, 1f);
        current = StartCoroutine(GoToPile3Routine(pilePosition3));
    }

    private IEnumerator GoToPile3Routine(Vector3 target)
    {
        Debug.Log("Here");
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * .25f;
            animator.Play("orcWalk");
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
                if (transform.position.x >= currStart.x)
                {
                    transform.rotation = new Quaternion(0, 180, 0, 0);
                }
                else
                {
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                }
            }
            else
            {
                transform.position = target;

                yield break;
            }
            
            yield return null;
        }
        
    }
    public void rotatePile3()
    {
        transform.rotation = new Quaternion(0, 180, 0, 0);
    }
    

    public void GoToPile4()
    {
        if (current != null)
        {
            StopCoroutine(current);
        }
        // play grass sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WalkGrass, 1f);
        current = StartCoroutine(GoToPile4Routine(pilePosition4));
    }

    private IEnumerator GoToPile4Routine(Vector3 target)
    {
        Debug.Log("Here");
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * .25f;
            animator.Play("orcWalk");
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
                if (transform.position.x >= currStart.x)
                {
                    transform.rotation = new Quaternion(0, 180, 0, 0);
                }
                else
                {
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                }
            }
            else
            {
                transform.position = target;

                yield break;
            }

            yield return null;
        }

    }

    public void GoToPile5()
    {
        if (current != null)
        {
            StopCoroutine(current);
        }
        // play grass sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WalkGrass, 1f);
        current = StartCoroutine(GoToPile5Routine(pilePosition5));
    }

    private IEnumerator GoToPile5Routine(Vector3 target)
    {
        Debug.Log("Here");
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * .25f;
            animator.Play("orcWalk");
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
                if (transform.position.x >= currStart.x)
                {
                    transform.rotation = new Quaternion(0, 180, 0, 0);
                }
                else
                {
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                }
            }
            else
            {
                transform.position = target;

                yield break;
            }

            yield return null;
        }
    }


    public void GoToOrigin()
    {
        if (current != null)
        {
            StopCoroutine(current);
        }
        // play grass sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WalkGrass, 1f);
        current = StartCoroutine(GoToOriginRoutine(origin));
    }

    private IEnumerator GoToOriginRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 0.5f;
        animator.Play("orcWalk");
        while (true)
        {
            // animate movement
            timer += Time.deltaTime * .25f;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
                if (transform.position.x >= currStart.x)
                {
                    transform.rotation = new Quaternion(0, 180, 0, 0);
                }
                else
                {
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                }
            }
            else
            {
                transform.position = target;

                yield break;
            }
            //yield return new WaitForSeconds(1.25f);
            //animator.Play("orcIdle");
            yield return null;
        }
    }

    public void movingOrc()
    {
        StartCoroutine(movingOrcRoutine());
    }

    private IEnumerator movingOrcRoutine()
    {
        yield return new WaitForSeconds(0f);
        animator.Play("orcWalk");
    }

    public void stopOrc()
    {
        StartCoroutine(stopOrcRoutine());
    }

    private IEnumerator stopOrcRoutine()
    {
        yield return new WaitForSeconds(0f);
        animator.Play("orcIdle");
    }

    public void channelOrc()
    {
        StartCoroutine(channelOrcRoutine());
    }

    private IEnumerator channelOrcRoutine()
    {
        yield return new WaitForSeconds(0f);
        animator.Play("orcRummage");
        // play rummage sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WoodRummage, 0.5f, "wood_rummage");
    }

    public void successOrc()
    {
        StartCoroutine(successOrcRoutine());
    }

    private IEnumerator successOrcRoutine()
    {
        animator.Play("orcCelebrate");
        yield return new WaitForSeconds(1f);
    }

    public void failOrc()
    {
        StartCoroutine(failOrcRoutine());
    }

    private IEnumerator failOrcRoutine()
    {
        animator.Play("orcNo");
        yield return new WaitForSeconds(1f);
    }
}
