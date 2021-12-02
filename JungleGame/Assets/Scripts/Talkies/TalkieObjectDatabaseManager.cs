using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEditor;

public class TalkieObjectImportException : System.Exception
{
    public string msg;
    public int line;
    public int column;

    public TalkieObjectImportException(string msg, int line, int column)
    {
        this.msg = msg;
        this.line = line;
        this.column = column;
    }

    public void PrintError()
    {
        GameManager.instance.SendError("TalkieObjectDatabaseManager", msg + " @ line " + line + ", col " + column);
    }
}

// used to transfer data from string representation to enum / int representation
public struct TalkieEmotionEntry
{
    public int emotionNum;
    public TalkieMouth mouthEnum;
    public TalkieEyes eyesEnum;
}

#if UNITY_EDITOR
public class TalkieObjectDatabaseManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown fileDropdown;
    [SerializeField] private TextMeshProUGUI uploadText;
    [SerializeField] private TextMeshProUGUI textText;
    [SerializeField] private TextMeshProUGUI updateText;

    private const string csv_folder_path = "Assets/Resources/CSV_folder/";
    public const string talkie_audio_folder = "Assets/Resources/TalkieAudioFiles/";

    public  List<AudioClip> globalTalkieAudioList;
    private List<TalkieObject> localTalkieObjects;
    private List<string> filePaths;
    private string fileText;

    private bool loadDone = false;
    private bool testDone = false;

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

    private void ResetData()
    {
        localTalkieObjects = new List<TalkieObject>();
        filePaths = new List<string>();

        localTalkieObjects.Clear();
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

    public void OnFileDropdownValueChange()
    {
        Debug.Log("reseting data!");
        ResetData();

        // set up the path list
        var info = new DirectoryInfo(csv_folder_path);
        var fileInfo = info.GetFiles();
        SetupFilePathList(fileInfo);
    }

    private void SetupFileDropdown(FileInfo[] filesInfo)
    {
        // refresh database
        AssetDatabase.Refresh();

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

    public void OnUploadCSVPressed()
    {
        loadDone = true;

        // get all text from correct file
        fileText = System.IO.File.ReadAllText(filePaths[fileDropdown.value]);
        Debug.Log("uploading file '" + filePaths[fileDropdown.value] + "'");
        //print ("full-text: " + fileText);

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
        bool readingVoiceovers = false;
        string errorMsg = "";

        // word countd
        int redWordCount = 0;
        int darwinWordCount = 0;
        int wallyWordCount = 0;
        int lesterWordCount = 0;
        int juliusWordCount = 0;
        int brutusWordCount = 0;
        int marcusWordCount = 0;
        int cloggWordCount = 0;
        int spindleWordCount = 0;
        int bubblesWordCount = 0;
        int ollieWordCount = 0;
        int celesteWordCount = 0;
        int sylvieWordCount = 0;


        // change text and color
        textText.color = Color.cyan;
        textText.text = "testing file '" + fileDropdown.options[fileDropdown.value].text + "'";

        localTalkieObjects = new List<TalkieObject>();

        // make new talkie entry
        TalkieObject entry = null;

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

            print ("index: " + lineCount + " line: " + line + ", cells: " + rowData.Length);
            
            // catch any parse errors
            try
            {
                // check that row is 12 long
                if (rowData.Length != 12)
                {   
                    throw new TalkieObjectImportException("invalid row size", lineCount, column_count);
                }

                // reading header
                if (rowData[0].ToString().StartsWith("$"))
                {
                    switch (rowData[0].ToString())
                    {
                        case "$Title": // create new talkie object

                            // add to local talkie list iff not null
                            if (entry != null)
                            {
                                localTalkieObjects.Add(entry);
                                print ("adding entry!");
                                entry = null;
                            } 

                            print ("making new talkie!");

                            // make new talkie entry
                            entry = new TalkieObject();
                            entry.segmnets = new List<TalkieSegment>();

                            // add talkie name
                            entry.talkieName = rowData[1];
                            break;

                        case "$Quip Collection": // denotes if talkie is a quip collection

                            if (rowData[1] == "yes")
                                entry.quipsCollection = true;
                            else if (rowData[1] == "no")
                                entry.quipsCollection = false;
                            else
                                throw new TalkieObjectImportException("invalid \'$Quip\' option (should be \'yes\' or \'no\'", lineCount, column_count);
                            break;

                        case "$Talkie Start": // denotes how the talkies initially enter

                            if (rowData[1] == "up")
                                entry.start = TalkieStart.EnterUp;
                            else if (rowData[1] == "sides")
                                entry.start = TalkieStart.EnterSides;
                            else if (rowData[1] == "left")
                                entry.start = TalkieStart.EnterLeft;
                            else if (rowData[1] == "right")
                                entry.start = TalkieStart.EnterRight;
                            else
                                throw new TalkieObjectImportException("invalid \'$Talkie Start\' option (should be \'up\', \'sides\', \'left\', or\'right\'", lineCount, column_count);
                            break;

                        case "$Talkie End": // denotes how the talkies exit at the end

                            if (rowData[1] == "down")
                                entry.ending = TalkieEnding.ExitDown;
                            else if (rowData[1] == "sides")
                                entry.ending = TalkieEnding.ExitSides;
                            else if (rowData[1] == "left")
                                entry.ending = TalkieEnding.ExitLeft;
                            else if (rowData[1] == "right")
                                entry.ending = TalkieEnding.ExitRight;
                            else
                                throw new TalkieObjectImportException("invalid \'$Talkie End\' option (should be \'down\', \'sides\', \'left\', or\'right\'", lineCount, column_count);
                            break;

                        case "$Dim BG Start": // dim the background when starting ?
                            
                            if (rowData[1] == "yes")
                                entry.addBackgroundBeforeTalkie = true;
                            else if (rowData[1] == "no")
                                entry.addBackgroundBeforeTalkie = false;
                            else
                                throw new TalkieObjectImportException("invalid \'$Dim BG Start\' option (should be \'yes\' or \'no\'", lineCount, column_count);
                            break;

                        case "$Dim BG End": // undim the backgrund when ending ?
                            
                            if (rowData[1] == "yes")
                                entry.removeBackgroundAfterTalkie = true;
                            else if (rowData[1] == "no")
                                entry.removeBackgroundAfterTalkie = false;
                            else
                                throw new TalkieObjectImportException("invalid \'$Dim BG End\' option (should be \'yes\' or \'no\'", lineCount, column_count);
                            break;

                        case "$Letterbox Start": // show letterbox bars when starting ?

                            if (rowData[1] == "yes")
                                entry.addLetterboxBeforeTalkie = true;
                            else if (rowData[1] == "no")
                                entry.addLetterboxBeforeTalkie = false;
                            else
                                throw new TalkieObjectImportException("invalid \'$Letterbox Start\' option (should be \'yes\' or \'no\'", lineCount, column_count);
                            break;

                        case "$Letterbox End": // remove letterbox bars when ending ?
                            
                            if (rowData[1] == "yes")
                                entry.removeLetterboxAfterTalkie = true;
                            else if (rowData[1] == "no")
                                entry.removeLetterboxAfterTalkie = false;
                            else
                                throw new TalkieObjectImportException("invalid \'$Letterbox End\' option (should be \'yes\' or \'no\'", lineCount, column_count);
                            break;

                        case "$Segments":

                            print ("adding segments!");
                            readingSegments = true; // start reading segments
                            continue;

                        case "$Voiceover":

                            print ("starting voiceovers!");
                            readingVoiceovers = true;
                            break;
                    }
                }
                else if (rowData[0] == "")
                {
                    print ("empty line detected!");
                    // add to local talkie list iff not null
                    if (entry != null)
                    {
                        localTalkieObjects.Add(entry);
                        print ("finished talkie entry!");
                        entry = null;
                    }        
                    
                    readingSegments = false; // finish reading segmenets
                    readingVoiceovers = false; // finish reading voiceover
                }

                // /* 
                // ################################################
                // #   VOICEOVERS / TALKIE SEGMENTS
                // ################################################
                // */ 

                // reading voiceovers
                if (readingVoiceovers)
                {
                    foreach(string cell in rowData)
                    {
                        List<TalkieCharacter> characters = new List<TalkieCharacter>();

                        //print ("column count: " + column_count);
                        switch (column_count)
                        {
                            case 0: // audio file name
                                // determine active character(s)
                                characters = DetermineActiveCharacters(cell);
                                break;

                            case 1: // text
                                var words = cell.Split(' ');

                                switch (characters[0])
                                {
                                    case TalkieCharacter.None:
                                        break;

                                    case TalkieCharacter.Red:
                                        redWordCount += words.Length;
                                        break;

                                    case TalkieCharacter.Wally:
                                        wallyWordCount += words.Length;
                                        break;

                                    case TalkieCharacter.Darwin:
                                        darwinWordCount += words.Length;
                                        break;

                                    case TalkieCharacter.Lester:
                                        lesterWordCount += words.Length;
                                        break;

                                    case TalkieCharacter.Brutus:
                                        brutusWordCount += words.Length;
                                        break;

                                    case TalkieCharacter.Marcus:
                                        marcusWordCount += words.Length;
                                        break;

                                    case TalkieCharacter.Julius:
                                        juliusWordCount += words.Length;
                                        break;
                                        
                                    case TalkieCharacter.Clogg:
                                        cloggWordCount += words.Length;
                                        break;

                                    case TalkieCharacter.Spindle:
                                        spindleWordCount += words.Length;
                                        break;

                                    case TalkieCharacter.Bubbles:
                                        bubblesWordCount += words.Length;
                                        break;

                                    case TalkieCharacter.Ollie:
                                        ollieWordCount += words.Length;
                                        break;

                                    case TalkieCharacter.Celeste:
                                        celesteWordCount += words.Length;
                                        break;

                                    case TalkieCharacter.Sylvie:
                                        sylvieWordCount += words.Length;
                                        break;
                                }
                                break;
                        }
                    }
                }

                // reading segments
                if (readingSegments)
                {
                    TalkieSegment segment = new TalkieSegment();
                    List<TalkieCharacter> activeCharacters = new List<TalkieCharacter>();

                    foreach(string cell in rowData)
                    {
                        //print ("column count: " + column_count);
                        switch (column_count)
                        {
                            case 0: // audio file name
                                segment.audioClipName = cell;
                                segment.audioClip = SearchForAudioByName(cell); // try to find audio

                                // determine active character(s)
                                activeCharacters = DetermineActiveCharacters(cell);
                                //print ("active characters: " + activeCharacters[0]);

                                break;
                            case 1: // text
                                segment.audioString = cell.Replace("~", ",");
                                break;
                            case 2: // index
                                break;
                            case 3: // left character
                                segment.leftCharacter = GetCharacterFromString(cell);

                                // check to see if active character is left
                                if (activeCharacters.Contains(segment.leftCharacter))
                                {
                                    segment.activeCharacter = ActiveCharacter.Left;
                                }

                                break;
                            case 4: // right character
                                segment.rightCharacter = GetCharacterFromString(cell);

                                // check to see if active character is left
                                if (activeCharacters.Contains(segment.rightCharacter))
                                {
                                    // if active character was prev left -> both are active characters
                                    if (segment.activeCharacter == ActiveCharacter.Left)
                                    {
                                        segment.activeCharacter = ActiveCharacter.Both;
                                    }
                                    // else the right talkie is active
                                    segment.activeCharacter = ActiveCharacter.Right;
                                }

                                break;
                            case 5: // left emote
                                TalkieEmotionEntry leftEmotionEntry = GetEmotionEntryFromString(cell);
                                segment.leftEmotionNum = leftEmotionEntry.emotionNum;
                                segment.leftMouthEnum = leftEmotionEntry.mouthEnum;
                                segment.leftEyesEnum = leftEmotionEntry.eyesEnum;
                                break;
                            case 6: // right emote
                                TalkieEmotionEntry rightEmotionEntry = GetEmotionEntryFromString(cell);
                                segment.rightEmotionNum = rightEmotionEntry.emotionNum;
                                segment.rightMouthEnum = rightEmotionEntry.mouthEnum;
                                segment.rightEyesEnum = rightEmotionEntry.eyesEnum;
                                break;
                            case 7: // require YN?

                                if (cell == "yes")
                                    segment.requireYN = true;
                                else if (cell == "no" || cell == "")
                                    segment.requireYN = false;
                                else
                                    throw new TalkieObjectImportException("invalid \'RequireYN?\' option (should be \'yes\' or \'no\' (or left blank)", lineCount, column_count);
                                break;

                            case 8: // on yes (goto x)

                                if (cell != "")
                                {
                                    string num = cell.Replace("goto", "");
                                    segment.onYesGoto = int.Parse(num);
                                }
                                break;

                            case 9: // on no (goto x)
                                
                                if (cell != "")
                                {
                                    string num = cell.Replace("goto", "");
                                    segment.onNoGoto = int.Parse(num);
                                }
                                break;
                            
                            case 10: // end talkie?

                                if (cell == "yes")
                                    segment.endTalkieAfterThisSegment = true;
                                else if (cell == "no" || cell == "")
                                    segment.endTalkieAfterThisSegment = false;
                                else
                                    throw new TalkieObjectImportException("invalid \'EndTalkie\' option (should be \'yes\' or \'no\' (or left blank)", lineCount, column_count);
                                break;

                            case 11: // talkie motion
                                break;
                        }
                        column_count++;
                    }
                    // add segment to entry
                    entry.segmnets.Add(segment);

                    // count the words characters said in this segment
                    // red
                    if (segment.leftCharacter == TalkieCharacter.Red && segment.activeCharacter == ActiveCharacter.Left ||
                        segment.rightCharacter == TalkieCharacter.Red && segment.activeCharacter == ActiveCharacter.Right)
                    {
                        var words = segment.audioString.Split(' ');
                        redWordCount += words.Length;
                    }
                    // wally
                    if (segment.leftCharacter == TalkieCharacter.Wally && segment.activeCharacter == ActiveCharacter.Left ||
                        segment.rightCharacter == TalkieCharacter.Wally && segment.activeCharacter == ActiveCharacter.Right)
                    {
                        var words = segment.audioString.Split(' ');
                        wallyWordCount += words.Length;
                    }
                    // darwin
                    if (segment.leftCharacter == TalkieCharacter.Darwin && segment.activeCharacter == ActiveCharacter.Left ||
                        segment.rightCharacter == TalkieCharacter.Darwin && segment.activeCharacter == ActiveCharacter.Right)
                    {
                        var words = segment.audioString.Split(' ');
                        darwinWordCount += words.Length;
                    }
                    // lester
                    if (segment.leftCharacter == TalkieCharacter.Lester && segment.activeCharacter == ActiveCharacter.Left ||
                        segment.rightCharacter == TalkieCharacter.Lester && segment.activeCharacter == ActiveCharacter.Right)
                    {
                        var words = segment.audioString.Split(' ');
                        lesterWordCount += words.Length;
                    }
                    // julius
                    if (segment.leftCharacter == TalkieCharacter.Julius && segment.activeCharacter == ActiveCharacter.Left ||
                        segment.rightCharacter == TalkieCharacter.Julius && segment.activeCharacter == ActiveCharacter.Right)
                    {
                        var words = segment.audioString.Split(' ');
                        juliusWordCount += words.Length;
                    }
                    // marcus
                    if (segment.leftCharacter == TalkieCharacter.Marcus && segment.activeCharacter == ActiveCharacter.Left ||
                        segment.rightCharacter == TalkieCharacter.Marcus && segment.activeCharacter == ActiveCharacter.Right)
                    {
                        var words = segment.audioString.Split(' ');
                        marcusWordCount += words.Length;
                    }
                    // brutus
                    if (segment.leftCharacter == TalkieCharacter.Brutus && segment.activeCharacter == ActiveCharacter.Left ||
                        segment.rightCharacter == TalkieCharacter.Brutus && segment.activeCharacter == ActiveCharacter.Right)
                    {
                        var words = segment.audioString.Split(' ');
                        brutusWordCount += words.Length;
                    }


                    // clogg
                    if (segment.leftCharacter == TalkieCharacter.Clogg && segment.activeCharacter == ActiveCharacter.Left ||
                        segment.rightCharacter == TalkieCharacter.Clogg && segment.activeCharacter == ActiveCharacter.Right)
                    {
                        var words = segment.audioString.Split(' ');
                        cloggWordCount += words.Length;
                    }
                    // spindle
                    if (segment.leftCharacter == TalkieCharacter.Spindle && segment.activeCharacter == ActiveCharacter.Left ||
                        segment.rightCharacter == TalkieCharacter.Spindle && segment.activeCharacter == ActiveCharacter.Right)
                    {
                        var words = segment.audioString.Split(' ');
                        spindleWordCount += words.Length;
                    }
                    // bubbles
                    if (segment.leftCharacter == TalkieCharacter.Bubbles && segment.activeCharacter == ActiveCharacter.Left ||
                        segment.rightCharacter == TalkieCharacter.Bubbles && segment.activeCharacter == ActiveCharacter.Right)
                    {
                        var words = segment.audioString.Split(' ');
                        bubblesWordCount += words.Length;
                    }
                    // ollie
                    if (segment.leftCharacter == TalkieCharacter.Ollie && segment.activeCharacter == ActiveCharacter.Left ||
                        segment.rightCharacter == TalkieCharacter.Ollie && segment.activeCharacter == ActiveCharacter.Right)
                    {
                        var words = segment.audioString.Split(' ');
                        ollieWordCount += words.Length;
                    }
                    // celeste
                    if (segment.leftCharacter == TalkieCharacter.Celeste && segment.activeCharacter == ActiveCharacter.Left ||
                        segment.rightCharacter == TalkieCharacter.Celeste && segment.activeCharacter == ActiveCharacter.Right)
                    {
                        var words = segment.audioString.Split(' ');
                        celesteWordCount += words.Length;
                    }
                    // sylvie
                    if (segment.leftCharacter == TalkieCharacter.Sylvie && segment.activeCharacter == ActiveCharacter.Left ||
                        segment.rightCharacter == TalkieCharacter.Sylvie && segment.activeCharacter == ActiveCharacter.Right)
                    {
                        var words = segment.audioString.Split(' ');
                        sylvieWordCount += words.Length;
                    }
                }
            }
            catch (TalkieObjectImportException e)
            {
                passTest = false;
                e.PrintError();
                return;
            }
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
        
        print ("local talkies made: " + localTalkieObjects.Count);
        print ("red word count: " + redWordCount);
        print ("wally word count: " + wallyWordCount);
        print ("darwin word count: " + darwinWordCount);
        print ("lester word count: " + lesterWordCount);
        print ("julius word count: " + juliusWordCount);
        print ("brutus word count: " + brutusWordCount);
        print ("marcus word count: " + marcusWordCount);
        print ("clogg word count: " + cloggWordCount);
        print ("spindle word count: " + spindleWordCount);
        print ("bubbles word count: " + bubblesWordCount);
        print ("ollie word count: " + ollieWordCount);
        print ("celeste word count: " + celesteWordCount);
        print ("sylvie word count: " + sylvieWordCount);
    }

    public void OnUpdatePressed()
    {
        // only continue iff tests complete
        if (!testDone)
            return;
        
        updateText.color = Color.cyan;
        updateText.text = "creating objects...";
        
        // update database using entries
        foreach (var entry in localTalkieObjects)
        {
            TalkieDatabase.instance.UpdateCreateObject(entry);
        }

        updateText.color = Color.green;
        updateText.text = "database updated!";
    }

    string[] characterNames = new string[] { "red", "wally", "darwin", "lester", "brutus", "marcus", "julius", "clogg", "spindle", "bubbles", "ollie", "celeste", "sylvie" };
    private List<TalkieCharacter> DetermineActiveCharacters(string str)
    {
        List<TalkieCharacter> activeCharacters = new List<TalkieCharacter>();

        str = str.ToLower(); // more consistent parsing

        //print ("str: " + str);

        foreach(var name in characterNames)
        {
            if (str.Contains(name))
            {
                //print ("str contains character: " + name);
                activeCharacters.Add(GetCharacterFromString(name));
            }
        }

        return activeCharacters;
    }   

    private TalkieCharacter GetCharacterFromString(string str)
    {
        str = str.ToLower(); // more consistent parsing
        switch (str)
        {
            case "":
            case "none":
            case "empty":
                return TalkieCharacter.None;
            case "red":
                return TalkieCharacter.Red;
            case "wally":
                return TalkieCharacter.Wally;
            case "darwin":
                return TalkieCharacter.Darwin;
            case "lester":
                return TalkieCharacter.Lester;
            case "brutus":
                return TalkieCharacter.Brutus;
            case "marcus":
                return TalkieCharacter.Marcus;
            case "julius":
                return TalkieCharacter.Julius;
            case "clogg":
                return TalkieCharacter.Clogg;
            case "spindle":
                return TalkieCharacter.Spindle;
            case "bubbles": 
                return TalkieCharacter.Bubbles;
            case "ollie":
                return TalkieCharacter.Ollie;
            case "celeste":
                return TalkieCharacter.Celeste;
            case "sylvie":
                return TalkieCharacter.Sylvie;
            default:    
                GameManager.instance.SendError(this, "invalid talkie character: \'" + str + "\'");
                return TalkieCharacter.None;
        }
    }

    private TalkieEmotionEntry GetEmotionEntryFromString(string str)
    {
        // return empty iff str is empty
        if (str == "")
        {
            TalkieEmotionEntry emptyOut = new TalkieEmotionEntry();
            emptyOut.emotionNum = 0;
            emptyOut.mouthEnum = TalkieMouth.None;
            emptyOut.eyesEnum = TalkieEyes.None;
            return emptyOut;
        }

        str = str.ToLower(); // more consistent parsing
        string[] splitData = str.Split('_');

        TalkieEmotionEntry output = new TalkieEmotionEntry();

        // get emotion number
        output.emotionNum = int.Parse(splitData[0]);

        // get mouth enum
        switch (splitData[1])
        {
            default:
                output.mouthEnum = TalkieMouth.None;
                break;
            case "mc":
                output.mouthEnum = TalkieMouth.Closed;
                break;
            case "mo":
                output.mouthEnum = TalkieMouth.Open;
                break;
        }

        // make sure data contains eyes id
        if (splitData.Length > 2)
        {
            // get eyes enum
            switch (splitData[2])
            {
                default:
                    output.eyesEnum = TalkieEyes.None;
                    break;
                case "ei":
                    output.eyesEnum = TalkieEyes.Inwards;
                    break;
                case "ep":
                    output.eyesEnum = TalkieEyes.Player;
                    break;
                case "ec":
                    output.eyesEnum = TalkieEyes.Closed;
                    break;
                case "eo":
                    output.eyesEnum = TalkieEyes.Outwards;
                    break;
            }
        }
        // if not - leave it as NONE
        else
        {
            output.eyesEnum = TalkieEyes.None;
        }

        return output;
    }

    private void InitCreateGlobalList()
    {
        var files = Resources.LoadAll<AudioClip>("TalkieAudioFiles");

        globalTalkieAudioList = new List<AudioClip>();
        foreach (var file in files)
        {
            globalTalkieAudioList.Add(file);
        }

        print ("global audio list: " + globalTalkieAudioList.Count);
    }

    private AudioClip SearchForAudioByName(string str)
    {
        // linear search
        foreach (var file in globalTalkieAudioList)
        {
            if (file.name == str)
            {
                //print ("found audio!");
                return file;
            }
        }
        // return null if not found
        return null;
    }
}
#endif