using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataLoader : MonoBehaviour
{
    public static MapDataLoader instance;

    // gorilla village section
    public MapIcon GV_house1;
    public MapIcon GV_house2;
    public MapIcon GV_fire;
    public MapIcon GV_statue;

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

    public MapIcon GetMapIconFromID(MapIconIdentfier id)
    {
        switch (id)
        {
            default:
            case MapIconIdentfier.GV_house1:
                return GV_house1;
            case MapIconIdentfier.GV_house2:
                return GV_house2;
            case MapIconIdentfier.GV_statue:
                return GV_statue;
            case MapIconIdentfier.GV_fire:
                return GV_fire;
        }
    }
}
