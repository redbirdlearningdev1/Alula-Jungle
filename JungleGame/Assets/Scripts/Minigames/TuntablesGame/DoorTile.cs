using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DoorTile : MonoBehaviour
{
    public Image image;
    public bool isCenterTile;
    

    // set the door tile to be a new sprite
    public void SetTile(ActionWordEnum word, bool instant = false)
    {
        if (instant)
        {
            image.sprite = GetTileSprite(word);
        }
        else
        {
            StartCoroutine(SwapTileRoutine(word));
        }
    }

    // swaps tiles with shaking animation
    private IEnumerator SwapTileRoutine(ActionWordEnum word)
    {
        GetComponent<WiggleController>().StartWiggle();
        yield return new WaitForSeconds(0.5f);
        image.sprite = GetTileSprite(word);
        yield return new WaitForSeconds(0.5f);
        GetComponent<WiggleController>().StopWiggle();
    }

    // returns the correct sprite
    private Sprite GetTileSprite(ActionWordEnum word)
    {
        if (isCenterTile)
        {
            return GameManager.instance.GetActionWord(word).centerIcon;
        }
        else
        {
            return GameManager.instance.GetActionWord(word).doorIcon;
        }
    }
}
