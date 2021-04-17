using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancingManTestScene : MonoBehaviour
{
    public DancingManController dancingMan;

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            dancingMan.PlayBaby();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            dancingMan.PlayBackpack();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            dancingMan.PlayBumphead();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            dancingMan.PlayChoice();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            dancingMan.PlayExplorer();
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            dancingMan.PlayFrustrating();
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            dancingMan.PlayGive();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            dancingMan.PlayGorilla();
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            dancingMan.PlayHello();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            dancingMan.PlayListen();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            dancingMan.PlayMudslide();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            dancingMan.PlayMudslide2();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            dancingMan.PlayOrc();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            dancingMan.PlayPirate();
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            dancingMan.PlayPoop();
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            dancingMan.PlayScared();
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            dancingMan.PlaySounds();
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            dancingMan.PlaySpider();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            dancingMan.PlaySpider2();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            dancingMan.PlayStrongwind();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dancingMan.PlayThatGuy();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            dancingMan.PlayThink();
        }
    }
}
