using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropdownToolbar : MonoBehaviour
{
    public static DropdownToolbar instance;

    [SerializeField] private float hiddenHeight;
    [SerializeField] private float visibleHeight;
    [SerializeField] private float moveTime;

    [SerializeField] private TextMeshProUGUI silverText;
    [SerializeField] private TextMeshProUGUI goldText;

    private bool visable;

    void Awake()
    {
        if (instance == null)
            instance = this;
        
        // set toolbar to not be visible
        visable = false;
        transform.localPosition = new Vector3(0f, hiddenHeight, 0f);
    }

    void Update()
    {
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                ToggleToolbar(false);
            if (Input.GetKeyDown(KeyCode.DownArrow))
                ToggleToolbar(true);
        }
    }

    public void LoadToolbarDataFromProfile()
    {
        var data = StudentInfoSystem.currentStudentPlayer;
        // update gold and silver coins
        SetGoldText(StudentInfoSystem.currentStudentPlayer.goldCoins.ToString());
        UpdateSilverCoins();
    }

    public void AwardGoldCoins(int amountToAward)
    {
        // add and save to profile
        StudentInfoSystem.currentStudentPlayer.goldCoins += amountToAward;
        StudentInfoSystem.SaveStudentPlayerData();

        // update toolbar string
        SetGoldText(StudentInfoSystem.currentStudentPlayer.goldCoins.ToString());
    }

    public void RemoveGoldCoins(int amountToRemove)
    {
        // remove and save to profile
        StudentInfoSystem.currentStudentPlayer.goldCoins -= amountToRemove;
        StudentInfoSystem.SaveStudentPlayerData();

        // update toolbar string
        SetGoldText(StudentInfoSystem.currentStudentPlayer.goldCoins.ToString());
    }

    public void UpdateSilverCoins()
    {
        silverText.text = StudentInfoSystem.currentStudentPlayer.unlockedStickers.Count.ToString() + "/999";
    }

    private void SetGoldText(string text)
    {
        goldText.text = "x" + text;
    }

    public void ToggleToolbar(bool opt)
    {
        if (opt == visable)
            return;
        else
            StartCoroutine(ToggleToolbarRoutine(opt));
    }

    private IEnumerator ToggleToolbarRoutine(bool opt)
    {
        float start, end;
        if (opt)
        {
            start = hiddenHeight;
            end = visibleHeight;
            visable = true;
        }
        else
        {
            start = visibleHeight;
            end = hiddenHeight;
            visable = false;
        }

        float timer = 0f;
        while (true)
        {   
            timer += Time.deltaTime;
            if (timer > moveTime)
            {
                break;
            }

            float height = Mathf.Lerp(start, end, timer / moveTime);
            transform.localPosition = new Vector3(0f, height, 0f);

            yield return null;
        }
    }
}
