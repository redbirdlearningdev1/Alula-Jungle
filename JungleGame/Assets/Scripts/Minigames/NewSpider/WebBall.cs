using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebBall : MonoBehaviour
{
    public static WebBall instance;

    private int currBall = 0;
    private const int maxBall = 4;

    [Header("Objects")]
    [SerializeField] private Image image;


    [Header("Images")]
    [SerializeField] private List<Sprite> chestBall;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        image.sprite = chestBall[currBall];
    }

    public void UpgradeChest()
    {
        if (currBall < maxBall)
        {
            currBall++;
        }

        StartCoroutine(UpgradeChestRoutine());
    }

    private IEnumerator UpgradeChestRoutine()
    {
        GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
        image.sprite = chestBall[currBall];
        // play coin drop sound
        AudioManager.instance.PlayCoinDrop();
        yield return new WaitForSeconds(0.2f);
        GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
    }

    public void DowngradeChest()
    {
        if (currBall > 0)
        {
            currBall--;
        }
        image.sprite = chestBall[currBall];
    }
}
