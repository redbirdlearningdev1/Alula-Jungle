using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadSaveTestSceneManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Toggle isActiveToggle;
    [SerializeField] private TMP_InputField studentNameField;
    [SerializeField] private TMP_InputField totalStarsField;

    private StudentPlayerData studentData;
    
    void Awake() 
    {
        // every scene must call this in Awake()
        GameHelper.SceneInit();
    }

    void Start()
    {
        LoadData();
    }

    void Update()
    {
        // quit game on escape press TODO-> PLACE THIS IN SINGLETON / DEV-MODE
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }

    public void SaveButton()
    {
        if (studentData == null)
            studentData = new StudentPlayerData();
        
        studentData.studentIndex = GetStudentIndex(dropdown.value);

        // get from UI elements
        studentData.active = isActiveToggle.isOn;
        studentData.name = studentNameField.text;
        studentData.totalStars = System.Convert.ToInt32(totalStarsField.text);

        LoadSaveSystem.SaveStudentData(studentData);
    }

    public void LoadData()
    {
        studentData = LoadSaveSystem.LoadStudentData(GetStudentIndex(dropdown.value));
        if (studentData != null)
        {
            // set UI elements
            isActiveToggle.isOn = studentData.active;
            studentNameField.text = studentData.name;
            totalStarsField.text = studentData.totalStars.ToString();
        }
    }

    private StudentIndex GetStudentIndex(int value)
    {
        switch (value)
        {
            case 0: return StudentIndex.student_1;
            case 1: return StudentIndex.student_2;
            case 2: return StudentIndex.student_3;
            default: return StudentIndex.student_1;
        }
    }

    public void OnDevMenuPressed()
    {
        GameHelper.LoadScene("DevMenu", true);
    }
}
