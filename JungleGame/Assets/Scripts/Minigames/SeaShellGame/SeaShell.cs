using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeaShell : MonoBehaviour
{

    [SerializeField] public GameObject shell;
    [SerializeField] public GameObject shadow;

    public ActionWordEnum type;
    public int logIndex;
    public Transform originalPos;
    public Transform shellParent;
    public float moveSpeed = 5f;



    private BoxCollider2D myCollider;
    private Image image;
    private bool audioPlaying;

    // original vars
    
    private bool originalSet = false;
    public bool raycast;
    [HideInInspector] public bool isClicked;

    void Awake()
    {

        RectTransform rt = GetComponent<RectTransform>();
        myCollider = gameObject.AddComponent<BoxCollider2D>();
        myCollider.size = rt.sizeDelta;

        image = GetComponent<Image>();
    }

    void Update()
    {
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

    public void ReturnToLog()
    {
        StartCoroutine(ReturnToOriginalPosRoutine(originalPos.position));
    }

    private IEnumerator ReturnToOriginalPosRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * moveSpeed;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.position = target;
                transform.SetParent(shellParent);
                yield break;
            }

            yield return null;
        }
    }

    public void SetShellType(ActionWordEnum type)
    {
        this.type = type;
        
    }

    public void PlayPhonemeAudio()
    {
        if (!audioPlaying)
        {
            StartCoroutine(PlayPhonemeAudioRoutine());
        }
    }


    private IEnumerator PlayPhonemeAudioRoutine()
    {
        audioPlaying = true;
        AudioManager.instance.PlayPhoneme(type);
        yield return new WaitForSeconds(1f);
        audioPlaying = false;
    }

    public void ToggleVisibility(bool opt, bool smooth)
    {
        if (smooth)
            StartCoroutine(ToggleVisibilityRoutine(opt));
        else
        {
            if (!image)
                image = GetComponent<Image>();
            Color temp = image.color;
            if (opt) { temp.a = 1f; }
            else { temp.a = 0; }
            image.color = temp;
        }
    }

    private IEnumerator ToggleVisibilityRoutine(bool opt)
    {
        float end = 0f;
        if (opt) { end = 1f; }
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            Color temp = image.color;
            temp.a = Mathf.Lerp(temp.a, end, timer);
            image.color = temp;

            if (image.color.a == end)
            {
                break;
            }
            yield return null;
        }
    }
}
