using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public struct ChallengeWordEntry
{
    public string word;
    public int elkoninCount;
    public List<ElkoninValue> elkoninList;
    public ActionWordEnum set;
    public string imageText;

    public string toString()
    {
        return word + " - elkoninCount: " + elkoninCount + ", set: " + set + ", imageText: " + imageText;
    }
}

public class ChallengeWordDatabaseManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown fileDropdown;
    [SerializeField] private TextMeshProUGUI uploadText;
    [SerializeField] private TextMeshProUGUI textText;
    [SerializeField] private TextMeshProUGUI updateText;

    private List<ChallengeWordEntry> challengeWords;
    private List<string> filePaths;
    private string fileText;

    private bool loadDone = false;
    private bool testDone = false;

    private const string csv_folder_path = "Assets/Resources/CSV_folder/";

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
        string errorMsg = "";

        // change text and color
        textText.color = Color.cyan;
        textText.text = "testing file '" + fileDropdown.options[fileDropdown.value].text + "'";

        challengeWords = new List<ChallengeWordEntry>();

        // split text into elkonin values
        string[] lines = fileText.Split('\n');
        int lineCount = 1;
        foreach (string line in lines)
        {
            // skip first line
            if (firstLine)
            {
                firstLine = false;
                continue;
            }

            var entry = new ChallengeWordEntry();
            entry.elkoninList = new List<ElkoninValue>();
            string[] rowData = line.Split(',');

            // check that row is 10 long
            if (rowData.Length != 10)
            {   
                passTest = false;
                errorMsg = "invalid row size @ line " + lineCount;
                break;
            }

            int column_count = 0;   

            try 
            {
                foreach (string cell in rowData)
                {
                    switch (column_count)
                    {
                        case 0: // word
                            entry.word = cell;
                            break;

                        case 1: // elkonin count
                            entry.elkoninCount = int.Parse(cell);
                            break;

                        case 2: // e1
                        case 3: // e2
                        case 4: // e3
                        case 5: // e4
                        case 6: // e5
                        case 7: // e6
                            entry.elkoninList.Add(ConvertToElkoninValue(cell));
                            break;

                        case 8: // set
                            entry.set = ConvertToActionWord(cell);
                            break;
                        case 9: // image text
                            entry.imageText = cell;
                            break;
                    }
                    column_count++;
                }
            }
            catch
            {
                passTest = false;
                errorMsg = "invalid cell detected @ line " + lineCount + " column " + column_count;
            }

            // add entry to list
            challengeWords.Add(entry);

            lineCount++;
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

        // print out challenge word entires
        // foreach (var entry in challengeWords)
        // {
        //     print (entry.toString());
        // }
    }

    public void OnUpdatePressed()
    {
        // only continue iff tests complete
        if (!testDone)
            return;
        
        updateText.color = Color.cyan;
        updateText.text = "creating objects...";
        
        // update database using entries
        foreach (var entry in challengeWords)
        {
            ChallengeWordDatabase.InitCreateGlobalList();
            ChallengeWordDatabase.UpdateCreateWord(entry);
        }

        updateText.color = Color.green;
        updateText.text = "database updated!";
    }

    private ElkoninValue ConvertToElkoninValue(string val)
    {
        switch (val)
        {
            default:
                return ElkoninValue.empty_gold;
            // action words
            case "mudslide":
                return ElkoninValue.mudslide;
            case "listen":
                return ElkoninValue.listen;
            case "poop":
                return ElkoninValue.poop;
            case "orcs":
                return ElkoninValue.orcs;
            case "think":
                return ElkoninValue.think;
            case "hello":
                return ElkoninValue.hello;
            case "spider":
                return ElkoninValue.spider;
            case "explorer":
                return ElkoninValue.explorer;
            case "scared":
                return ElkoninValue.scared;
            case "scare":
                return ElkoninValue.scared;
            case "that_guy":
                return ElkoninValue.thatguy;
            case "choice":
                return ElkoninValue.choice;
            case "strong_wind":
                return ElkoninValue.strongwind;
            case "pirate":
                return ElkoninValue.pirate;
            case "gorilla":
                return ElkoninValue.gorilla;
            case "sounds":
                return ElkoninValue.sounds;
            case "give":
                return ElkoninValue.give;
            case "backpack":
                return ElkoninValue.backpack;
            case "howler_monkey": 
                return ElkoninValue.frustrating;
            case "frustrating":
                return ElkoninValue.frustrating;
            case "bump_head":
                return ElkoninValue.bumphead;
            case "baby":
                return ElkoninValue.baby;
            case "baby_monkey":
                return ElkoninValue.baby;

            // consonants
            case "b":
                return ElkoninValue.b;
            case "c":
                return ElkoninValue.c;
            case "ch":
                return ElkoninValue.ch;
            case "d":
                return ElkoninValue.d;
            case "f":
                return ElkoninValue.f;
            case "g":
                return ElkoninValue.g;
            case "h":
                return ElkoninValue.h;
            case "j":
                return ElkoninValue.j;
            case "k":
                return ElkoninValue.k;
            case "l":
                return ElkoninValue.l;
            case "m":
                return ElkoninValue.m;
            case "n":
                return ElkoninValue.n;
            case "p":
                return ElkoninValue.p;
            case "qu":
                return ElkoninValue.qu;
            case "r":
                return ElkoninValue.r;
            case "s":
                return ElkoninValue.s;
            case "sh":
                return ElkoninValue.sh;
            case "t":
                return ElkoninValue.t;
            case "th":
                return ElkoninValue.th;
            case "v":
                return ElkoninValue.v;
            case "w":
                return ElkoninValue.w;
            case "x":
                return ElkoninValue.x;
            case "y":
                return ElkoninValue.y;
            case "z":
                return ElkoninValue.z;
        }
    }

    private ActionWordEnum ConvertToActionWord(string val) 
    {
        switch (val)
        {
            // action words
            default:
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

    private void ResetData()
    {
        challengeWords = new List<ChallengeWordEntry>();
        filePaths = new List<string>();

        challengeWords.Clear();
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
    }
}
