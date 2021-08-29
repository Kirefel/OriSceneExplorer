using System.Collections.Generic;
using UnityEngine;

namespace OriSceneExplorer
{
    public class LogView : EditorView
    {
        List<string> logs = new List<string>();
        Vector2 scrollPosition = Vector2.zero;
        private bool reverse = true;

        public LogView(Rect windowRect) : base(windowRect, "Unity Logs")
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
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            if (!reverse)
            {
                for (int i = 0; i < logs.Count; i++)
                    GUILayout.Label(logs[i]);
            }
            else
            {
                for (int i = logs.Count - 1; i >= 0; i--)
                    GUILayout.Label(logs[i]);
            }
            GUILayout.EndScrollView();
        }

        public void ClearLogs()
        {
            logs.Clear();
        }
    }
}