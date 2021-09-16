using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisibleFramesController : MonoBehaviour
{
    public static VisibleFramesController instance;

    public List<GameObject> frames;

    public Transform offScreenPos;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetNumberOfFrames(int num)
    {
        // set each frame to be active
        foreach(var frame in frames)
            frame.SetActive(true);

        // deactivate unused frames
        int size = frames.Count - num;
        for (int i = 0; i < size; i++)
        {
            frames[frames.Count - i - 1].SetActive(false);
        }
    }

    public void ResetFrames()
    {
        // set each frame to be active
        foreach(var frame in frames)
            frame.SetActive(true);
        
        // move frames off-screen
        foreach(var frame in frames)
            frame.transform.localPosition = offScreenPos.localPosition;
    }

    public void PlaceActiveFrames(Vector2 pos)
    {
        foreach(var frame in frames)
        {
            if (frame.activeSelf)
            {
                frame.transform.localPosition = pos;
            }
        }
    }

    public void LerpFramesToInvisibleFrames()
    {
        int count = 0;
        foreach(var frame in frames)
        {
            if (frame.activeSelf)
            {
                frame.GetComponent<LerpableObject>().LerpPosition(InvisibleFrameLayout.instance.frames[count].transform.position, 0.5f, false);
                count++;
            }
        }
    }

    public void AddFrameSmooth()
    {
        StartCoroutine(AddFrameSmoothRoutine());
    }

    private IEnumerator AddFrameSmoothRoutine()
    {
        // count active frames
        int count = 0;
        foreach(var frame in frames)
        {
            if (frame.activeSelf)
            {
                count++;
            }
        }

        // make frame to add invisible frames
        InvisibleFrameLayout.instance.SetNumberOfFrames(count + 1);

        // make newest frame invisible
        frames[count].GetComponent<LerpableObject>().SetImageAlpha(frames[count].GetComponent<Image>(), 0f);

        // make frame active
        SetNumberOfFrames(count + 1);
        yield return new WaitForSeconds(0.5f);

        // move frames to invisible frames
        StartCoroutine(MoveFramesToInvisibleFramesRoutine());
        yield return new WaitForSeconds(1f);

        // reveal new frame
        frames[count].GetComponent<LerpableObject>().LerpImageAlpha(frames[count].GetComponent<Image>(), 1f, 0.5f);
    }

    public void MoveFramesToInvisibleFrames()
    {
        foreach(var frame in frames)
        {
            if (frame.activeSelf)
            {
                frame.transform.localScale = new Vector3(0f, 0f, 1f);
            }
        }
        StartCoroutine(MoveFramesToInvisibleFramesRoutine());
    }

    private IEnumerator MoveFramesToInvisibleFramesRoutine()
    {
        int count = 0;
        foreach(var frame in frames)
        {
            if (frame.activeSelf)
            {
                frame.GetComponent<LerpableObject>().LerpPosition(InvisibleFrameLayout.instance.frames[count].transform.position, 0.5f, false);
                frame.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.5f);
                yield return new WaitForSeconds(0.1f);
            }
            count++;
        }
    }

    public void MoveFramesOffScreen()
    {
        StartCoroutine(MoveFramesOffScreenRoutine());
    }

    private IEnumerator MoveFramesOffScreenRoutine()
    {
        foreach(var frame in frames)
        {
            if (frame.activeSelf)
            {
                frame.GetComponent<LerpableObject>().LerpPosition(new Vector2(frame.transform.position.x, frame.transform.position.y - 500f), 0.5f, false);
            }
        }
        yield return new WaitForSeconds(1f);
        ResetFrames();
    }
}
