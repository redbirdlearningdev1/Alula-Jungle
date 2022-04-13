using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

public struct StoryGameEntry
{
    public string storyName;
    public StoryGameBackground background;
    public List<StoryGameSegmentEntry> segments;
}

public struct StoryGameSegmentEntry
{
    public string text;
    public AssetReference audio;
    public bool moveWord;
    public ActionWordEnum actionWord;
    public bool requireInput;
    public string postText;
    public bool advanceBG;
}

public class StroyGameObjectImportException : System.Exception
{
    public string msg;
    public int line;
    public int column;

    public StroyGameObjectImportException(string msg, int line, int column)
    {
        this.msg = msg;
        this.line = line;
        this.column = column;
    }

    public void PrintError()
    {
        GameManager.instance.SendError("StoryGameDatabaseManager", msg + " @ line " + line + ", col " + column);
    }
}

public class StoryGameDatabaseManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown fileDropdown;
    [SerializeField] private TextMeshProUGUI uploadText;
    [SerializeField] private TextMeshProUGUI textText;
    [SerializeField] private TextMeshProUGUI updateText;

    public List<AssetReference> storyGameAudioList;

    private List<StoryGameEntry> storyGameEntries;
    private List<string> filePaths;
    private string fileText;

    private bool loadDone = false;
    private bool testDone = false;

    private const string csv_folder_path = "Assets/Resources/CSV_folder/";
    private const string storygame_folder_path = "Assets/Resources/StoryGameObjects/";

    void Awake()
    {
        // set data to empty
        ResetData();

        // get files in csv folder + set up path list
        var info = new DirectoryInfo(csv_folder_path);
        var fileInfo = info.GetFiles();
        SetupFileDropdown(fileInfo);
        SetupFilePathList(fileInfo);
    }

    public void OnFileDropdownValueChange()
    {
        Debug.Log("resetting data!");
        ResetData();

        // set up the path list
        var info = new DirectoryInfo(csv_folder_path);
        var fileInfo = info.GetFiles();
        SetupFilePathList(fileInfo);
    }

    private void SetupFileDropdown(FileInfo[] filesInfo)
    {
        // refresh database
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        // set profile dropdown
        List<string> profileList = new List<string>();
        for (int i = 0; i < filesInfo.Length; i++)
        {
            // ignore meta files
            if (filesInfo[i].Name.EndsWith(".meta"))
                continue;

            // add to list
            profileList.Add(filesInfo[i].Name);
        }
        // update file dropdown
        fileDropdown.ClearOptions();
        fileDropdown.AddOptions(profileList);
        fileDropdown.value = 0;
    }

    private void SetupFilePathList(FileInfo[] filesInfo)
    {
        filePaths = new List<string>();
        for (int i = 0; i < filesInfo.Length; i++)
        {
            // ignore meta files
            if (filesInfo[i].Name.EndsWith(".meta"))
                continue;

            // add to list
            filePaths.Add(filesInfo[i].FullName);
        }
    }

    private void ResetData()
    {
        storyGameEntries = new List<StoryGameEntry>();
        filePaths = new List<string>();

        storyGameEntries.Clear();
        filePaths.Clear();
        fileText = "";

        uploadText.text = "no file found";
        textText.text = "text not run";
        updateText.text = "database not updated";

        uploadText.color = Color.red;
        textText.color = Color.red;
        updateText.color = Color.red;

        loadDone = false;
        testDone = false;

        InitCreateGlobalList();
    }

    /* 
    ################################################
    #   BUTTON FUNCTIONS
    ################################################
    */

    public void OnUploadCSVPressed()
    {
        loadDone = true;

        // get all text from correct file
        fileText = System.IO.File.ReadAllText(filePaths[fileDropdown.value]);
        Debug.Log("uploading file '" + filePaths[fileDropdown.value] + "'");
        print("full-text: " + fileText);

        // change text and color
        uploadText.color = Color.green;
        uploadText.text = "found file '" + fileDropdown.options[fileDropdown.value].text + "'";
    }

    public void OnTestPressed()
    {
        // only continue iff loaded file
        if (!loadDone)
            return;

        bool firstLine = true;
        bool passTest = true;
        bool readingSegments = false;
        string errorMsg = "";

        // change text and color
        textText.color = Color.cyan;
        textText.text = "testing file '" + fileDropdown.options[fileDropdown.value].text + "'";

        // make new talkie entry
        StoryGameEntry entry = new StoryGameEntry();

        // split text into elkonin values
        string[] lines = fileText.Split('\n');
        int lineCount = 0;
        foreach (string line in lines)
        {
            lineCount++;
            int column_count = 0;

            // skip first line
            if (firstLine)
            {
                firstLine = false;
                continue;
            }

            string[] rowData = line.Split(',');

            // print ("index: " + lineCount + " line: " + line + ", cells: " + rowData.Length);

            // catch any parse errors
            try
            {
                // check that row is 7 long
                if (rowData.Length != 7)
                {
                    throw new StroyGameObjectImportException("invalid row size", lineCount, column_count);
                }

                // reading header
                if (rowData[0].ToString().StartsWith("$"))
                {
                    switch (rowData[0].ToString())
                    {
                        case "$Name": // create new story game entry

                            // add entry to list and make new entry
                            // if (storyGameEntries.Count > 0)
                            //     storyGameEntries.Add(entry);

                            // make new story game entry
                            entry = new StoryGameEntry();
                            entry.segments = new List<StoryGameSegmentEntry>();

                            // add story game name
                            entry.storyName = rowData[1];
                            break;

                        case "$Background": // denotes if talkie is a quip collection

                            switch (rowData[1])
                            {
                                case "Prologue":
                                    entry.background = StoryGameBackground.Prologue;
                                    break;
                                case "Beginning":
                                    entry.background = StoryGameBackground.Beginning;
                                    break;
                                case "FollowRed":
                                    entry.background = StoryGameBackground.FollowRed;
                                    break;
                                case "Emerging":
                                    entry.background = StoryGameBackground.Emerging;
                                    break;
                                case "Resolution":
                                    entry.background = StoryGameBackground.Resolution;
                                    break;
                            }
                            break;

                        case "$Segments":

                            //print ("adding segments!");
                            readingSegments = true; // start reading segments
                            continue;
                    }
                }
                else if (rowData[0] == "")
                {
                    // add to entry to list
                    storyGameEntries.Add(entry);
                    print("finished entry!");
                    readingSegments = false; // finish reading segmenets
                }

                // /* 
                // ################################################
                // #   STORY GAME SEGMENTS
                // ################################################
                // */

                // reading segments
                if (readingSegments)
                {
                    StoryGameSegmentEntry segment = new StoryGameSegmentEntry();

                    foreach (string cell in rowData)
                    {
                        //print ("column count: " + column_count);
                        switch (column_count)
                        {
                            case 0: // string literal

                                segment.text = cell.Replace("\"", "");
                                //print ("text: " + segment.text);
                                break;

                            case 1: // audio

                                segment.audio = SearchForAudioByName(cell); // try to find audio
                                //print ("audio: " + segment.audio.name);
                                break;

                            case 2: // move word?

                                if (cell == "yes")
                                    segment.moveWord = true;
                                else
                                    segment.moveWord = false;
                                //print ("moveword: " + segment.moveWord);
                                break;

                            case 3: // action word
                                if (cell != "")
                                    segment.actionWord = ConvertToActionWord(cell);
                                //print ("actionword: " + segment.actionWord);
                                break;

                            case 4: // require mic input?

                                if (cell == "yes")
                                    segment.requireInput = true;
                                else
                                    segment.requireInput = false;
                                //print ("micinput: " + segment.requireInput);
                                break;

                            case 5: // post text

                                segment.postText = cell;
                                //print ("posttext: " + segment.postText);
                                break;

                            case 6: // advance background

                                if (cell.Contains("yes"))
                                    segment.advanceBG = true;
                                else
                                    segment.advanceBG = false;
                                print("seg.advanceBG: " + segment.advanceBG);
                                break;

                        }
                        column_count++;
                    }
                    // add segment to entry
                    entry.segments.Add(segment);
                    //print ("adding segment");
                }
            }
            catch (StroyGameObjectImportException e)
            {
                passTest = false;
                e.PrintError();
                return;
            }
        }

        // check if there is one last entry
        if (entry.storyName != "")
        {
            storyGameEntries.Add(entry);
        }

        // change test outcome text
        if (passTest)
        {
            textText.color = Color.green;
            textText.text = "test complete - successful!";
            testDone = true;
        }
        else
        {
            textText.color = Color.red;
            textText.text = "test complete - fail: " + errorMsg;
        }
    }

#if UNITY_EDITOR
    public void OnUpdatePressed()
    {
        // only continue iff tests complete
        if (!testDone)
            return;

        updateText.color = Color.cyan;
        updateText.text = "creating objects...";

        // update database using entries
        foreach (var entry in storyGameEntries)
        {
            UpdateCreateObject(entry);
        }

        updateText.color = Color.green;
        updateText.text = "database updated!";
    }

    public void UpdateCreateObject(StoryGameEntry entry)
    {
        string exact_filename = entry.storyName;
        print("exact_filename: " + exact_filename);
        string[] results = AssetDatabase.FindAssets(exact_filename);
        string result = "";

        // filter results to be exact string filename
        foreach (var res in results)
        {
            string path = AssetDatabase.GUIDToAssetPath(res);
            var split = path.Split('/');
            var filename = split[split.Length - 1];

            if (filename == exact_filename + ".asset")
            {
                result = path;
                Debug.Log("result: " + result);
            }
        }

        StoryGameData yourObject = null;

        // create new word pair object
        if (result == "")
        {
            GameManager.instance.SendLog("StoryGameWordDatabase", "!!! creating new story game object -> " + exact_filename);
            yourObject = ScriptableObject.CreateInstance<StoryGameData>();
            AssetDatabase.CreateAsset(yourObject, storygame_folder_path + exact_filename + ".asset");
        }
        // get word pair object
        else
        {
            GameManager.instance.SendLog("StoryGameWordDatabase", "%%% found story game object -> " + exact_filename);
            yourObject = (StoryGameData)AssetDatabase.LoadAssetAtPath(result, typeof(StoryGameData));
        }
        // new segments
        yourObject.segments = new List<StoryGameSegment>();

        // Do your changes
        yourObject.storyName = entry.storyName;
        yourObject.background = entry.background;
        foreach (var seg in entry.segments)
        {
            StoryGameSegment new_seg = new StoryGameSegment();
            new_seg.text = seg.text;
            new_seg.audio = seg.audio;
            new_seg.moveWord = seg.moveWord;
            new_seg.actionWord = seg.actionWord;
            new_seg.requireInput = seg.requireInput;
            new_seg.postText = seg.postText;
            new_seg.advanceBG = seg.advanceBG;

            yourObject.segments.Add(new_seg);
        }

        EditorUtility.SetDirty(yourObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif

    private ActionWordEnum ConvertToActionWord(string val)
    {
        string fixed_val = val.Replace("\r", "");

        switch (fixed_val)
        {
            // action words
            default:
                return ActionWordEnum._blank;
            case "mudslide":
                return ActionWordEnum.mudslide;
            case "listen":
                return ActionWordEnum.listen;
            case "poop":
                return ActionWordEnum.poop;
            case "orcs":
                return ActionWordEnum.orcs;
            case "think":
                return ActionWordEnum.think;
            case "hello":
                return ActionWordEnum.hello;
            case "spider":
                return ActionWordEnum.spider;
            case "explorer":
                return ActionWordEnum.explorer;
            case "scared":
                return ActionWordEnum.scared;
            case "scare":
                return ActionWordEnum.scared;
            case "that_guy":
                return ActionWordEnum.thatguy;
            case "choice":
                return ActionWordEnum.choice;
            case "strong_wind":
                return ActionWordEnum.strongwind;
            case "pirate":
                return ActionWordEnum.pirate;
            case "gorilla":
                return ActionWordEnum.gorilla;
            case "sounds":
                return ActionWordEnum.sounds;
            case "give":
                return ActionWordEnum.give;
            case "backpack":
                return ActionWordEnum.backpack;
            case "howler_monkey":
                return ActionWordEnum.frustrating;
            case "frustrating":
                return ActionWordEnum.frustrating;
            case "bump_head":
                return ActionWordEnum.bumphead;
            case "baby":
                return ActionWordEnum.baby;
            case "baby_monkey":
                return ActionWordEnum.baby;
        }
    }

    private void InitCreateGlobalList()
    {
        var files = Resources.LoadAll<AudioClip>("StoryGameAudioFiles");
        // TODO: MAJOR TESTING REQUIRED HERE
        List<AssetReference> audioRefs = new List<AssetReference>();
        foreach (AudioClip clip in files)
        {
            audioRefs.Add(new AssetReference(clip.name));
        }

        storyGameAudioList = new List<AssetReference>();
        foreach (AssetReference reference in audioRefs)
        {
            storyGameAudioList.Add(reference);
        }

        print("global audio list: " + storyGameAudioList.Count);
    }

    private AssetReference SearchForAudioByName(string str)
    {
        // TODO: MAJOR TESTING REQUIRED
        // linear search
        foreach (var file in storyGameAudioList)
        {
            if (file.ToString() == str)
            {
                //print ("found audio!");
                return file;
            }
        }
        // return null if not found
        return null;
    }
}
