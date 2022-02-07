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

    [Header("Spooky Forest")]
    public MapIcon SF_lamp;
    public MapIcon SF_web;
    public MapIcon SF_shrine;
    public MapIcon SF_spider;

    [Header("Orc Camp")]
    public MapIcon OC_axe;
    public MapIcon OC_bigTent;
    public MapIcon OC_smallTent;
    public MapIcon OC_fire;

    [Header("Gorilla Poop")]
    public MapIcon GP_house1;
    public MapIcon GP_house2;
    public MapIcon GP_rock1;
    public MapIcon GP_rock2;

    [Header("Windy Cliff")]
    public MapIcon WC_statue;
    public MapIcon WC_lighthouse;
    public MapIcon WC_ladder;
    public MapIcon WC_rock;
    public MapIcon WC_sign;
    public MapIcon WC_octo;

    [Header("Pirate Ship")]
    public MapIcon PS_wheel;
    public MapIcon PS_sail;
    public MapIcon PS_boat;
    public MapIcon PS_bridge;
    public MapIcon PS_front;
    public MapIcon PS_parrot;

    [Header("Mermaid Beach")]
    public MapIcon MB_mermaids;
    public MapIcon MB_rock;
    public MapIcon MB_castle;
    public MapIcon MB_bucket;
    public MapIcon MB_umbrella;
    public MapIcon MB_ladder;

    [Header("Ruins")]
    public MapIcon R_lizard1;
    public MapIcon R_lizard2;
    public MapIcon R_caveRock;
    public MapIcon R_pyramid;
    public MapIcon R_face;
    public MapIcon R_arch;

    [Header("Exit Jungle")]
    public MapIcon EJ_puppy;
    public MapIcon EJ_bridge;
    public MapIcon EJ_sign;
    public MapIcon EJ_torch;

    [Header("Gorilla Study")]
    public MapIcon GS_tent1;
    public MapIcon GS_tent2;
    public MapIcon GS_statue;
    public MapIcon GS_fire;

    [Header("Monkeys")]
    public MapIcon M_flower;
    public MapIcon M_tree;
    public MapIcon M_bananas;
    public MapIcon M_guards;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetRoyalRumbleBanner()
    {
        if (StudentInfoSystem.GetCurrentProfile().royalRumbleActive)
        {
            GetMapIconFromID(StudentInfoSystem.GetCurrentProfile().royalRumbleID).SetRoyalRumberBanner(true);
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

        // spooky forest section
        SF_lamp.SetFixed(mapData.SF_lamp.isFixed, false, false);
        SF_lamp.SetStars(mapData.SF_lamp.stars);

        SF_web.SetFixed(mapData.SF_web.isFixed, false, false);
        SF_web.SetStars(mapData.SF_web.stars);

        SF_shrine.SetFixed(mapData.SF_shrine.isFixed, false, false);
        SF_shrine.SetStars(mapData.SF_shrine.stars);

        SF_spider.SetFixed(mapData.SF_spider.isFixed, false, false);
        SF_spider.SetStars(mapData.SF_spider.stars);

        // orc camp section
        OC_axe.SetFixed(mapData.OC_axe.isFixed, false, false);
        OC_axe.SetStars(mapData.OC_axe.stars);

        OC_bigTent.SetFixed(mapData.OC_bigTent.isFixed, false, false);
        OC_bigTent.SetStars(mapData.OC_bigTent.stars);

        OC_smallTent.SetFixed(mapData.OC_smallTent.isFixed, false, false);
        OC_smallTent.SetStars(mapData.OC_smallTent.stars);

        OC_fire.SetFixed(mapData.OC_fire.isFixed, false, false);
        OC_fire.SetStars(mapData.OC_fire.stars);

        // gorilla poop section
        GP_house1.SetFixed(mapData.GP_house1.isFixed, false, false);
        GP_house1.SetStars(mapData.GP_house1.stars);

        GP_house2.SetFixed(mapData.GP_house2.isFixed, false, false);
        GP_house2.SetStars(mapData.GP_house2.stars);

        GP_rock1.SetFixed(mapData.GP_rock1.isFixed, false, false);
        GP_rock1.SetStars(mapData.GP_rock1.stars);

        GP_rock2.SetFixed(mapData.GP_rock2.isFixed, false, false);
        GP_rock2.SetStars(mapData.GP_rock2.stars);

        // windy cliff section
        WC_ladder.SetFixed(mapData.WC_ladder.isFixed, false, false);
        WC_ladder.SetStars(mapData.WC_ladder.stars);

        WC_lighthouse.SetFixed(mapData.WC_lighthouse.isFixed, false, false);
        WC_lighthouse.SetStars(mapData.WC_lighthouse.stars);

        WC_octo.SetFixed(mapData.WC_octo.isFixed, false, false);
        WC_octo.SetStars(mapData.WC_octo.stars);

        WC_rock.SetFixed(mapData.WC_rock.isFixed, false, false);
        WC_rock.SetStars(mapData.WC_rock.stars);

        WC_sign.SetFixed(mapData.WC_sign.isFixed, false, false);
        WC_sign.SetStars(mapData.WC_sign.stars);

        WC_statue.SetFixed(mapData.WC_statue.isFixed, false, false);
        WC_statue.SetStars(mapData.WC_statue.stars);

        // pirate ship section
        PS_boat.SetFixed(mapData.PS_boat.isFixed, false, false);
        PS_boat.SetStars(mapData.PS_boat.stars);

        PS_bridge.SetFixed(mapData.PS_bridge.isFixed, false, false);
        PS_bridge.SetStars(mapData.PS_bridge.stars);

        PS_front.SetFixed(mapData.PS_front.isFixed, false, false);
        PS_front.SetStars(mapData.PS_front.stars);

        PS_parrot.SetFixed(mapData.PS_parrot.isFixed, false, false);
        PS_parrot.SetStars(mapData.PS_parrot.stars);

        PS_sail.SetFixed(mapData.PS_sail.isFixed, false, false);
        PS_sail.SetStars(mapData.PS_sail.stars);

        PS_wheel.SetFixed(mapData.PS_wheel.isFixed, false, false);
        PS_wheel.SetStars(mapData.PS_wheel.stars);

        // mermaid beach section
        MB_bucket.SetFixed(mapData.MB_bucket.isFixed, false, false);
        MB_bucket.SetStars(mapData.MB_bucket.stars);

        MB_castle.SetFixed(mapData.MB_castle.isFixed, false, false);
        MB_castle.SetStars(mapData.MB_castle.stars);

        MB_ladder.SetFixed(mapData.MB_ladder.isFixed, false, false);
        MB_ladder.SetStars(mapData.MB_ladder.stars);

        MB_mermaids.SetFixed(mapData.MB_mermaids.isFixed, false, false);
        MB_mermaids.SetStars(mapData.MB_mermaids.stars);

        MB_rock.SetFixed(mapData.MB_rock.isFixed, false, false);
        MB_rock.SetStars(mapData.MB_rock.stars);

        MB_umbrella.SetFixed(mapData.MB_umbrella.isFixed, false, false);
        MB_umbrella.SetStars(mapData.MB_umbrella.stars);

        // ruins section
        R_arch.SetFixed(mapData.R_arch.isFixed, false, false);
        R_arch.SetStars(mapData.R_arch.stars);

        R_caveRock.SetFixed(mapData.R_caveRock.isFixed, false, false);
        R_caveRock.SetStars(mapData.R_caveRock.stars);

        R_face.SetFixed(mapData.R_face.isFixed, false, false);
        R_face.SetStars(mapData.R_face.stars);

        R_lizard1.SetFixed(mapData.R_lizard1.isFixed, false, false);
        R_lizard1.SetStars(mapData.R_lizard1.stars);

        R_lizard2.SetFixed(mapData.R_lizard2.isFixed, false, false);
        R_lizard2.SetStars(mapData.R_lizard2.stars);

        R_pyramid.SetFixed(mapData.R_pyramid.isFixed, false, false);
        R_pyramid.SetStars(mapData.R_pyramid.stars);

        // exit jungle section
        EJ_bridge.SetFixed(mapData.EJ_bridge.isFixed, false, false);
        EJ_bridge.SetStars(mapData.EJ_bridge.stars);

        EJ_puppy.SetFixed(mapData.EJ_puppy.isFixed, false, false);
        EJ_puppy.SetStars(mapData.EJ_puppy.stars);

        EJ_sign.SetFixed(mapData.EJ_sign.isFixed, false, false);
        EJ_sign.SetStars(mapData.EJ_sign.stars);

        EJ_torch.SetFixed(mapData.EJ_torch.isFixed, false, false);
        EJ_torch.SetStars(mapData.EJ_torch.stars);

        // gorilla study section
        GS_fire.SetFixed(mapData.GS_fire.isFixed, false, false);
        GS_fire.SetStars(mapData.GS_fire.stars);

        GS_statue.SetFixed(mapData.GS_statue.isFixed, false, false);
        GS_statue.SetStars(mapData.GS_statue.stars);

        GS_tent1.SetFixed(mapData.GS_tent1.isFixed, false, false);
        GS_tent1.SetStars(mapData.GS_tent1.stars);

        GS_tent2.SetFixed(mapData.GS_tent2.isFixed, false, false);
        GS_tent2.SetStars(mapData.GS_tent2.stars);

        // monkeys section
        M_bananas.SetFixed(mapData.M_bananas.isFixed, false, false);
        M_bananas.SetStars(mapData.M_bananas.stars);

        M_bananas.SetFixed(mapData.M_bananas.isFixed, false, false);
        M_bananas.SetStars(mapData.M_bananas.stars);

        M_guards.SetFixed(mapData.M_guards.isFixed, false, false);
        M_guards.SetStars(mapData.M_guards.stars);

        M_tree.SetFixed(mapData.M_tree.isFixed, false, false);
        M_tree.SetStars(mapData.M_tree.stars);

    }

    public MapIcon GetMapIconFromID(MapIconIdentfier id)
    {
        switch (id)
        {
            default:
            // GV
            case MapIconIdentfier.GV_house1:
                return GV_house1;
            case MapIconIdentfier.GV_house2:
                return GV_house2;
            case MapIconIdentfier.GV_statue:
                return GV_statue;
            case MapIconIdentfier.GV_fire:
                return GV_fire;
            // MS
            case MapIconIdentfier.MS_logs:
                return MS_logs;
            case MapIconIdentfier.MS_pond:
                return MS_pond;
            case MapIconIdentfier.MS_ramp:
                return MS_ramp;
            case MapIconIdentfier.MS_tower:
                return MS_tower;
            // OV
            case MapIconIdentfier.OV_houseL:
                return OV_houseL;
            case MapIconIdentfier.OV_houseS:
                return OV_houseS;
            case MapIconIdentfier.OV_statue:
                return OV_statue;
            case MapIconIdentfier.OV_fire:
                return OV_fire;
            // SF
            case MapIconIdentfier.SF_lamp:
                return SF_lamp;
            case MapIconIdentfier.SF_shrine:
                return SF_shrine;
            case MapIconIdentfier.SF_spider:
                return SF_spider;
            case MapIconIdentfier.SF_web:
                return SF_web;
            // OC
            case MapIconIdentfier.OC_axe:
                return OC_axe;
            case MapIconIdentfier.OC_bigTent:
                return OC_bigTent;
            case MapIconIdentfier.OC_smallTent:
                return OC_smallTent;
            case MapIconIdentfier.OC_fire:
                return OC_fire;
            // GP
            case MapIconIdentfier.GP_outhouse1:
                return GP_house1;
            case MapIconIdentfier.GP_outhouse2:
                return GP_house2;
            case MapIconIdentfier.GP_rocks1:
                return GP_rock1;
            case MapIconIdentfier.GP_rocks2:
                return GP_rock2;
            // WC
            case MapIconIdentfier.WC_ladder:
                return WC_ladder;
            case MapIconIdentfier.WC_lighthouse:
                return WC_lighthouse;
            case MapIconIdentfier.WC_octo:
                return WC_octo;
            case MapIconIdentfier.WC_rock:
                return WC_rock;
            case MapIconIdentfier.WC_sign:
                return WC_sign;
            case MapIconIdentfier.WC_statue:
                return WC_statue;
            // PS
            case MapIconIdentfier.PS_boat:
                return PS_boat;
            case MapIconIdentfier.PS_bridge:
                return PS_bridge;
            case MapIconIdentfier.PS_front:
                return PS_front;
            case MapIconIdentfier.PS_parrot:
                return PS_parrot;
            case MapIconIdentfier.PS_sail:
                return PS_sail;
            case MapIconIdentfier.PS_wheel:
                return PS_wheel;
            // MB
            case MapIconIdentfier.MB_bucket:
                return MB_bucket;
            case MapIconIdentfier.MB_castle:
                return MB_castle;
            case MapIconIdentfier.MB_ladder:
                return MB_ladder;
            case MapIconIdentfier.MB_mermaids:
                return MB_mermaids;
            case MapIconIdentfier.MB_rock:
                return MB_rock;
            case MapIconIdentfier.MB_umbrella:
                return MB_umbrella;
            // R
            case MapIconIdentfier.R_arch:
                return R_arch;
            case MapIconIdentfier.R_caveRock:
                return R_caveRock;
            case MapIconIdentfier.R_face:
                return R_face;
            case MapIconIdentfier.R_lizard1:
                return R_lizard1;
            case MapIconIdentfier.R_lizard2:
                return R_lizard2;
            case MapIconIdentfier.R_pyramid:
                return R_pyramid;
            // EJ
            case MapIconIdentfier.EJ_bridge:
                return EJ_bridge;
            case MapIconIdentfier.EJ_puppy:
                return EJ_puppy;
            case MapIconIdentfier.EJ_sign:
                return EJ_sign;
            case MapIconIdentfier.EJ_torch:
                return EJ_torch;
            // GS
            case MapIconIdentfier.GS_fire:
                return GS_fire;
            case MapIconIdentfier.GS_statue:
                return GS_statue;
            case MapIconIdentfier.GS_tent1:
                return GS_tent1;
            case MapIconIdentfier.GS_tent2:
                return GS_tent2;
            // M
            case MapIconIdentfier.M_bananas:
                return M_bananas;
            case MapIconIdentfier.M_flower:
                return M_flower;
            case MapIconIdentfier.M_guards:
                return M_guards;
            case MapIconIdentfier.M_tree:
                return M_tree;
        }
    }
}
