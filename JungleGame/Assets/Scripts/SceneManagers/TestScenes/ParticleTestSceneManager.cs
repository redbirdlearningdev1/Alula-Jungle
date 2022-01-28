using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParticleTestSceneManager : MonoBehaviour
{
    public TextMeshProUGUI currentParticleText;

    void Start()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // turn on particle controller
        ParticleController.instance.isOn = true;

        // set current particle text
        currentParticleText.text = "current particle character: #" + (int)ParticleController.instance.currentParticleCharacter + " " + ParticleController.instance.currentParticleCharacter.ToString();
    }

    void Update()
    {
        // switch particle types with space
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ParticleController.instance.IncreaseCharacterParticle();
            // set current particle text
            currentParticleText.text = "current particle character: #" + (int)ParticleController.instance.currentParticleCharacter + " " + ParticleController.instance.currentParticleCharacter.ToString();
        }

        // switch particle types with space
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ParticleController.instance.DecreaseCharacterParticle();
            // set current particle text
            currentParticleText.text = "current particle character: #" + (int)ParticleController.instance.currentParticleCharacter + " " + ParticleController.instance.currentParticleCharacter.ToString();
        }
    }
}
