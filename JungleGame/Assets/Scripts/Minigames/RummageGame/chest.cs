using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chest : MonoBehaviour
{
    public static chest instance;

    private int currBag = 0;
    private const int maxBag = 5;

    [Header("Objects")]
    [SerializeField] private Image image;

    [Header("Images")]
    [SerializeField] private List<Sprite> bagSprites;


    void Awake()
    {
        if (instance == null)
            instance = this;

        image.sprite = bagSprites[currBag];
    }

    public void UpgradeBag()
    {
        if (currBag < maxBag)
        {
            currBag++;
        }

        StartCoroutine(UpgradeChestRoutine());

        // play coin drop sound effect
        AudioManager.instance.PlayCoinDrop();
        // play right choice sound effect
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 1f);
    }

    private IEnumerator UpgradeChestRoutine()
    {
        GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
        
        image.sprite = bagSprites[currBag];
        yield return new WaitForSeconds(0.2f);

        GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
    }

    public void DowngradeBag()
    {
        if (currBag > 0)
        {
            currBag--;
        }

        image.sprite = bagSprites[currBag];
    }

    public void ToggleScaleAndWiggle(bool opt)
    {
        if (opt)
        {
            GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
            GetComponent<WiggleController>().StartWiggle();
        }
        else
        {
            GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
            GetComponent<WiggleController>().StopWiggle();
        }
    }
}
