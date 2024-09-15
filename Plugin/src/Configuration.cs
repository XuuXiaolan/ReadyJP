
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;

namespace JohnPaularatus.Configs {
    public class JohnPaularatusConfig {
        public JohnPaularatusConfig(ConfigFile configFile) {
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