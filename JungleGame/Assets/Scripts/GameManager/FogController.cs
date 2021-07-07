using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{
    public static FogController instance;

    public bool animateShake;
    public float shakeSpeed;
    public float shakeAmount;

    public float mapXpos;
    [SerializeField] private Transform mapFogObject;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!animateShake)
            return;
        
        Vector3 pos = new Vector3(0f, 0f, 0f);
        pos.x = mapXpos + Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        mapFogObject.localPosition = pos;
    }

    public void MoveFogAnimation(float newXpos, float time)
    {
        StartCoroutine(MoveFogRoutine(newXpos, time));
    }

    private IEnumerator MoveFogRoutine(float newXpos, float time)
    {
        float timer = 0f;
        float startX = mapXpos;
        
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= time) 
            {
                mapXpos = newXpos;
                break;
            }

            mapXpos = Mathf.Lerp(startX, newXpos, timer / time);
            yield return null;
        }
    }
}
