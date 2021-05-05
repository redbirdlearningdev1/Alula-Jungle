using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurntablesGameManager : MonoBehaviour
{
    [SerializeField] private Door door1;
    [SerializeField] private Door door2;
    [SerializeField] private Door door3;
    [SerializeField] private Door door4;

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();
    }

    void Update()
    {
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                bool direction = (Random.Range(0, 2) == 0);
                int angle = Random.Range(0, 360);

                door1.RotateToAngle(angle, direction);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                bool direction = (Random.Range(0, 2) == 0);
                int angle = Random.Range(0, 360);

                door2.RotateToAngle(angle, direction);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                bool direction = (Random.Range(0, 2) == 0);
                int angle = Random.Range(0, 360);

                door3.RotateToAngle(angle, direction);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                bool direction = (Random.Range(0, 2) == 0);
                int angle = Random.Range(0, 360);

                door4.RotateToAngle(angle, direction);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                bool direction = (Random.Range(0, 2) == 0);
                door1.RotateToAngle(0, direction);
                direction = (Random.Range(0, 2) == 0);
                door2.RotateToAngle(0, direction);
                direction = (Random.Range(0, 2) == 0);
                door3.RotateToAngle(0, direction);
                direction = (Random.Range(0, 2) == 0);
                door4.RotateToAngle(0, direction);
            }
        }
    }
}
