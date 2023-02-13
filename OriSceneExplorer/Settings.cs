using System;
using System.IO;
using UnityEngine;

namespace OriSceneExplorer.Configuration
{
    [Serializable]
    public class Settings
    {
        private const string FileName = "ose_settings.json";

        public bool AutoPause = true;
        public float PropertyNameColumnWidth = 240;
        public float PropertyTypeColumnWidth = 160;
        public KeyCode ToggleView = KeyCode.F1;
        public KeyCode Suspend = KeyCode.F2;
        public KeyCode RefreshHierarchy = KeyCode.F5;
        public KeyCode ClearLogs = KeyCode.F4;
        public KeyCode PickObject = KeyCode.Mouse2;

        public static Settings LoadAll()
        {
            Settings settingsObject;

            if (!File.Exists(FileName))
            {
                settingsObject = new Settings();
            }
            else
            {
                string str = File.ReadAllText(FileName);
                settingsObject = JsonUtility.FromJson<Settings>(str);
            }

            settingsObject.WriteFile();
            return settingsObject;
        }

        public void WriteFile()
        {
            File.WriteAllText(FileName, JsonUtility.ToJson(this, true));
        }
    }
}
