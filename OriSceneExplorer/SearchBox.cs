using System;
using UnityEngine;

namespace OriSceneExplorer
{
    public class SearchBox
    {
        public event Action<string> OnSearch;

        static int nextID = 0;

        private readonly string controlName;

        public SearchBox()
        {
            controlName = $"search-{nextID}";
            nextID++;
        }

        private string text = "";
        public void Draw()
        {
            GUILayout.BeginHorizontal();

            GUI.SetNextControlName(controlName);
            text = GUILayout.TextField(text, GUILayout.ExpandWidth(true));

            // Don't ask me why it throws if you press enter, but not if you click the button
            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == controlName)
                Dispatch.Queue(() => OnSearch?.Invoke(text));

            if (GUILayout.Button("Search", GUILayout.Width(100)))
                OnSearch?.Invoke(text);

            if (GUILayout.Button("Clear", GUILayout.Width(100)))
            {
                text = "";
                OnSearch?.Invoke("");
            }

            GUILayout.EndHorizontal();
        }
    }
}