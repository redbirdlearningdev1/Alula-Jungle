using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvisibleFrameLayout : MonoBehaviour
{
    public static InvisibleFrameLayout instance;

    public HorizontalLayoutGroup layoutGroup;

    public float[] spacings;
    public List<GameObject> frames;

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

        // set correct spacing between frames
        layoutGroup.spacing = spacings[num - 1];
    }   
}
