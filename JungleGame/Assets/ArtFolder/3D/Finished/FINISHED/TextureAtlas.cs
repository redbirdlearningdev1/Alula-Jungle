using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAtlas : MonoBehaviour {

    public Renderer rend;
    public Texture basetex;
    public Texture baby;
    public Texture backpack;
    public Texture bumphead;
    public Texture choice;
    public Texture explorer;
    public Texture frustrating;
    public Texture give;
    public Texture gorilla;
    public Texture hello;
    public Texture listen;
    public Texture mudslide;
    public Texture orcs;
    public Texture pirate;
    public Texture poop;
    public Texture scared;
    public Texture sounds;
    public Texture spider;
    public Texture strongwind;
    public Texture thatguy;
    public Texture think;
    public Texture mudslide2;
    public Texture spider2;

    void Start() { 
    
        rend = GetComponent<MeshRenderer>();
        rend.sharedMaterial.mainTexture = basetex;
    }
	
	void returntobase() {

        rend.sharedMaterial.mainTexture = basetex;

    }

    void babyFace()
    {

        rend.sharedMaterial.mainTexture = baby;

    }


    void backpackFace()
    {

        rend.sharedMaterial.mainTexture = backpack;

    }

    void bumpheadFace()
    {

        rend.sharedMaterial.mainTexture = bumphead;

    }

    void choiceFace()
    {

        rend.sharedMaterial.mainTexture = choice;

    }

    void explorerFace()
    {

        rend.sharedMaterial.mainTexture = explorer;

    }

    void frustratingFace()
    {

        rend.sharedMaterial.mainTexture = frustrating;

    }

    void giveFace()
    {

        rend.sharedMaterial.mainTexture = give;

    }

    void gorillaFace()
    {

        rend.sharedMaterial.mainTexture = gorilla;

    }

    void helloFace()
    {

        rend.sharedMaterial.mainTexture = hello;

    }

    void listenFace()
    {

        rend.sharedMaterial.mainTexture = listen;

    }

    void mudslideFace()
    {

        rend.sharedMaterial.mainTexture = mudslide;

    }

    void orcsFace()
    {

        rend.sharedMaterial.mainTexture = orcs;

    }

    void pirateFace()
    {

        rend.sharedMaterial.mainTexture = pirate;

    }

    void poopFace()
    {

        rend.sharedMaterial.mainTexture = poop;

    }

    void scaredFace()
    {

        rend.sharedMaterial.mainTexture = scared;

    }

    void soundsFace()
    {

        rend.sharedMaterial.mainTexture = sounds;

    }

    void spiderFace()
    {

        rend.sharedMaterial.mainTexture = spider;

    }

    void strongwindFace()
    {

        rend.sharedMaterial.mainTexture = strongwind;

    }

    void thatguyFace()
    {

        rend.sharedMaterial.mainTexture = thatguy;

    }

    void thinkFace()
    {

        rend.sharedMaterial.mainTexture = think;

    }

    void mudslide2Face()
    {

        rend.sharedMaterial.mainTexture = mudslide2;

    }

    void spider2Face()
    {

        rend.sharedMaterial.mainTexture = spider2;

    }
}

