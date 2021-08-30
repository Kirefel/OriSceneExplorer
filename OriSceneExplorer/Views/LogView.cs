using System.Collections.Generic;
using UnityEngine;

namespace OriSceneExplorer
{
    public class LogView : EditorView
    {
        List<string> logs = new List<string>();
        Vector2 scrollPosition = Vector2.zero;

        public LogView(int col, int row, int width, int height) : base(col, row, width, height, "Unity Logs")
        {
            Application.logMessageReceived += Application_logMessageReceived;
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            logs.Add($"{Time.time:F5} [{type}] {condition}");
            if (!string.IsNullOrEmpty(stackTrace))
                logs.Add(stackTrace);
        }

        protected override void Draw(int windowID)
        {
            if (GUILayout.Button("Clear", GUILayout.Width(150)))
                ClearLogs();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < logs.Count; i++)
                GUILayout.Label(logs[i]);
            GUILayout.EndScrollView();
        }

        public void ClearLogs()
        {
            logs.Clear();
        }
    }
}