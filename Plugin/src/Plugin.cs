﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalLib.Modules;
using JohnPaularatus;
using JohnPaularatus.Configs;
using UnityEngine;

namespace JohnPaularatusEnemy;
[BepInPlugin(JohnPaularatus.PluginInfo.PLUGIN_GUID, JohnPaularatus.PluginInfo.PLUGIN_NAME, JohnPaularatus.PluginInfo.PLUGIN_VERSION)]
[BepInDependency(LethalLib.Plugin.ModGUID)] 
public class Plugin : BaseUnityPlugin {
    internal static new ManualLogSource Logger;
        public static JohnPaularatusConfig BoundConfig { get; private set; } // prevent from accidently overriding the config
        public static EnemyType JohnPaularatusEnemyType;
        public static GameObject UtilsPrefab;
        public static Dictionary<string, Item> samplePrefabs = [];
        private readonly Harmony _harmony = new Harmony(JohnPaularatus.PluginInfo.PLUGIN_GUID);

    private void Awake() {
        Logger = base.Logger;

        BoundConfig = new JohnPaularatusConfig(this.Config); // Create the config with the file from here.
        Assets.PopulateAssets();
        _harmony.PatchAll(Assembly.GetExecutingAssembly());

        UtilsPrefab = Assets.MainAssetBundle.LoadAsset<GameObject>("JohnPaularatusUtils");

        JohnPaularatusEnemyType = Assets.MainAssetBundle.LoadAsset<EnemyType>("ReadyJPObj");
        TerminalNode JohnPaularatusTerminalNode = Assets.MainAssetBundle.LoadAsset<TerminalNode>("ReadyJPTN");
        TerminalKeyword JohnPaularatusTerminalKeyword = Assets.MainAssetBundle.LoadAsset<TerminalKeyword>("ReadyJPTK");
        NetworkPrefabs.RegisterNetworkPrefab(JohnPaularatusEnemyType.enemyPrefab);

        RegisterEnemyWithConfig(BoundConfig.ConfigJohnPaularatusSpawnWeights.Value, JohnPaularatusEnemyType, JohnPaularatusTerminalNode, JohnPaularatusTerminalKeyword, BoundConfig.ConfigJohnPaularatusPowerLevel.Value, BoundConfig.ConfigJohnPaularatusMaxCount.Value);

        InitializeNetworkBehaviours();
        Logger.LogInfo($"Plugin {JohnPaularatus.PluginInfo.PLUGIN_GUID} is loaded!");
    }

    protected void RegisterEnemyWithConfig(string configMoonRarity, EnemyType enemy, TerminalNode terminalNode, TerminalKeyword terminalKeyword, float powerLevel, int spawnCount) {
        enemy.MaxCount = spawnCount;
        enemy.PowerLevel = powerLevel;
        (Dictionary<Levels.LevelTypes, int> spawnRateByLevelType, Dictionary<string, int> spawnRateByCustomLevelType) = ConfigParsing(configMoonRarity);
        Enemies.RegisterEnemy(enemy, spawnRateByLevelType, spawnRateByCustomLevelType, terminalNode, terminalKeyword);
    }

    protected (Dictionary<Levels.LevelTypes, int> spawnRateByLevelType, Dictionary<string, int> spawnRateByCustomLevelType) ConfigParsing(string configMoonRarity) {
        Dictionary<Levels.LevelTypes, int> spawnRateByLevelType = new Dictionary<Levels.LevelTypes, int>();
        Dictionary<string, int> spawnRateByCustomLevelType = new Dictionary<string, int>();

        foreach (string entry in configMoonRarity.Split(',').Select(s => s.Trim())) {
            string[] entryParts = entry.Split(':');

            if (entryParts.Length != 2) continue;

            string name = entryParts[0];
            int spawnrate;

            if (!int.TryParse(entryParts[1], out spawnrate)) continue;

            if (System.Enum.TryParse(name, true, out Levels.LevelTypes levelType))
            {
                spawnRateByLevelType[levelType] = spawnrate;
            }
            else
            {
                // Try appending "Level" to the name and re-attempt parsing
                string modifiedName = name + "Level";
                if (System.Enum.TryParse(modifiedName, true, out levelType))
                {
                    spawnRateByLevelType[levelType] = spawnrate;
                }
                else
                {
                    spawnRateByCustomLevelType[name] = spawnrate;
                }
            }
        }
        return (spawnRateByLevelType, spawnRateByCustomLevelType);
    }

    private void InitializeNetworkBehaviours() {
        IEnumerable<Type> types;
        try
        {
            types = Assembly.GetExecutingAssembly().GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            types = e.Types.Where(t => t != null);
        }
        foreach (var type in types)
        {
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                if (attributes.Length > 0)
                {
                    method.Invoke(null, null);
                }
            }
        }
    }
}

public static class Assets {
    public static AssetBundle MainAssetBundle = null;
    public static void PopulateAssets() {
        string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        MainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "readyjpassets"));
        if (MainAssetBundle == null) {
            Plugin.Logger.LogError("Failed to load custom assets.");
            return;
        }
    }
}