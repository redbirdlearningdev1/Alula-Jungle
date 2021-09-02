using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
