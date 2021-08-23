using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TalkieTestSceneManager : MonoBehaviour
{
    public TalkieObject placeTalkieHere;

    public const string talkie_audio_folder = "Assets/Resources/TalkieAudioFiles/";

    public static List<AudioClip> globalTalkieAudioList;

    public static void InitCreateGlobalList()
    {
        // return if list already created for efficiency
        if (globalTalkieAudioList != null)
            return;

        var files = Resources.LoadAll<AudioClip>("TalkieAudioFiles");

        globalTalkieAudioList = new List<AudioClip>();
        foreach (var file in files)
        {
            globalTalkieAudioList.Add(file);
        }
    }

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


#if UNITY_EDITOR
    public void OnAddAudioPressed()
    {
        AddAudioToTalkie(placeTalkieHere);
    }

    private void AddAudioToTalkie(TalkieObject talkieObject)
    {
        // get all audio clips in one list
        InitCreateGlobalList();

        // make a copy of talkie object to iterate though
        TalkieSegment[] talkieSegments = new TalkieSegment[talkieObject.segmnets.Count];
        talkieObject.segmnets.CopyTo(talkieSegments);

        // go though each segment
        int index = 0;
        foreach (var seg in talkieSegments)
        {
            // only attempt search if audio clip is misisng
            if (seg.audioClip == null)
            {
                AudioClip foundClip = SearchForAudioByName(seg.audioClipName);
                if (foundClip != null)
                {
                    TalkieSegment updatedSegment = new TalkieSegment();
                    // update new segment with prev values
                    updatedSegment.endTalkieAfterThisSegment = seg.endTalkieAfterThisSegment;
                    updatedSegment.requireYN = seg.requireYN;
                    updatedSegment.onYes = seg.onYes;
                    updatedSegment.onNo = seg.onNo;

                    updatedSegment.leftCharacter = seg.leftCharacter;
                    updatedSegment.leftEmotionNum = seg.leftEmotionNum;
                    updatedSegment.leftMouthEnum = seg.leftMouthEnum;
                    updatedSegment.leftEyesEnum = seg.leftEyesEnum;

                    updatedSegment.rightCharacter = seg.rightCharacter;
                    updatedSegment.rightEmotionNum = seg.rightEmotionNum;
                    updatedSegment.rightMouthEnum = seg.rightMouthEnum;
                    updatedSegment.rightEyesEnum = seg.rightEyesEnum;

                    updatedSegment.activeCharacter = seg.activeCharacter;

                    // add file to object segment and save
                    updatedSegment.audioClip = foundClip; // audio to play
                    updatedSegment.audioClipName = seg.audioClipName;
                    updatedSegment.audioString = seg.audioString;

                    talkieObject.segmnets[index] = updatedSegment;

                    EditorUtility.SetDirty(talkieObject);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    print ("updated segment " + index + " with audio clip " + foundClip.name);
                }
            }
            index++;
        }
    }

    public static AudioClip SearchForAudioByName(string str)
    {
        // return if list already created for efficiency
        if (globalTalkieAudioList == null)
            InitCreateGlobalList();

        // linear search
        foreach (var file in globalTalkieAudioList)
        {
            if (file.name == str)
                return file;
        }
        // return null if not found
        return null;
    }

    public void UpdateAllTalkieObjects()
    {
        // get global talkie list
        List<TalkieObject> globalTalkieObjects = TalkieDatabase.instance.GetGlobalTalkieList();
        foreach(var talkie in globalTalkieObjects)
        {
            AddAudioToTalkie(talkie);
            print ("successfully added audio to talkie: " + talkie.name);
        }
    }
#endif
}
