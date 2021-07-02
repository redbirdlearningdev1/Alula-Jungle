using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatternRightWrong : MonoBehaviour
{
    private int currState = 0;

    [Header("Objects")]
    [SerializeField] private SpriteRenderer state;

    [Header("Images")]
    [SerializeField] private List<Sprite> states;

    void Awake()
    {
        state.sprite = states[currState];
    }

    public void baseState()
    {


        state.sprite = states[0];
    }
    public void correct()
    {


        state.sprite = states[1];
    }
    public void incorrect()
    {


        state.sprite = states[2];
    }
}
