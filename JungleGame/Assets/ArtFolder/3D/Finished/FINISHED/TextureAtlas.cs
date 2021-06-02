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
	
	public void returntobase() {

        rend.sharedMaterial.mainTexture = basetex;

    }

    public void babyFace()
    {

        rend.sharedMaterial.mainTexture = baby;

    }


    public void backpackFace()
    {

        rend.sharedMaterial.mainTexture = backpack;

    }

    public void bumpheadFace()
    {

        rend.sharedMaterial.mainTexture = bumphead;

    }

    public void choiceFace()
    {

        rend.sharedMaterial.mainTexture = choice;

    }

    public void explorerFace()
    {

        rend.sharedMaterial.mainTexture = explorer;

    }

    public void frustratingFace()
    {

        rend.sharedMaterial.mainTexture = frustrating;

    }

    public void giveFace()
    {

        rend.sharedMaterial.mainTexture = give;

    }

    public void gorillaFace()
    {

        rend.sharedMaterial.mainTexture = gorilla;

    }

    public void helloFace()
    {

        rend.sharedMaterial.mainTexture = hello;

    }

    public void listenFace()
    {

        rend.sharedMaterial.mainTexture = listen;

    }

    public void mudslideFace()
    {

        rend.sharedMaterial.mainTexture = mudslide;

    }

    public void orcsFace()
    {

        rend.sharedMaterial.mainTexture = orcs;

    }

    public void pirateFace()
    {

        rend.sharedMaterial.mainTexture = pirate;

    }

    public void poopFace()
    {

        rend.sharedMaterial.mainTexture = poop;

    }

    public void scaredFace()
    {

        rend.sharedMaterial.mainTexture = scared;

    }

    public void soundsFace()
    {

        rend.sharedMaterial.mainTexture = sounds;

    }

    public void spiderFace()
    {

        rend.sharedMaterial.mainTexture = spider;

    }

    public void strongwindFace()
    {

        rend.sharedMaterial.mainTexture = strongwind;

    }

    public void thatguyFace()
    {

        rend.sharedMaterial.mainTexture = thatguy;

    }

    public void thinkFace()
    {

        rend.sharedMaterial.mainTexture = think;

    }

    public void mudslide2Face()
    {

        rend.sharedMaterial.mainTexture = mudslide2;

    }

    public void spider2Face()
    {

        rend.sharedMaterial.mainTexture = spider2;

    }
}

