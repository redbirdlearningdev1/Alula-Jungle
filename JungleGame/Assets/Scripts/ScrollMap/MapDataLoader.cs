using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataLoader : MonoBehaviour
{
    public static MapDataLoader instance;

    // gorilla village section
    [SerializeField] private MapIcon GV_house1;
    [SerializeField] private MapIcon GV_house2;
    [SerializeField] private MapIcon GV_fire;
    [SerializeField] private MapIcon GV_statue;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void LoadMapData(MapData mapData)
    {
        // gorilla village section
        GV_house1.SetFixed(mapData.GV_house1.isFixed);
        GV_house1.SetStars(mapData.GV_house1.stars);

        GV_house2.SetFixed(mapData.GV_house2.isFixed);
        GV_house2.SetStars(mapData.GV_house2.stars);

        GV_fire.SetFixed(mapData.GV_fire.isFixed);
        GV_fire.SetStars(mapData.GV_fire.stars);

        GV_statue.SetFixed(mapData.GV_statue.isFixed);
        GV_statue.SetStars(mapData.GV_statue.stars);
    }
}
