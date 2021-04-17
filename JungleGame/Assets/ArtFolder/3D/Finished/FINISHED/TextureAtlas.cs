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

    private List<Texture> textures;

    void Start() 
    { 
        rend.sharedMaterial.mainTexture = basetex;
        CreateTextureList();
    }

    private void CreateTextureList()
    {
        textures= new List<Texture>();
        textures.Add(basetex);
        textures.Add(baby);
        textures.Add(backpack);
        textures.Add(bumphead);
        textures.Add(choice);
        textures.Add(explorer);
        textures.Add(frustrating);
        textures.Add(give);
        textures.Add(gorilla);
        textures.Add(hello);
        textures.Add(listen);
        textures.Add(mudslide);
        textures.Add(orcs);
        textures.Add(pirate);
        textures.Add(poop);
        textures.Add(scared);
        textures.Add(sounds);
        textures.Add(spider);
        textures.Add(strongwind);
        textures.Add(thatguy);
        textures.Add(think);
        textures.Add(mudslide2);
        textures.Add(spider2);
    }

    public void RandomTexture()
    {
        int index = Random.Range(0, textures.Count);
        rend.sharedMaterial.mainTexture = textures[index];
    }
	
	public void ReturnToBase() 
    {
        rend.sharedMaterial.mainTexture = basetex;
    }

    public void BabyFace()
    {
        rend.sharedMaterial.mainTexture = baby;
    }


    public void BackpackFace()
    {
        rend.sharedMaterial.mainTexture = backpack;
    }

    public void BumpheadFace()
    {
        rend.sharedMaterial.mainTexture = bumphead;
    }

    public void ChoiceFace()
    {
        rend.sharedMaterial.mainTexture = choice;
    }

    public void ExplorerFace()
    {
        rend.sharedMaterial.mainTexture = explorer;
    }

    public void FrustratingFace()
    {
        rend.sharedMaterial.mainTexture = frustrating;
    }

    public void GiveFace()
    {
        rend.sharedMaterial.mainTexture = give;
    }

    public void GorillaFace()
    {
        rend.sharedMaterial.mainTexture = gorilla;
    }

    public void HelloFace()
    {
        rend.sharedMaterial.mainTexture = hello;
    }

    public void ListenFace()
    {
        rend.sharedMaterial.mainTexture = listen;
    }

    public void MudslideFace()
    {
        rend.sharedMaterial.mainTexture = mudslide;
    }

    public void OrcsFace()
    {
        rend.sharedMaterial.mainTexture = orcs;
    }
    
    public void PirateFace()
    {
        rend.sharedMaterial.mainTexture = pirate;
    }

    public void PoopFace()
    {
        rend.sharedMaterial.mainTexture = poop;
    }

    public void ScaredFace()
    {
        rend.sharedMaterial.mainTexture = scared;
    }

    public void SoundsFace()
    {
        rend.sharedMaterial.mainTexture = sounds;
    }

    public void SpiderFace()
    {
        rend.sharedMaterial.mainTexture = spider;
    }

    public void StrongwindFace()
    {
        rend.sharedMaterial.mainTexture = strongwind;
    }

    public void ThatGuyFace()
    {
        rend.sharedMaterial.mainTexture = thatguy;
    }

    public void ThinkFace()
    {
        rend.sharedMaterial.mainTexture = think;
    }

    public void Mudslide2Face()
    {
        rend.sharedMaterial.mainTexture = mudslide2;
    }

    public void Spider2Face()
    {
        rend.sharedMaterial.mainTexture = spider2;
    }
}