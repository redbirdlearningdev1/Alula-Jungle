using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReportSceneManager : MonoBehaviour
{
    // current profile parts
    private int currentProfile;
    private List<StudentPlayerData> profiles;
    public Image profileImage;
    public TextMeshProUGUI profileText;

    void Awake()
    {
        GameManager.instance.SceneInit();

        // get all profiles
        profiles = new List<StudentPlayerData>();
        List<StudentPlayerData> allProfiles = StudentInfoSystem.GetAllStudentDatas();
        // determine active profiles
        foreach (var profile in allProfiles)
        {
            if (profile.active)
            {
                profiles.Add(profile);
            }
        }

        // only allow practice iff more than 0 active profiles
        currentProfile = 0;
        if (profiles.Count > 0)
        {
            profileImage.sprite = GameManager.instance.avatars[profiles[currentProfile].profileAvatar];
            profileText.text = profiles[currentProfile].name;
        }
        else
        {
            // no active profiles
        }
    }

    public void OnProfileButtonPressed()
    {
        currentProfile++;
        if (currentProfile > profiles.Count - 1)
        {
            currentProfile = 0;
        }

        // swap current profile
        profileImage.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        profileImage.sprite = GameManager.instance.avatars[profiles[currentProfile].profileAvatar];
        profileText.text = profiles[currentProfile].name;
    }
}
