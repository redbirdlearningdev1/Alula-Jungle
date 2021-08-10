using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkieTestSceneManager : MonoBehaviour
{
    public TalkieObject placeTalkieHere;

    void Update()
    {
        // press space to end talkie
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TalkieManager.instance.StopTalkieSystem();
        }
    }

    public void OnTestTalkiePressed()
    {
        if (placeTalkieHere != null)
        {
            TalkieManager.instance.PlayTalkie(placeTalkieHere);
        }
    }
}
