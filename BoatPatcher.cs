using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using System.Reflection;
using UnityEngine;      
using System.IO;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace ShatteredSeasExpansion
{
    public class MatLib
    {
        //Declaring the materials to use
        public static Material convexHull;
        public static Material foam;
        public static Material objectInteraction;
        public static Material water4;
        public static Material particleSplash;
        //public static Material mask;

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
            //mask = GameObject.Find(boatMain).transform.Find(boatSub).transform.Find(structConst).transform.Find

            if(convexHull != null && foam != null && objectInteraction != null && water4 != null)
            {
                Debug.LogWarning("Clipper: Materials Registered");
            }
        }
    }

    internal class Patches
    {
        

        [HarmonyPatch(typeof(FloatingOriginManager))]
        public static class FloatingOriginManagerPatches
        {
            public static GameObject veilPiercer;
            public static GameObject embarkVeilPiercer;
            public static GameObject dockMoorOne;
            public static GameObject dockMoorTwo;
            public static bool boatInstalled;
            public static bool cleanerPres;

            [HarmonyPrefix]
            [HarmonyPatch("Start")]
            public static void StartPatch(FloatingOriginManager __instance)
            {
                cleanerCheck();
                if(__instance.name == "_shifting world" && cleanerPres == false)
                {
                    ShipSetup();
                    if(veilPiercer == null)
                    {
                        Debug.LogError("Clipper in null");
                    }

                    else
                    {
                        GameObject clipper = Object.Instantiate(veilPiercer, __instance.transform);
                        embarkVeilPiercer = GameObject.Find("WALK Shroud Large");
                        dockMoorOne = GameObject.Find("dock_mooring SC (1)");
                        dockMoorTwo = GameObject.Find("dock_mooring SC (2)");  
                        var walkCol = GameObject.Find("walk cols").transform;
                        var islandFort = GameObject.Find("island 15 M (Fort)").transform;
                        embarkVeilPiercer.transform.SetParent(walkCol);
                        embarkVeilPiercer.transform.localPosition = new Vector3(-19, 0, -100);
                        dockMoorOne.transform.SetParent(islandFort);
                        dockMoorTwo.transform.SetParent(islandFort);
                        //dockMoorOne.transform.localPosition = new Vector3(-207, 2, 56);
                        //dockMoorTwo.transform.localPosition = new Vector3(-200, 2, 31);

                        MatLib.RegisterMaterials();
                        clipper.transform.Find("WaterFoam (1)").GetComponent<MeshRenderer>().material = MatLib.foam;
                        clipper.transform.Find("WaterObjectInteractionSphereBack").GetComponent<MeshRenderer>().material = MatLib.objectInteraction;
                        clipper.transform.Find("WaterObjectInteractionSphereFront").GetComponent<MeshRenderer>().material = MatLib.objectInteraction;
                        clipper.transform.Find("shroud large").transform.Find("Clipper Mask").GetComponent<MeshRenderer>().material = MatLib.convexHull;
                        clipper.transform.Find("shroud large").transform.Find("damage_water").GetComponent<MeshRenderer>().material = MatLib.water4;
                        clipper.transform.Find("overflow particles (2)").GetComponent<Renderer>().material = MatLib.particleSplash;
                        clipper.transform.Find("overflow particles (3)").GetComponent<Renderer>().material = MatLib.particleSplash;
                        clipper.GetComponent<BoatMooringRopes>().mooringFront = dockMoorOne.transform;
                        clipper.GetComponent<BoatMooringRopes>().mooringBack = dockMoorTwo.transform;
                        Debug.LogWarning("Clipper loaded");
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

                string assetPath = Paths.PluginPath + "\\ShatteredSeasExpansion\\Veil Piercer";
                if(!File.Exists(assetPath))
                {
                    Debug.LogError("Clipper not installed correctly or removed");
                    boatInstalled = false;
                }
                else
                {
                    var bundle = AssetBundle.LoadFromFile(assetPath);
                    string clipperPath = "Assets/Shattered Seas/BOAT Shroud Large.prefab";
                    veilPiercer = bundle.LoadAsset(clipperPath) as GameObject;
                    boatInstalled = true;
                }
                
            }
            private static void cleanerCheck()
            {
                string cleanerPath = Paths.PluginPath + "\\ClipperSaveCleaner.dll";
                if (File.Exists(cleanerPath))
                {
                    cleanerPres = true;
                    Debug.LogWarning("Save Cleaner Detected");
                }
                else
                {
                    cleanerPres = false;
                }
            }
        }
    }

    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class BoatPatcher : BaseUnityPlugin
    {
        public const string pluginGuid = "com.TheOriginOfAllEvil.ShatteredSeas";
        internal const string pluginName = "Shattered Seas";
        public const string pluginVersion = "0.1.3";
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
