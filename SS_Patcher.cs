using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using System.IO;
using SS_ScriptHandler;

namespace ShroudSmallPatcher
{
    public class MatLib
    {
        //Declaring the materials to use
        public static Material convexHull;
        public static Material foam;
        public static Material objectInteraction;
        public static Material water4;
        public static Material particleSplash;
        public static Material mask;

        //Declaring the main file paths
        public const string boatMain = "BOAT medi medium (50)";
        public const string boatSub = "medi medium new";
        public const string structConst = "structure_container";

        public static void RegisterMaterials()
        {
            convexHull = GameObject.Find(boatMain).transform.Find(boatSub).transform.Find(structConst).transform.Find("mask").GetComponent<MeshRenderer>().material;
            foam = GameObject.Find(boatMain).transform.Find("WaterFoam (1)").GetComponent<MeshRenderer>().material;
            objectInteraction = GameObject.Find(boatMain).transform.Find("WaterObjectInteractionSphereBack").GetComponent<MeshRenderer>().material;
            water4 = GameObject.Find(boatMain).transform.Find(boatSub).transform.Find("damage_water").GetComponent<MeshRenderer>().material;
            particleSplash = GameObject.Find(boatMain).transform.Find("overflow particles (2)").GetComponent<Renderer>().material;
            mask = GameObject.Find(boatMain).transform.Find(boatSub).transform.Find(structConst).transform.Find("mask_splash").GetComponent<MeshRenderer>().material;

            if (convexHull != null && foam != null && objectInteraction != null && water4 != null)
            {
                Debug.LogWarning("Sh'ba: Materials Registered");
            }
        }
    }
    /*
    public class DoorSetup
    {

        public static void doorSetup(GameObject boat, object[,] doors)
        {
            Transform mainContainer = boat.transform.Find("shroud small");
            Transform functionContainer = mainContainer.Find("Functionals_container");
            Transform doorContainer = functionContainer.Find("Doors Group"); //1
            Transform interiorDoors = doorContainer.Find("Interior Doors Group");

            Transform mainHatchCont = doorContainer.Find("Main_Hatch_Slider"); //2
            Transform Bulkhead1Cont = interiorDoors.Find("Bulkhead 1 Doors"); //3
            Transform Bulkhead3Cont = interiorDoors.Find("Bulkhead 3 Doors"); //4
            Transform FABulkheadCont = interiorDoors.Find("Fore and Aft Bulkhead Doors"); //5
            Transform AftWallCont = interiorDoors.Find("Aft Wall Hatches"); //6
            Transform hatchBase1 = doorContainer.Find("Hatch_Base"); //7
            Transform hatchBase2 = doorContainer.Find("Hatch Base Weather Hardened"); //8

            Transform tempCont;
            for (int i = 0; i < doors.GetLength(0)-1; i++)
            {
                switch (doors[i, 1])
                {
                    case 1:
                        tempCont = doorContainer;
                        break;

                    case 2:
                        tempCont = mainHatchCont;
                        break;

                    case 3:
                        tempCont = Bulkhead1Cont;
                        break;

                    case 4:
                        tempCont = Bulkhead3Cont;
                        break;

                    case 5:
                        tempCont = FABulkheadCont;
                        break;

                    case 6:
                        tempCont = AftWallCont;
                        break;

                    case 7:
                        tempCont = hatchBase1;
                        break;

                    case 8:
                        tempCont = hatchBase2;
                        break;

                    default:
                        tempCont = doorContainer;
                        break;
                }
                if(doors[i, 0] == null || tempCont == null)
                {
                    Debug.LogError("Door list null");
                    return;
                }
                else
                {
                    //Debug.LogWarning(doors[i, 0] as string);
                    Debug.LogWarning(tempCont.name);
                    string tempString = doors[i, 0] as string;
                    Debug.LogWarning(tempString);
                    Transform currentCont = tempCont.Find(tempString);
                    Debug.LogWarning(currentCont.name);
                    Debug.LogWarning("Component Added");
                }
                
            }

        }
    }*/

    internal class Patches
    {


        [HarmonyPatch(typeof(FloatingOriginManager))]
        public static class FloatingOriginManagerPatches
        {
            public static GameObject riverSloop;
            public static GameObject embarkBoat;
            public static bool boatInstalled;

            public static object[,] doors =
            {
                { "Right_Cabin_Door", 2},
                { "Left_Cabin_Door", 2},
                { "Cabin_Sliding_Roof", 2},
                { "Bulkhead_1a_High_Door", 3 },
                { "Bulkhead_1a_Low_Door", 3 },
                { "Bulkhead_1b_High_Door", 3 },
                { "Bulkhead_1b_Low_Door", 3 },
                { "Bulkhead_3a_High_Door", 4 },
                { "Bulkhead_3a_Low_Door", 4 },
                { "Bulkhead_3b_High_Door", 4 },
                { "Bulkhead_3b_Low_Door", 4 },
                { "Bulkhead_FA_1_High_Door", 5 },
                { "Bulkhead_FA_1_Low_Door", 5 },
                { "Aft_Wall_B_Low_Hatch", 6 },
                { "Aft_Wall_B_High_Hatch", 6 },
                { "Hatch_Type_1_Starboard", 7 },
                { "Hatch_Type_1_Port", 7 },
                { "Hold_Grate", 1 },
                { "Hatch_Type_2_Starboard", 8 },
                { "Hatch_Type_2_Port", 8 }
            };

            [HarmonyPrefix]
            [HarmonyPatch("Start")]
            public static void StartPatch(FloatingOriginManager __instance)
            {
                if (__instance.name == "_shifting world")
                {
                    ShipSetup();
                    if (riverSloop == null)
                    {
                        Debug.LogError("Sh'ba is null");
                    }

                    else
                    {
                        GameObject sloop = Object.Instantiate(riverSloop, __instance.transform);
                        embarkBoat = GameObject.Find("WALK Shroud Small");
                        var walkCol = GameObject.Find("walk cols").transform;
                        embarkBoat.transform.SetParent(walkCol);
                        embarkBoat.transform.localPosition = new Vector3(-200, 0, -100);

                        doorSetup(sloop);

                        MatLib.RegisterMaterials();
                        sloop.transform.Find("WaterFoam").GetComponent<MeshRenderer>().material = MatLib.foam;
                        sloop.transform.Find("WaterObjectInteractionSphereBack").GetComponent<MeshRenderer>().material = MatLib.objectInteraction;
                        sloop.transform.Find("WaterObjectInteractionSphereFront").GetComponent<MeshRenderer>().material = MatLib.objectInteraction;
                        sloop.transform.Find("shroud small").transform.Find("Sh'ba Mask").GetComponent<MeshRenderer>().material = MatLib.convexHull;
                        sloop.transform.Find("shroud small").transform.Find("damage_water").GetComponent<MeshRenderer>().material = MatLib.water4;
                        sloop.transform.Find("overflow particles left").GetComponent<Renderer>().material = MatLib.particleSplash;
                        sloop.transform.Find("overflow particles right").GetComponent<Renderer>().material = MatLib.particleSplash;
                        //sloop.transform.Find("shroud small").transform.Find("Sloop_Splash_Mask").GetComponent<MeshRenderer>().material = MatLib.mask;

                        Debug.LogWarning("Boat loaded");
                    }
                }
                else
                {
                    GameState.currentBoat = null;
                    GameState.lastBoat = null;
                }
            }
            private static void ShipSetup()
            {   //load the assets from the bundle, assigns the script to the assets

                string assetPath = Paths.PluginPath + "\\Shattered Seas Small\\shroud small";
                if (!File.Exists(assetPath))
                {
                    Debug.LogError("Boat not installed correctly or removed");
                    boatInstalled = false;
                }
                else
                {
                    var bundle = AssetBundle.LoadFromFile(assetPath);
                    string boatPath = "Assets/Shattered Seas/BOAT Shroud Small (160).prefab";
                    riverSloop = bundle.LoadAsset(boatPath) as GameObject;
                    boatInstalled = true;
                }

            }

            private static void doorSetup(GameObject boat)
            {
                foreach (GPButtonTrapdoor component in boat.GetComponentsInChildren<GPButtonTrapdoor>())
                {
                    component.gameObject.AddComponent<DoorColToggler>();
                }
            }
        }
    }
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class BoatPatcher : BaseUnityPlugin
    {
        public const string pluginGuid = "com.TheOriginOfAllEvil.riverSloop";
        internal const string pluginName = "Shattered Seas Shroud Small";
        public const string pluginVersion = "1.2.0";
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {pluginGuid} is loaded!");

            //Initialising Harmony Instance
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), pluginGuid);
        }
    }
}

