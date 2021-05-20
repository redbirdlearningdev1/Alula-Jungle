using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pileRummage : MonoBehaviour
{
    // Start is called before the first frame update
    private int currPile = 0;
    private const int maxPile = 1;
    public bool chosen = false;

    [Header("Objects")]
    [SerializeField] private Image Pile;


    [Header("Images")]
    [SerializeField] private List<Sprite> pileSprites;




    void Awake()
    {
        Pile.sprite = pileSprites[currPile];
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       

    }
    public void UpgradeBag()
    {
        if (currPile < maxPile)
        {
            currPile++;
        }

        Pile.sprite = pileSprites[currPile];

    }
    public void pileChose()
    {
        if(currPile == 0)
        {
            chosen = true;
            Debug.Log(Pile.ToString());
        }
        

    }
    public void pileComplete()
    {
        chosen = false;
    }
}
