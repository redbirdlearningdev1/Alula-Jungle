
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct BackgroundSprites
{
    public StoryGameBackground gameBackground;
    public Sprite skySprite;
    public List<BackgroundLoop> loops;
}

[System.Serializable]
public struct BackgroundLoop
{
    public bool isConnector;
    public Sprite front;
    public Sprite mid;
    public Sprite back;
    public Vector2 size;
}

public enum StoryGameLayer
{
    front, mid, back
}

public class ScrollingBackground : MonoBehaviour
{
    public static ScrollingBackground instance;

    public Animator gorillaAnimator;
    public GameObject buildingBlock;
    public Vector2 fullLayerSize;

    [Header("Background Sprite Database")]
    public BackgroundSprites prologueSprites;
    public BackgroundSprites beginningSprites;
    public BackgroundSprites followRedSprites;
    public BackgroundSprites emergingSprites;
    public BackgroundSprites resolutionSprites;

    [Header("Layers")]
    public Transform skyLayer;
    public Transform backLayer;
    public Transform midLayer;
    public Transform frontLayer;

    [Header("Parallax Speeds")]
    public float frontSpeed;
    public float midSpeed;
    public float backSpeed;

    // private variables
    private bool isMoving = false;
    private BackgroundSprites currentSprites;
    private int currIndex;
    private float overlapAmount = 0.1f;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Update()
    {
        // dev testing stuff
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (isMoving) StopMoving();
            else StartMoving();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            IncreaseLoopIndex();
        }


        if (isMoving)
        {
            MoveLayer(StoryGameLayer.front);
            MoveLayer(StoryGameLayer.mid);
            MoveLayer(StoryGameLayer.back);
        }
    }

    private void MoveLayer(StoryGameLayer layerEnum)
    {
        // grab layer transform and speed
        Transform layer = null;
        float speed = 0;
        switch (layerEnum)
        {
            default:
            case StoryGameLayer.front:
                layer = frontLayer;
                speed = frontSpeed;
                break;
            case StoryGameLayer.mid:
                layer = midLayer;
                speed = midSpeed;
                break;
            case StoryGameLayer.back:
                layer = backLayer;
                speed = backSpeed;
                break;
        }

        // move each child of layer transform left and equal amount
        foreach(Transform t in layer)
        {
            Vector3 pos = t.transform.position;
            pos.x -= speed;
            t.transform.position = pos;
        }

        // if the last child in layer is near the edge of the right side of screen, add more blocks
        RectTransform lastBlock = layer.GetChild(layer.childCount - 1).GetComponent<RectTransform>();

        if (lastBlock.transform.localPosition.x + lastBlock.sizeDelta.x < 600)
        {
            // print ("last block pos x: " + lastBlock.transform.localPosition.x);
            // print ("last width: " + lastBlock.sizeDelta.x);

            BackgroundLoop loop = currentSprites.loops[currIndex];
            // get sprite and size
            Sprite sprite = null;
            Vector2 size = loop.size;
            switch (layerEnum)
            {
                default:
                case StoryGameLayer.front:
                    sprite = loop.front;
                    break;
                case StoryGameLayer.mid:
                    sprite = loop.mid;
                    break;
                case StoryGameLayer.back:
                    sprite = loop.back;
                    break;
            }

            // add block to layer
            GameObject obj = Instantiate(buildingBlock, layer);
            obj.GetComponent<ParallaxBlock>().SetBlock(sprite, size);
            obj.transform.localPosition = new Vector3(lastBlock.transform.localPosition.x + (lastBlock.sizeDelta.x / 2) + (size.x / 2) - overlapAmount, 0f, 0f);

            // start deleting blocks if child size is too large
            if (layer.childCount >= 8)
            {
                Destroy(layer.GetChild(0).gameObject);
            }
        }
    }

    public void StartMoving()
    {
        isMoving = true;
        gorillaAnimator.Play("walk");
    }

    public void StopMoving()
    {
        isMoving = false;
        gorillaAnimator.Play("sit_think");
    }

    public void IncreaseLoopIndex()
    {
        currIndex++;
        if (currIndex >= currentSprites.loops.Count)
        {
            currIndex = 0;
        }

        // add connector to all layers if connector segment
        BackgroundLoop loop = currentSprites.loops[currIndex];
        if (loop.isConnector)
        {
            AddConnector(StoryGameLayer.front, loop.front, loop.size);
            AddConnector(StoryGameLayer.mid, loop.mid, loop.size);
            AddConnector(StoryGameLayer.back, loop.back, loop.size);

            IncreaseLoopIndex();
        }
    }

    private void AddConnector(StoryGameLayer layerEnum, Sprite sprite, Vector2 size, int currSize = 0)
    {
        Transform layer = null;
        switch (layerEnum)
        {
            default:
            case StoryGameLayer.front:
                layer = frontLayer;
                break;
            case StoryGameLayer.mid:
                layer = midLayer;
                break;
            case StoryGameLayer.back:
                layer = backLayer;
                break;
        }

        RectTransform lastBlock = null;

        if (layer.childCount > 0)
            lastBlock = layer.GetChild(layer.childCount - 1).GetComponent<RectTransform>();

        // add block to end of layer
        GameObject obj = Instantiate(buildingBlock, layer);
        obj.GetComponent<ParallaxBlock>().SetBlock(sprite, size);

        if (lastBlock != null)
            obj.transform.localPosition = new Vector3(lastBlock.transform.localPosition.x + (lastBlock.sizeDelta.x / 2) + (size.x / 2) - overlapAmount, 0f, 0f);
        else
            obj.transform.localPosition = new Vector3(currSize + (size.x / 2), 0f, 0f);
    }

    public void SetBackgroundType(StoryGameBackground background)
    {
        ResetLayers();

        switch (background)
        {
            case StoryGameBackground.Prologue:
                SetupBackground(prologueSprites);
                break;
            case StoryGameBackground.Beginning:
                SetupBackground(beginningSprites);
                break;
            case StoryGameBackground.FollowRed:
                SetupBackground(followRedSprites);
                break;
            case StoryGameBackground.Emerging:
                SetupBackground(emergingSprites);
                break;
            case StoryGameBackground.Resolution:
                SetupBackground(resolutionSprites);
                break;
        }
    }

    private void ResetLayers()
    {
        // destroy all children in sky layer
        foreach(Transform t in skyLayer)
            Destroy(t);

        // destroy all children in back layer
        foreach(Transform t in backLayer)
            Destroy(t);
        
        // destroy all children in mid layer
        foreach(Transform t in midLayer)
            Destroy(t);
        
        // destroy all children in front layer
        foreach(Transform t in frontLayer)
            Destroy(t);

        currIndex = 0;
    }

    private void SetupBackground(BackgroundSprites sprites)
    {
        // set surrent sprites
        currentSprites = sprites;

        // add sky layer
        GameObject obj = Instantiate(buildingBlock, skyLayer);
        obj.GetComponent<ParallaxBlock>().SetBlock(currentSprites.skySprite, fullLayerSize);
        obj.transform.localPosition = new Vector3(0f, 0f, 0f);

        BackgroundLoop currLoop = currentSprites.loops[currIndex];
        // fill first connector loop
        if (currLoop.isConnector)
        {
            FillLayer(StoryGameLayer.front, currLoop.front, currLoop.size, true);
            FillLayer(StoryGameLayer.mid, currLoop.mid, currLoop.size, true);
            FillLayer(StoryGameLayer.back, currLoop.back, currLoop.size, true);
            // go to next loop
            currIndex++;
            currLoop = currentSprites.loops[currIndex];
        }
        // fill the rest of the layers
        FillLayer(StoryGameLayer.front, currLoop.front, currLoop.size, false);
        FillLayer(StoryGameLayer.mid, currLoop.mid, currLoop.size, false);
        FillLayer(StoryGameLayer.back, currLoop.back, currLoop.size, false);
    }

    private void FillLayer(StoryGameLayer layerEnum, Sprite sprite, Vector2 size, bool isConnector = false)
    {       
        int currSize = -400;

        // determine layer
        Transform layer = null;
        switch (layerEnum)
        {
            default:
            case StoryGameLayer.front:
                layer = frontLayer;
                break;
            case StoryGameLayer.mid:
                layer = midLayer;
                break;
            case StoryGameLayer.back:
                layer = backLayer;
                break;
        }

        // add single connector
        if (isConnector)
        {
            AddConnector(layerEnum, sprite, size, currSize);
        }
        // fill layer until size is larger than screen width (800)
        else
        {
            while (currSize < 400)
            {
                // if the last child in layer is near the edge of the right side of screen, add more blocks
                RectTransform lastBlock = null;
                if (layer.childCount > 0)
                    lastBlock = layer.GetChild(layer.childCount - 1).GetComponent<RectTransform>();

                // add block to layer
                GameObject obj = Instantiate(buildingBlock, layer);
                obj.GetComponent<ParallaxBlock>().SetBlock(sprite, size);

                if (lastBlock != null)
                    obj.transform.localPosition = new Vector3(lastBlock.transform.localPosition.x + (lastBlock.sizeDelta.x / 2) + (size.x / 2) - overlapAmount, 0f, 0f);
                else
                    obj.transform.localPosition = new Vector3(currSize + (size.x / 2), 0f, 0f);

                // increase layer size
                currSize += (int)size.x;
            }
        }
    }
}
