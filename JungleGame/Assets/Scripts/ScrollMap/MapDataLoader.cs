using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataLoader : MonoBehaviour
{
    public static MapDataLoader instance;

    [Header("Gorilla Village")]
    public MapIcon GV_house1;
    public MapIcon GV_house2;
    public MapIcon GV_fire;
    public MapIcon GV_statue;

    [Header("Mudslide")]
    public MapIcon MS_logs;
    public MapIcon MS_pond;
    public MapIcon MS_ramp;
    public MapIcon MS_tower;

    [Header("Orc Village")]
    public MapIcon OV_houseL;
    public MapIcon OV_houseS;
    public MapIcon OV_statue;
    public MapIcon OV_fire;

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
        GV_house1.SetFixed(mapData.GV_house1.isFixed, false, false);
        GV_house1.SetStars(mapData.GV_house1.stars);

        GV_house2.SetFixed(mapData.GV_house2.isFixed, false, false);
        GV_house2.SetStars(mapData.GV_house2.stars);

        GV_fire.SetFixed(mapData.GV_fire.isFixed, false, false);
        GV_fire.SetStars(mapData.GV_fire.stars);

        GV_statue.SetFixed(mapData.GV_statue.isFixed, false, false);
        GV_statue.SetStars(mapData.GV_statue.stars);

        // mudslide section
        MS_logs.SetFixed(mapData.MS_logs.isFixed, false, false);
        MS_logs.SetStars(mapData.MS_logs.stars);

        MS_pond.SetFixed(mapData.MS_pond.isFixed, false, false);
        MS_pond.SetStars(mapData.MS_pond.stars);

        MS_ramp.SetFixed(mapData.MS_ramp.isFixed, false, false);
        MS_ramp.SetStars(mapData.MS_ramp.stars);

        MS_tower.SetFixed(mapData.MS_tower.isFixed, false, false);
        MS_tower.SetStars(mapData.MS_tower.stars);

        // orc village section
        OV_houseL.SetFixed(mapData.OV_houseL.isFixed, false, false);
        OV_houseL.SetStars(mapData.OV_houseL.stars);

        OV_houseS.SetFixed(mapData.OV_houseS.isFixed, false, false);
        OV_houseS.SetStars(mapData.OV_houseS.stars);

        OV_statue.SetFixed(mapData.OV_statue.isFixed, false, false);
        OV_statue.SetStars(mapData.OV_statue.stars);

        OV_fire.SetFixed(mapData.OV_fire.isFixed, false, false);
        OV_fire.SetStars(mapData.OV_fire.stars);
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

            case MapIconIdentfier.MS_logs:
                return MS_logs;
            case MapIconIdentfier.MS_pond:
                return MS_pond;
            case MapIconIdentfier.MS_ramp:
                return MS_ramp;
            case MapIconIdentfier.MS_tower:
                return MS_tower;

            case MapIconIdentfier.OV_houseL:
                return OV_houseL;
            case MapIconIdentfier.OV_houseS:
                return OV_houseS;
            case MapIconIdentfier.OV_statue:
                return OV_statue;
            case MapIconIdentfier.OV_fire:
                return OV_fire;
        }
    }
}
