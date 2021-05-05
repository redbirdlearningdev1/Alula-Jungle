using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameIcon : MonoBehaviour
{
    [SerializeField] private Image frameIconImage;
    private ActionWordEnum currentIcon;

    public void SetFrameIcon(ActionWordEnum icon)
    {
        currentIcon = icon;
        frameIconImage.sprite = GameManager.instance.GetActionWord(currentIcon).frameIcon;
    }
}
