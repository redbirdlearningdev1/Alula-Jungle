using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polaroid : MonoBehaviour
{
    [SerializeField] private SpriteRenderer picture;

    public void SetPolaroid(ChallengeWord word)
    {
        // set picture
        picture.sprite = word.sprite;
    }
}
