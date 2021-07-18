using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite fullStar;


    public void SetStar(bool full)
    {
        print ("setting full star: " + full);
        // get sprite renderer if null
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (full)
        {
            print ("full star");
            spriteRenderer.sprite = fullStar;
        }
        else
        {
            print ("empty star");
            spriteRenderer.sprite = emptyStar;
        }
            
    }
}
