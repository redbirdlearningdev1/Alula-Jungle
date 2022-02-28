using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockedBoatManager : MonoBehaviour
{
    public static DockedBoatManager instance;

    [Header("Holders")]
    [SerializeField] private GameObject AnimationHolder;

    [Header("Animators")]
    [SerializeField] private Animator spiderAnim;

    [Header("Prefabs")]
    [SerializeField] private GameObject birdPrefab;
    [SerializeField] private GameObject bubblesPrefab;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
