
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;

namespace JohnPaularatus.Configs {
    public class JohnPaularatusConfig {
        public ConfigEntry<string> ConfigJohnPaularatusSpawnWeights { get; private set; }
        public ConfigEntry<int> ConfigJohnPaularatusMaxCount { get; private set; }
        public ConfigEntry<int> ConfigJohnPaularatusPowerLevel { get; private set; }

        public JohnPaularatusConfig(ConfigFile configFile) {
            ConfigJohnPaularatusPowerLevel = configFile.Bind("JohnPaularatus Options",
                                                "John Paularatus | Power Level",
                                                3,
                                                "Power Level of the John Paularatus in the moon.");
            ConfigJohnPaularatusSpawnWeights = configFile.Bind("JohnPaularatus Options",
                                                "John Paularatus | Spawn Weights",
                                                "All:66,Modded:66,Vanilla:66",
                                                "Spawn Weight of the John Paularatus in moons.");

            ConfigJohnPaularatusMaxCount = configFile.Bind("JohnPaularatus Options",
                                                "John Paularatus | Max Count",
                                                1,
                                                "Max Count of the JohnPaularatus that spawn naturally in the moon.");
			ClearUnusedEntries(configFile);
        }
        
        private void ClearUnusedEntries(ConfigFile configFile) {
            // Normally, old unused config entries don't get removed, so we do it with this piece of code. Credit to Kittenji.
            PropertyInfo orphanedEntriesProp = configFile.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);
            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(configFile, null);
            orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
            configFile.Save(); // Save the config file to save these changes
        }
    }
}